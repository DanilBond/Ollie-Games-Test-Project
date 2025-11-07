using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HolidayItemData", menuName = "Data/Holiday Item Data")]
public class HolidayItemData : ScriptableObject
{
    [SerializeField] private string holidayType;
    [SerializeField] private HolidayView holidayViewPrefab;
    [SerializeField] private HolidayAlignment holidayAlignment;
    
    public string GetHolidayType => holidayType;
    public HolidayView GetHolidayViewPrefab => holidayViewPrefab;
    public HolidayAlignment GetHolidayAlignment => holidayAlignment;

    public bool IsValid()
    {
        if (string.IsNullOrEmpty(holidayType))
        {
            Logger.LogWarning("Invalid Holiday Type");
            return false;
        }
        if (!holidayViewPrefab)
        {
            Logger.LogWarning("Invalid Holiday Prefab");
            return false;
        }

        return true;
    }
}