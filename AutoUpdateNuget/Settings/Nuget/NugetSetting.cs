using System;
using Newtonsoft.Json;

namespace VSIXProject1.Settings.Nuget
{
    [Serializable]
    public struct NugetSetting
    {
        public string Server { get; set; }
        public string ApiKey { get; set; }
        [JsonIgnore]
        public string Package { get; set; }
        [JsonIgnore]
        public bool IncrementarVersion { get; set; }
    }
}
