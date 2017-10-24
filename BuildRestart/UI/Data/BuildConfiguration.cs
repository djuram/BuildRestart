using System;
using System.Collections.Generic;
using System.Linq;

namespace BuildRestart.UI.Data
{
    [Serializable]
    public class BuildConfiguration
    {
        public string SolutionName { get; set; }

        public string AppPoolName { get; set; }

        public bool RestartAppPool { get; set; }

        public List<string> ParseAppPoolNames()
        {
            return string.IsNullOrEmpty(AppPoolName) ? new List<string>() : Parse(AppPoolName);
        }

        private static List<string> Parse(string property)
        {
            return property.Split(
                new[] { ";" },
                StringSplitOptions.RemoveEmptyEntries).
                Select(s => s.Trim()).ToList();
        }
    }
}
