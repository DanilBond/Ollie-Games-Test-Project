using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TimeManager
{
    private static int debugOffsetMinutes;
    
    public static DateTime GetUtcTime()
    {
        return debugOffsetMinutes == 0 
            ? DateTime.UtcNow 
            : DateTime.UtcNow.AddMinutes(debugOffsetMinutes);
    }
}
