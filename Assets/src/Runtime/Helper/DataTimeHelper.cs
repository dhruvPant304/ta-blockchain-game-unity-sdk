using System;

namespace TA.Helpers{
public static class DataTimeHelper {
    public static string GetCurrentTimeInIsoFormat(){
        return DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
    }
}}
