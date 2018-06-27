using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Haasonline.Public.LocalApi.CSharp.DataObjects.CustomBots;

namespace BruteScalp
{
    public static class AutoScalpeManager
    {

        public static List<Tuple<string, string>> GetMarketsPrioritized()
        {
            List<Tuple<string, string>> marketsPrioritized = new List<Tuple<string, string>>();

            var marketsToTest = ConfigManager.mainConfig.MarketsToTest;

            string[] accountGuidSplit = ConfigManager.mainConfig.AccountGUID.Split('-');

            string botNamePrefixToMatch = "BS-" + accountGuidSplit[0];

            var customBots = HaasActionManager.GetAllCustomBotsWithPrefix(botNamePrefixToMatch);

            // Find active bot markets
            foreach (var bot in customBots)
            {
                marketsPrioritized.Add(new Tuple<string, string>(bot.PriceMarket.PrimaryCurrency, bot.PriceMarket.SecondaryCurrency));
            }

            foreach(var market in marketsToTest)
            {
                bool matchFound = false;

                foreach (var pMarket in marketsPrioritized)
                {
                    if (market.Equals(pMarket))
                    {
                        matchFound = true;
                    }
                }

                if(!matchFound)
                {
                    marketsPrioritized.Add(market);
                }
            }

            return marketsPrioritized;
        }

        public static BackTestData GetHistoryForMarket(string accountGuid, List<BackTestData> backTestHistory, Tuple<string,string> market)
        {
            foreach(var bth in backTestHistory)
            {
                if( bth.AccountGUID.Equals(accountGuid) && bth.PrimaryCurrency == market.Item1 && bth.SecondayCurrency == market.Item2)
                {
                    return bth;
                }
            }

            return null;
        }
    }
}
