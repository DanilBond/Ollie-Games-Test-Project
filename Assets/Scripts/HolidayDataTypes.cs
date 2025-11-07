using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using UnityEngine;

public enum HolidayAlignment
{
    Top,
    Bottom
}

[System.Serializable]
public class HolidayEntry
{
    [JsonProperty] private string startDate;
    [JsonProperty] private string endDate;
    [JsonProperty] private string holidayType;
    
    public string GetHolidayType() => holidayType;
    public DateTime GetStartDate => DateTime.Parse(startDate, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
    public DateTime GetEndDate => DateTime.Parse(endDate, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

    public bool IsValid()
    {
        var styles = DateTimeStyles.RoundtripKind;
        
        if (!DateTimeOffset.TryParse(startDate, CultureInfo.InvariantCulture, styles, out var start)) return false;
        if (!DateTimeOffset.TryParse(endDate, CultureInfo.InvariantCulture, styles, out var end)) return false;
        if (string.IsNullOrEmpty(holidayType)) return false;
        if (end < start) return false;
        
        return true;
    }

    public bool IsInTimeRange(DateTime timeRange)
    {
        return timeRange >= GetStartDate && timeRange <= GetEndDate;
    }
}

[Serializable]
public class HolidaysWrapper
{
    public List<HolidayEntry> holidaySchedule = new List<HolidayEntry>();

    public bool IsValid() => holidaySchedule != null && holidaySchedule.Count > 0;
}