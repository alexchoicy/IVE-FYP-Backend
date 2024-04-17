using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace api.utils
{
    public class TimeLoader
    {
        public static string folderPath = "../Config/TimeConfig.json";

        public static DateTime GetTime()
        {
            if (File.Exists(folderPath))
            {
                string json = File.ReadAllText(folderPath);
                TimeConfig timeConfig = JsonConvert.DeserializeObject<TimeConfig>(json);
                if (timeConfig.useCustomTime)
                {
                    return DateTime.Parse(timeConfig.customDateTime);
                }
                else
                {
                    return DateTime.Now;
                }
            }
            else
            {
                return DateTime.Now;
            }
        }
    }

    public class TimeConfig
    {
        public Boolean useCustomTime { get; set; }
        public string customDateTime { get; set; }
    }
}