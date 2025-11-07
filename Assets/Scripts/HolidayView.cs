using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HolidayView : MonoBehaviour
{
    [Header("UI")] 
    [SerializeField] private TextMeshProUGUI holidayTypeText;

    public void Init(HolidayEntry entry)
    {
        if (!holidayTypeText)
        {
            Logger.LogWarning("HolidayTypeText is null!");
            return;
        }
        
        holidayTypeText.text = entry.GetHolidayType();
    }
}
