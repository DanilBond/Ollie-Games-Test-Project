using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.CompilerServices;

public static class Logger
{
    public static void Log(string message,
        [CallerFilePath] string filePath = "",
        [CallerMemberName] string memberName = "")
    {
        string className = GetClassName(filePath);
        Debug.Log($"[{className}] {message}");
    }

    public static void LogWarning(string message,
        [CallerFilePath] string filePath = "",
        [CallerMemberName] string memberName = "")
    {
        string className = GetClassName(filePath);
        Debug.LogWarning($"[{className}] {message}");
    }

    public static void LogError(string message,
        [CallerFilePath] string filePath = "",
        [CallerMemberName] string memberName = "")
    {
        string className = GetClassName(filePath);
        Debug.LogError($"[{className}] {message}");
    }

    private static string GetClassName(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            return "Unknown";

        string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
        return fileName;
    }
}
