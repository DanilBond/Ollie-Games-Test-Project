using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class HolidayConfigLoader
{
    public HolidaysWrapper LoadConfig(string path)
    {
        HolidaysWrapper result = new HolidaysWrapper();

        try
        {
            TextAsset jsonFile = Resources.Load<TextAsset>(path);
            //Null file check
            if (jsonFile == null || string.IsNullOrWhiteSpace(jsonFile.text))
            {
                Logger.LogWarning("Config not found or empty. Using default empty config.");
                return result;
            }
            
            //JSON parse
            HolidaysWrapper rawHolidays;
            try
            {
                rawHolidays = JsonConvert.DeserializeObject<HolidaysWrapper>(jsonFile.text);
            }
            catch (Exception e)
            {
                Logger.LogWarning($"Config JSON malformed: {e.Message}. Using empty config.");
                return result;
            }
            
            //Config validity check
            if (rawHolidays == null || !rawHolidays.IsValid())
            {
                Logger.LogWarning("Config is invalid or empty. Using empty config.");
                return result;
            }
            
            //Filtering invalid entries in confug
            result.holidaySchedule.Clear();
            foreach (HolidayEntry entry in rawHolidays.holidaySchedule)
            {
                if (!entry.IsValid())
                {
                    Logger.LogWarning("Entry structure incorrect. Skipping");
                    continue;
                }
                result.holidaySchedule.Add(entry);
            }
            
            return result;
        }
        catch (Exception e)
        {
            Logger.LogWarning($"Unexpected error while loading config: {e.Message}. Using empty config.");
            return result;
        }
    }
}
