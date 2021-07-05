using RachelBot.Modules.CYOA.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RachelBot.Modules.CYOA.Handlers
{
    public class Pages
    {
        public static Page GetPage(Adventure adventure, int id)
        {
            return adventure.Pages.SingleOrDefault(p => p.Id == id);
        }

        public static Page GetPage(Adventure adventure, string code)
        {
            return adventure.Pages.SingleOrDefault(p => p.DevelopmentCode == code);
        }

#nullable enable
        public static Page EditPage(Adventure adventure,
                                    Page page,
                                    string newContent,
                                    string? newCode = null,
                                    string imageName = "",
                                    bool randomChoices = false,
                                    string randomChoiceContent = "")
        {
            if (newCode is not null && page.DevelopmentCode != newCode)
            {
                page = ChangeDevelopmentCode(adventure, page, newCode);
            }
            page = ChangeContent(adventure, page, newContent);

            page.ImageName = imageName;
            page.AreChoicesRandom = randomChoices;
            page.RandomChoiceContent = randomChoiceContent;

            return page;
        }
#nullable restore

        private static Page ChangeDevelopmentCode(Adventure adventure, Page page, string newCode)
        {
            if (ValidateDevelopmentCode(adventure, newCode))
            {
                throw new ArgumentException($"Development code {newCode} is already used!");
            }

            page.DevelopmentCode = newCode;
            Adventures.Save(adventure);

            return page;
        }

        private static Page ChangeContent(Adventure adventure, Page page, string newContent)
        {
            page.Content = newContent;
            Adventures.Save(adventure);

            return page;
        }

        public static Page CreatePage(Adventure adventure,
                                      string developmentCode,
                                      string content,
                                      string imageName = "",
                                      bool randomChoices = false,
                                      string randomChoiceContent = "")
        {
            if (ValidateDevelopmentCode(adventure, developmentCode))
            {
                throw new ArgumentException($"Development code {developmentCode} is already used!");
            }

            Page page = new Page()
            {
                Id = adventure.Pages.Count > 0 ? adventure.Pages.Max(p => p.Id) + 1 : 1,
                DevelopmentCode = developmentCode,
                ImageName = imageName,
                AreChoicesRandom = randomChoices,
                RandomChoiceContent = randomChoiceContent,
                Content = content,
                Choices = new List<Choice>()
            };

            adventure.Pages.Add(page);
            Adventures.Save(adventure);

            return page;
        }

        private static bool ValidateDevelopmentCode(Adventure adventure, string developmentCode)
        {
            return adventure.Pages.SingleOrDefault(p => p.DevelopmentCode == developmentCode) is not null;
        }
    }
}
