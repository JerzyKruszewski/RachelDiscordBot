using RachelBot.Modules.CYOA.Objects;
using System;
using System.Text.RegularExpressions;

namespace RachelBot.Modules.CYOA
{
    internal class Utils
    {
        public static string PerParsePageContent(string pageContent, Adventure adventure, PlayedStory story)
        {
            pageContent = ParseMCGenderSpecificWords(pageContent, adventure.IsMCFemale ?? story.IsMCFemale); //Default male

            pageContent = ParseMCName(pageContent, string.IsNullOrWhiteSpace(adventure.MCName) ? story.MCName : adventure.MCName);

            return pageContent;
        }

        private static string ParseMCName(string content, string name)
        {
            return content.Replace("%PlayerName%", name);
        }

        private static string ParseMCGenderSpecificWords(string content, bool isFemale)
        {
            foreach (Match match in Regex.Matches(content, @"\%Gender\(([^)]*)\)\%"))
            {
                GroupCollection keyValuePairs = match.Groups;
                string[] genderSpecific = keyValuePairs[1].Value.Replace(" ", "").Split(',');
                content = content.Replace(keyValuePairs[0].Value, isFemale ? genderSpecific[1] : genderSpecific[0]);
            }

            return content;
        }
    }
}
