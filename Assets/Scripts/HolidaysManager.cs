using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolidaysManager : MonoBehaviour
{
    [SerializeField] private string configPath;
    
    public void Start()
    {
        HolidayConfigLoader configLoader = new HolidayConfigLoader();
        HolidaysWrapper config = configLoader.LoadConfig(configPath);
        foreach (HolidayEntry holiday in config.holidaySchedule)
        {
            Logger.Log(holiday.GetHolidayType() + $"date: {holiday.GetStartDate} - {holiday.GetEndDate}");
        }
    }
}
