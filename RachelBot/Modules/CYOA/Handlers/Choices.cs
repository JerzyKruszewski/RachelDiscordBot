using RachelBot.Modules.CYOA.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RachelBot.Modules.CYOA.Handlers
{
    public class Choices
    {
        public static Choice GetChoice(Page page, int id)
        {
            return page.Choices.SingleOrDefault(c => c.Id == id);
        }

        public static Choice GetRandomChoice(Page page)
        {
            Random random = new Random();
            List<double> cumulativeDistribution = new List<double>()
            {
                0.0
            };

            double sumOfWeights = page.Choices.Sum(c => c.Weight);
            double randomNumber = random.NextDouble();

            for (int i = 1; i < page.Choices.Count; i++)
            {
                cumulativeDistribution.Add(cumulativeDistribution[i - 1] + page.Choices[i - 1].Weight / sumOfWeights);
            }

            for (int i = 0; i < cumulativeDistribution.Count - 1; i++)
            {
                if (randomNumber > cumulativeDistribution[i] && randomNumber <= cumulativeDistribution[i + 1])
                {
                    return page.Choices[i];
                }
            }

            return page.Choices[^1];
        }

        public static Choice CreateChoice(Adventure adventure, Page page, string content, int pointsToPageWithId, int weight = 10)
        {
            if (adventure.Pages.SingleOrDefault(p => p.Id == pointsToPageWithId) == null)
            {
                throw new ArgumentNullException($"Couldn't find page with id {pointsToPageWithId}.");
            }

            Choice choice = new Choice()
            {
                Id = page.Choices.Count > 0 ? page.Choices.Max(c => c.Id) + 1 : 1,
                Content = content,
                Weight = weight,
                PointsToPageWithId = pointsToPageWithId
            };

            page.Choices.Add(choice);
            Adventures.Save(adventure);

            return choice;
        }

#nullable enable
        public static Choice EditChoice(Adventure adventure, Page page, Choice choice, string newContent, int? newPointsToPageWithId = null, int weight = 10)
        {
            choice.Content = newContent;

            if (newPointsToPageWithId.HasValue)
            {
                Page newPointPage = adventure.Pages.SingleOrDefault(p => p.Id == newPointsToPageWithId.Value) ??
                     throw new ArgumentNullException($"Couldn't find page with id {newPointsToPageWithId.Value}");

                if (newPointPage == page)
                {
                    throw new ArgumentException($"Choice couldn't point to it's own page");
                }

                choice.PointsToPageWithId = newPointsToPageWithId.Value;
            }

            choice.Weight = weight;

            Adventures.Save(adventure);

            return choice;
        }
#nullable restore
    }
}
