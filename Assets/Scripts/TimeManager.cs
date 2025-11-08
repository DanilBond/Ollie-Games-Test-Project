using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TimeManager
{
    private static int debugOffsetHours;
    
    public static DateTime GetUtcTime() => DateTime.UtcNow.AddHours(debugOffsetHours);
    
    public static void SetTimeOffset(int newOffsetMinutes) => debugOffsetHours = newOffsetMinutes;
    public static int GetTimeOffset() => debugOffsetHours;
}
