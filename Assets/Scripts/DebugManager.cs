using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugManager : MonoBehaviour
{
    //
    //
    //
    // Сорри по красоте сделать не успеваю, по хорошему нужно разделить этот класс на View/Controller
    //
    //
    //
    
    [Header("References")]
    [SerializeField] private bool enableDebugging;
    [SerializeField] private HolidaysManager holidaysManager;
    [SerializeField] private GameObject debugCanvas;
    [SerializeField] private float updateIntervalInSeconds;
    
    [Header("UI")] 
    [SerializeField] private TextMeshProUGUI currentTimeText;
    [SerializeField] private TextMeshProUGUI activeHolidaysText;
    
    //Private props
    private float _timer;

    private void Start()
    {
        if (IsEditorOrDev())
        {
            TryGetHolidaysManager();

            if (debugCanvas == null)
            {
                Logger.LogWarning("DebugCanvas is missing.");
                return;
            }
            
            debugCanvas.SetActive(true);
        }
        else
        {
            debugCanvas.SetActive(false);
        }
    }

    private void Update()
    {
        _timer += Time.unscaledDeltaTime;
        if (_timer >= updateIntervalInSeconds)
        {
            _timer = 0f;
            RefreshUI();
        }
    }
    
    public void ChangeTimeOffsetHours(int minutes)
    {
        TimeManager.SetTimeOffset(TimeManager.GetTimeOffset() + minutes);
        RefreshUI();
    }

    public void ResetTimeOffsetHours()
    {
        TimeManager.SetTimeOffset(0);
        RefreshUI();
    }

    private void RefreshUI()
    {
        TryGetHolidaysManager();

        if (holidaysManager == null) return;
        
        SetCurrentTime(TimeManager.GetUtcTime());
        SetActiveHolidays(holidaysManager.GetActiveHolidayTypes());
    }
    
    private void SetCurrentTime(DateTime time)
    {
        if (currentTimeText != null)
            currentTimeText.text = $"UTC: {time:yyyy-MM-dd HH:mm:ss}";
    }
    
    private void SetActiveHolidays(IEnumerable<string> types)
    {
        if (activeHolidaysText == null) return;

        if (types == null)
        {
            activeHolidaysText.text = "Active: —";
            return;
        }

        var sb = new StringBuilder();
        bool any = false;
        foreach (var t in types)
        {
            if (string.IsNullOrWhiteSpace(t)) continue;
            if (any) sb.Append(", ");
            sb.Append(t);
            any = true;
        }

        activeHolidaysText.text = any ? $"Active: {sb}" : "Active: —";
    }

    private void TryGetHolidaysManager()
    {
        if (holidaysManager == null)
            holidaysManager = FindObjectOfType<HolidaysManager>();
    }

    private  bool IsEditorOrDev()
    {
        if (!enableDebugging) return false;
        
#if UNITY_EDITOR
        return true;
#else
        return Debug.isDebugBuild;
#endif
    }
}
