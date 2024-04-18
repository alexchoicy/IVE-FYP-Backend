using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace api.utils
{
    public class TimeLoader
    {
        public static string GetTimeConfigPath()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var timeConfigPath = Path.Combine(currentDirectory, "Config", "TimeConfig.json");

            return timeConfigPath;
        }
        public static DateTime GetTime()
        {
            string folderPath = GetTimeConfigPath();
            if (!File.Exists(folderPath))
            {
                Console.WriteLine("Time config file not found");
                return DateTime.Now;
            }

            string json = File.ReadAllText(folderPath);
            Console.WriteLine("Time config: " + json);
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