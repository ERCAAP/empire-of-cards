using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay
{
    public static class SeasonSystem
    {
        public static float GetMultiplier(SeasonType season, SectorType sector)
        {
            if (sector == SectorType.Restaurant)
            {
                switch (season)
                {
                    case SeasonType.Spring:  return Constants.SEASON_SPRING;
                    case SeasonType.Summer:  return Constants.SEASON_SUMMER;
                    case SeasonType.Autumn:  return Constants.SEASON_AUTUMN;
                    case SeasonType.Winter:  return Constants.SEASON_WINTER;
                    case SeasonType.Ramadan: return Constants.SEASON_RAMADAN;
                }
            }

            return 1f;
        }

        public static bool IsCardAvailable(CardData card, SeasonType currentSeason)
        {
            if (!card.isSeasonal)
                return true;

            string[] tags = card.tags;
            if (tags == null || tags.Length == 0)
                return true;

            for (int i = 0; i < tags.Length; i++)
            {
                switch (tags[i])
                {
                    case "summer_only":
                        return currentSeason == SeasonType.Summer;
                    case "winter_only":
                        return currentSeason == SeasonType.Winter;
                    case "ramadan_only":
                        return currentSeason == SeasonType.Ramadan;
                    case "spring_only":
                        return currentSeason == SeasonType.Spring;
                    case "autumn_only":
                        return currentSeason == SeasonType.Autumn;
                }
            }

            return true;
        }
    }
}
