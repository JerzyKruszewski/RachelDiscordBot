using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using RachelBot.Core.Configs;

namespace RachelBot.Lang
{
    public class AlertsHandler
    {
        private readonly Dictionary<string, string> _alerts;

        public AlertsHandler(GuildConfig config)
        {
            string filePath = $"./Lang/{config.GuildLanguageIso.ToLower()}.json";
            string json;
            try
            {
                json = File.ReadAllText(filePath);
            }
            catch (System.Exception)
            {
                json = File.ReadAllText("./Lang/en.json");
            }
            dynamic data = JsonConvert.DeserializeObject<dynamic>(json);
            _alerts = data.ToObject<Dictionary<string, string>>();
        }

        public string GetAlert(string key)
        {
            key = key.ToUpper();

            return (_alerts.ContainsKey(key)) ?
                _alerts[key] :
                "Error - does not contains key to alert. Please report this to creator.";
        }

        public string GetFormattedAlert(string key, params object[] parameter)
        {
            key = key.ToUpper();

            return (_alerts.ContainsKey(key)) ?
                string.Format(_alerts[key], parameter) :
                "Error - does not contains key to alert. Please report this to creator.";
        }
    }
}
