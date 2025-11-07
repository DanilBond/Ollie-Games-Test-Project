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
    private HolidaysWrapper _config;
    
    //References
    private HolidayConfigLoader _configLoader;

    public int testTimeOffset;

    private void Update()
    {
        TimeManager.SetTimeOffset(testTimeOffset);
        //CreateHolidays();
    }

    private void Awake()
    {
        CreateAllSystems();
    }

    public void Start()
    {
        Init();
    }

    private void CreateAllSystems()
    {
        _configLoader = new HolidayConfigLoader();
    }

    private void Init()
    {
        _config = _configLoader.LoadConfig(configPath);
        
        CreateHolidays();
    }

    private void CreateHolidays()
    {
        if (fetchConfigOnUpdate) _config = _configLoader.LoadConfig(configPath);
        
        //Remove existing
        if (topHolidaysContainer)
            foreach (Transform item in topHolidaysContainer) Destroy(item.gameObject);
        if (bottomHolidaysContainer)
            foreach (Transform item in bottomHolidaysContainer) Destroy(item.gameObject);
        
        //Spawn new
        foreach (HolidayEntry entry in _config.holidaySchedule)
        {
            HolidayItemData holidayItemData = GetHolidayItemData(entry.GetHolidayType());
            if (!holidayItemData && !holidayItemData.IsValid()) continue;
            if (!entry.IsInTimeRange(TimeManager.GetUtcTime())) continue;

            HolidayView HolidayViewInstance = Instantiate(holidayItemData.GetHolidayViewPrefab,
                GetContainerForHoliday(holidayItemData.GetHolidayAlignment));
            
            HolidayViewInstance.Init(entry);
        }
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
