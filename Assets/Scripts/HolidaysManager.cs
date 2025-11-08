using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class HolidaysManager : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private string configPath;
    [SerializeField, Tooltip("May be more expensive")] private bool fetchConfigOnUpdate;
    [SerializeField] private float updateIntervalInSeconds;
    [SerializeField] private HolidayItemData[] allHolidays;

    [Header("UI")] 
    [SerializeField] private Transform topHolidaysContainer;
    [SerializeField] private Transform bottomHolidaysContainer;
    
    //Private props
    private readonly Dictionary<string, HolidayEntry> _holidaysScheduleByType =
        new Dictionary<string, HolidayEntry>(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, HolidayView> _holidaysSpawnedByType =
        new Dictionary<string, HolidayView>(StringComparer.OrdinalIgnoreCase);

    private float _holidaysCheckTimer;
    
    //References
    private HolidayConfigLoader _configLoader;

    public void Start()
    {
        Init();
    }

    private void Update()
    {
        _holidaysCheckTimer += Time.unscaledDeltaTime;
        if (_holidaysCheckTimer >= updateIntervalInSeconds)
        {
            _holidaysCheckTimer = 0f;
            ReconcileHolidays();
        }
    }
    
    private void OnDestroy()
    {
        CleanupSpawned();
    }
    
    private void Init()
    {
        BuildConfigData();
        ReconcileHolidays();
    }
    
    private void BuildConfigData()
    {
        //~0.04 ms
        
        _configLoader ??= new HolidayConfigLoader();
        HolidaysWrapper config = _configLoader.LoadConfig(configPath);
        if (!config.IsValid()) return;
        
        //Making all holidays unique by adding them into dict, and it will be more performant if we need to get holiday my type
        _holidaysScheduleByType.Clear();
        foreach (HolidayEntry entry in config.holidaySchedule)
        {
            if (entry == null) continue;
            _holidaysScheduleByType.TryAdd(entry.GetHolidayType(), entry);
        }
    }

    private void ReconcileHolidays()
    {
        ////////////////////////////////////////////////////////////////////////////////////////////
        // The new method doesn't recreate the entire UI, but only adds/removes changed elements.
        // This reduces the load from ~0.4 ms to ~0.013 ms per check.
        ////////////////////////////////////////////////////////////////////////////////////////////
        
        if (fetchConfigOnUpdate)
            BuildConfigData();
        
        // 1. Get all active holidays type at the moment
        var activeTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var (holidayType, entry) in _holidaysScheduleByType)
        {
            if (entry == null) continue;
            if (!entry.IsInTimeRange(TimeManager.GetUtcTime())) continue;
        
            // Check item data
            HolidayItemData holidayItemData = GetHolidayItemData(holidayType);
            if (holidayItemData == null || !holidayItemData.IsValid()) continue;
            
            activeTypes.Add(holidayType);
        }
        
        // 2. Create what should be but is missing
        foreach (string holidayType in activeTypes)
        {
            if (_holidaysSpawnedByType.ContainsKey(holidayType))
                continue;
        
            HolidayEntry entry = _holidaysScheduleByType[holidayType];
            HolidayItemData itemData = GetHolidayItemData(holidayType);
        
            HolidayView view = Instantiate(itemData.GetHolidayViewPrefab,
                GetContainerForHoliday(itemData.GetHolidayAlignment));
            view.Init(entry);
        
            _holidaysSpawnedByType[holidayType] = view;
        }
        
        // 3) Delete those that are no longer active
        // We collect a list of items to remove so as not to change the collection during iteration.
        var toRemove = new List<string>();
        foreach (var kv in _holidaysSpawnedByType)
        {
            string holidayType = kv.Key;
            if (!activeTypes.Contains(holidayType))
                toRemove.Add(holidayType);
        }
        foreach (string holidayType in toRemove)
        {
            if (_holidaysSpawnedByType.TryGetValue(holidayType, out var view) && view != null)
                Destroy(view.gameObject);
            _holidaysSpawnedByType.Remove(holidayType);
        }
        
        // 4) Just in case: if the placement parameters of the active type have changed, you can rebind it
        // This is a rare case, so we'll keep it simple and readable.
        foreach (string holidayType in activeTypes)
        {
            if (!_holidaysSpawnedByType.TryGetValue(holidayType, out var view) || view == null) continue;
        
            var itemData = GetHolidayItemData(holidayType);
            var target = GetContainerForHoliday(itemData.GetHolidayAlignment);
            if (view.transform.parent != target)
            {
                view.transform.SetParent(target, worldPositionStays: false);
            }
        }
    }

    private void CleanupSpawned()
    {
        foreach (var view in _holidaysSpawnedByType.Values)
        {
            if (view != null)
                Destroy(view.gameObject);
        }

        _holidaysSpawnedByType.Clear();
        _holidaysScheduleByType.Clear();
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
    
    public IReadOnlyCollection<string> GetActiveHolidayTypes() => /*Copy to new list to prevent modifying original*/ new List<string>(_holidaysSpawnedByType.Keys);
}
