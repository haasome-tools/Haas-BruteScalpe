using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Haasonline.Public.LocalApi.CSharp.DataObjects.CustomBots;
using Newtonsoft.Json;

namespace BruteScalp
{
    public static class BackTestHistoryManager
    {
        private static BackTestHistory backTestHistory;

        public static string DefaultBackTestHistoryFileName = "Backtest.history";

        public static bool LoadBacktestHistory()
        {
            if (File.Exists(DefaultBackTestHistoryFileName))
            {
                BackTestHistoryManager.backTestHistory = Newtonsoft.Json.JsonConvert.DeserializeObject<BackTestHistory>(File.ReadAllText(DefaultBackTestHistoryFileName));
                return true;
            }
            else
            {

                return false;
            }

        }

        public static bool SaveBackTestHistory()
        {
            try
            {
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(BackTestHistoryManager.backTestHistory, Formatting.Indented);

                File.WriteAllText(DefaultBackTestHistoryFileName, json);

                return true;
            }
            catch
            {

                return false;
            }
        }

        public static void AddHistoryEntry(string accountGuid, BaseCustomBot baseCustomBot)
        {
            BackTestData backTestData = new BackTestData();

            backTestData.AccountGUID = accountGuid;
            backTestData.Exchange = baseCustomBot.PriceMarket.PriceSource;
            backTestData.PrimaryCurrency = baseCustomBot.PriceMarket.PrimaryCurrency;
            backTestData.SecondayCurrency = baseCustomBot.PriceMarket.SecondaryCurrency;
            backTestData.WinningROI = baseCustomBot.ROI;

            BackTestHistoryManager.backTestHistory.history.Add(backTestData);
        }

        public static void UpdateHistoryEntry(string accountGuid, BaseCustomBot baseCustomBot)
        {

            foreach(var backTestEntry in BackTestHistoryManager.backTestHistory.history)
            {
                if(backTestEntry.AccountGUID.Equals(accountGuid) && 
                    backTestEntry.PrimaryCurrency == baseCustomBot.PriceMarket.PrimaryCurrency && 
                    backTestEntry.SecondayCurrency == baseCustomBot.PriceMarket.SecondaryCurrency)
                {
                    BackTestHistoryManager.RemoveHistoryEntry(backTestEntry);
                    BackTestHistoryManager.AddHistoryEntry(accountGuid, baseCustomBot);
                    return;
                }
            }

            BackTestHistoryManager.AddHistoryEntry(accountGuid, baseCustomBot);

        }

        public static void RemoveHistoryEntry(BackTestData entry)
        {

            try
            { 
                    backTestHistory.history.Remove(entry);
            }
            catch
            {

           
            }
        }

        public static BackTestHistory GetAllHistory()
        {
            return BackTestHistoryManager.backTestHistory;
        }

        public static List<BackTestData> GetHistoryForAccount(string accountGUID)
        {
            List<BackTestData> results = new List<BackTestData>();

            foreach (var backTestEntry in BackTestHistoryManager.backTestHistory.history)
            {
                if(backTestEntry.AccountGUID.Equals(accountGUID))
                {
                    results.Add(backTestEntry);
                }
            }

            return results;

        }

        public static BackTestData GetHistoryForBot(BaseCustomBot baseCustomBot)
        {
            foreach (var backTestEntry in BackTestHistoryManager.backTestHistory.history)
            {
                if (backTestEntry.AccountGUID.Equals(ConfigManager.mainConfig.AccountGUID) &&
                    backTestEntry.PrimaryCurrency == baseCustomBot.PriceMarket.PrimaryCurrency &&
                    backTestEntry.SecondayCurrency == baseCustomBot.PriceMarket.SecondaryCurrency)
                {
                    return backTestEntry;
                }
            }

            return null;
        }

        public static BackTestData GetHistoryForMarket(string accountGuid, List<BackTestData> backTestHistory, Tuple<string, string> market)
        {
            foreach (var bth in backTestHistory)
            {
                if (bth.AccountGUID.Equals(accountGuid) && bth.PrimaryCurrency == market.Item1 && bth.SecondayCurrency == market.Item2)
                {
                    return bth;
                }
            }

            return null;
        }

        public static bool PerformStartup()
        {
            if (BackTestHistoryManager.LoadBacktestHistory())
            {
                return true;
            }
            else
            {
                BackTestHistoryManager.backTestHistory = new BackTestHistory();
                return false;
            }
        }
    }
}
