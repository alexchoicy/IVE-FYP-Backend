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
            if (!File.Exists(folderPath))
            {
                return DateTime.Now;
            }

            string json = File.ReadAllText(folderPath);
            TimeConfig timeConfig = JsonConvert.DeserializeObject<TimeConfig>(json);
            if (timeConfig.useCustomTime)
            {
                return DateTime.Parse(timeConfig.customDateTime);
            }
            return DateTime.Now;

        }
    }

    public class TimeConfig
    {
        public bool useCustomTime { get; set; }
        public string customDateTime { get; set; }
    }
}