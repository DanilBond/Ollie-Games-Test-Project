using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolidaysManager : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private string configPath;
    [SerializeField, Tooltip("May be more expensive")] private bool fetchConfigOnUpdate;
    [SerializeField] private HolidayItemData[] allHolidays;

    [Header("UI")] 
    [SerializeField] private Transform topHolidaysContainer;
    [SerializeField] private Transform bottomHolidaysContainer;

    
    //Private props
    //private HolidaysWrapper _config;
    private Dictionary<string, HolidayEntry> _holidaysSchedule = new Dictionary<string, HolidayEntry>(StringComparer.OrdinalIgnoreCase);
    
    //References
    private HolidayConfigLoader _configLoader;

    public int testTimeOffset;

    private void Update()
    {
        TimeManager.SetTimeOffset(testTimeOffset);
        CreateHolidays();
    }

    public void Start()
    {
        Init();
    }

    private void Init()
    {
        BuildConfigData();
        CreateHolidays();
    }

    private void BuildConfigData()
    {
        _configLoader ??= new HolidayConfigLoader();
        HolidaysWrapper config = _configLoader.LoadConfig(configPath);
        
        //Making all holidays unique by adding them into dict, and it will be more performant if we need to get holiday my type
        _holidaysSchedule = new Dictionary<string, HolidayEntry>(StringComparer.OrdinalIgnoreCase);
        foreach (HolidayEntry entry in config.holidaySchedule)
            _holidaysSchedule.TryAdd(entry.GetHolidayType(), entry);
    }

    private void CreateHolidays()
    {
        if (fetchConfigOnUpdate) 
            BuildConfigData();
        
        //Remove existing
        if (topHolidaysContainer)
            foreach (Transform item in topHolidaysContainer) Destroy(item.gameObject);
        if (bottomHolidaysContainer)
            foreach (Transform item in bottomHolidaysContainer) Destroy(item.gameObject);
        
        //Spawn new
        foreach (var entry in _holidaysSchedule)
        {
            HolidayItemData holidayItemData = GetHolidayItemData(entry.Value.GetHolidayType());
            if (holidayItemData == null || !holidayItemData.IsValid()) continue;
            if (!entry.Value.IsInTimeRange(TimeManager.GetUtcTime())) continue;

            HolidayView HolidayViewInstance = Instantiate(holidayItemData.GetHolidayViewPrefab,
                GetContainerForHoliday(holidayItemData.GetHolidayAlignment));
            
            HolidayViewInstance.Init(entry.Value);
        }
        // foreach (HolidayEntry entry in _config.holidaySchedule)
        // {
        //     HolidayItemData holidayItemData = GetHolidayItemData(entry.GetHolidayType());
        //     if (!holidayItemData && !holidayItemData.IsValid()) continue;
        //     if (!entry.IsInTimeRange(TimeManager.GetUtcTime())) continue;
        //
        //     HolidayView HolidayViewInstance = Instantiate(holidayItemData.GetHolidayViewPrefab,
        //         GetContainerForHoliday(holidayItemData.GetHolidayAlignment));
        //     
        //     HolidayViewInstance.Init(entry);
        // }
    }

    private HolidayItemData GetHolidayItemData(string holidayType)
    {
        for (var i = 0; i < allHolidays.Length; i++)
        {
            if (string.Equals(allHolidays[i].GetHolidayType, holidayType, StringComparison.OrdinalIgnoreCase)) 
                return allHolidays[i];
        }

        Logger.LogWarning("Holiday type '" + holidayType + "' not found");
        return null;
    }

    private Transform GetContainerForHoliday(HolidayAlignment alignment)
    {
        Transform target = alignment == HolidayAlignment.Top 
            ? topHolidaysContainer 
            : bottomHolidaysContainer;

        if (target == null)
        {
            Logger.LogWarning($"Container for '{alignment}' is not assigned. Falling back to Top container.");

            return topHolidaysContainer != null 
                ? topHolidaysContainer 
                : bottomHolidaysContainer;
        }

        return target;
    }
}
