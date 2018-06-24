using Haasonline.Public.LocalApi.CSharp;
using Haasonline.Public.LocalApi.CSharp.DataObjects.AccountData;
using Haasonline.Public.LocalApi.CSharp.DataObjects.CustomBots;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Haasonline.LocalApi.CSharp.Enums;

namespace BruteScalp
{
    // Meat of the application
    // Performs all real actions
    public static class ActionManager
    {
        public static HaasConfig mainConfig;

        public static string DefaultConfigName = "HaasTool.config";

        public static string BaseBotTemplateGuid = "";

        public static bool LoadConfig()
        {

            if (File.Exists(DefaultConfigName))
            {
                ActionManager.mainConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<HaasConfig>(File.ReadAllText(DefaultConfigName));
                return true;
            }
            else
            {
                return false;
            }

        }

        public static bool LoadConfig(string fileName)
        {

            if (File.Exists(fileName))
            {
                ActionManager.mainConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<HaasConfig>(File.ReadAllText(fileName));
                return true;
            }
            else
            {
                return false;
            }

        }

        public static bool SaveConfig()
        {
            try
            {
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(ActionManager.mainConfig);

                File.WriteAllText(DefaultConfigName, json);

                return true;
            }
            catch
            {

                return false;
            }

        }
 
        public static bool SaveConfig(string fileName)
        {
            try
            {
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(ActionManager.mainConfig);

                File.WriteAllText(fileName, json);

                return true;
            }
            catch
            {
                return false;
            }

        }

        public static void ShowConfig()
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(ActionManager.mainConfig));
        }

        public static void SetConfigIpAddress(string ipAddress)
        {
            ActionManager.mainConfig.IPAddress = ipAddress;
        }

        public static void SetConfigPort(int port)
        {
            ActionManager.mainConfig.Port = port;
        }

        public static void SetConfigSecret(string secret)
        {
            ActionManager.mainConfig.Secret = secret;
        }

        public static void SetConfigAccountGuid(string accountGUID)
        {
            ActionManager.mainConfig.AccountGUID = accountGUID;
        }

        public static void SetConfigKeepThreshold(decimal threshold)
        {
            ActionManager.mainConfig.KeepThreshold = threshold;
        }

        public static void SetConfigBacktestDelay(int delay)
        {
            ActionManager.mainConfig.BackTestDelayInMiliseconds = delay;
        }

        public static void SetConfigFee(decimal fee)
        {
            ActionManager.mainConfig.Fee = fee;
        }

        public static void SetConfigBackTestLength(int length)
        {
            ActionManager.mainConfig.BackTestLength = length;
        }

        public static void SetConfigWriteResultsToFile(bool writeToFile)
        {
            ActionManager.mainConfig.WriteResultsToFile = writeToFile;
        }

        public static void SetConfigPersistBots(bool persist)
        {
            ActionManager.mainConfig.PersistBots = persist;
        }

        public static void SetConfigRetryCount(int count)
        {
            ActionManager.mainConfig.RetryCount = count;   
        }

        public static void SetConfigStartTargetPercentage(decimal percentage)
        {
            ActionManager.mainConfig.StartTargetPercentage = percentage;
        }

        public static void SetConfigEndTargetPercentage(decimal percentage)
        {
            ActionManager.mainConfig.EndTargetPerecentage = percentage;
        }

        public static void SetConfigTargetPercentageStep(decimal percentage)
        {
            ActionManager.mainConfig.TargetPercentageStep = percentage;
        }

        public static void SetConfigStartSafetyPercentage(decimal percentage)
        {
            ActionManager.mainConfig.StartSafetyPercentage = percentage;
        }

        public static void SetConfigEndSafetyPercentage(decimal percentage)
        {
            ActionManager.mainConfig.EndSafetyPercentage = percentage;
        }

        public static void SetConfigSafetyPercentageStep(decimal percentage)
        {
            ActionManager.mainConfig.SafetyPercentageStep = percentage;
        }

        public static void AddMarketToTest(string market, string maincoin)
        {
            ActionManager.mainConfig.MarketsToTest.Add(new Tuple<string,string>(market, maincoin));
        }

        public static void RemoveMarketToTest(string market, string maincoin)
        {
            ActionManager.mainConfig.MarketsToTest.Remove(new Tuple<string, string>(market, maincoin));
        }

        public static bool CheckHaasConnection()
        {
            HaasonlineClient haasonlineClient = new HaasonlineClient(ActionManager.mainConfig.IPAddress, ActionManager.mainConfig.Port, ActionManager.mainConfig.Secret);

            var accounts = haasonlineClient.AccountDataApi.GetEnabledAccounts();

            if (accounts.Result.ErrorCode == Haasonline.LocalApi.CSharp.Enums.EnumErrorCode.Success)
            {
                if (ActionManager.GetAccountGUIDS().Count > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public static List<Tuple<string,string>> GetAccountGUIDS()
        {

            List<Tuple<string, string>> results = new List<Tuple<string, string>>();


            HaasonlineClient haasonlineClient = new HaasonlineClient(ActionManager.mainConfig.IPAddress, ActionManager.mainConfig.Port, ActionManager.mainConfig.Secret);

            var accounts = haasonlineClient.AccountDataApi.GetEnabledAccounts();

            // Quick hacky to get a key
            foreach (string x in accounts.Result.Result.Keys)
            {
                results.Add(new Tuple<string, string>(accounts.Result.Result[x], x));
            }
            
            return results;

        }

        public static List<Tuple<string, string>> GetMarkets()
        {

            List<Tuple<string, string>> results = new List<Tuple<string, string>>();

            if (ActionManager.CheckHaasConnection())
            {
                try
                {
                    HaasonlineClient haasonlineClient = new HaasonlineClient(ActionManager.mainConfig.IPAddress, ActionManager.mainConfig.Port, ActionManager.mainConfig.Secret);

                    AccountInformation accountInformation = haasonlineClient.AccountDataApi.GetAccountDetails(mainConfig.AccountGUID).Result.Result;

                    var markets = haasonlineClient.MarketDataApi.GetPriceMarkets(accountInformation.ConnectedPriceSource);

                    foreach (var market in markets.Result.Result)
                    {
                        results.Add(new Tuple<string,string>(market.PrimaryCurrency, market.SecondaryCurrency));
                    }
                }
                catch
                {
                    return results;
                }
            }

            return results;
        }

        public static string CreateTemplateBot()
        {

            List<Tuple<string, string>> markets = ActionManager.GetMarkets();

            if (ActionManager.CheckHaasConnection())
            {
                if (markets.Count > 0)
                {
                    HaasonlineClient haasonlineClient = new HaasonlineClient(ActionManager.mainConfig.IPAddress, ActionManager.mainConfig.Port, ActionManager.mainConfig.Secret);
                    var newBot = haasonlineClient.CustomBotApi.NewBot<ScalperBot>(Haasonline.Public.LocalApi.CSharp.Enums.EnumCustomBotType.ScalperBot,
                        "BruteScalpe-Template", mainConfig.AccountGUID, markets[0].Item1, markets[0].Item2, "");

                    ActionManager.BaseBotTemplateGuid = newBot.Result.Result.GUID;
                }
            }

            return ActionManager.BaseBotTemplateGuid;
        }

        public static void DeleteTemplateBot()
        {
            if (ActionManager.CheckHaasConnection())
            {
                HaasonlineClient haasonlineClient = new HaasonlineClient(ActionManager.mainConfig.IPAddress, ActionManager.mainConfig.Port, ActionManager.mainConfig.Secret);
                haasonlineClient.CustomBotApi.RemoveBot(ActionManager.BaseBotTemplateGuid);
            }
        }

        public static bool GrabMarketData(string market, string maincoin)
        {
            HaasonlineClient haasonlineClient = new HaasonlineClient(ActionManager.mainConfig.IPAddress, ActionManager.mainConfig.Port, ActionManager.mainConfig.Secret);

            AccountInformation accountInformation = haasonlineClient.AccountDataApi.GetAccountDetails(mainConfig.AccountGUID).Result.Result;

            var keepPolling = true;
            var counter = 0;
            while (keepPolling)
            {
                
                var res = haasonlineClient.MarketDataApi.GetHistory(accountInformation.ConnectedPriceSource, market, maincoin, "", 1, mainConfig.BackTestLength * 2).Result;
                if (res.ErrorCode == EnumErrorCode.Success && res.Result.Count >= mainConfig.BackTestLength)
                    break;

                counter++;
                if (counter > 30)
                    return false;

                Thread.Sleep(1000 * 2);
            }

            return true;
        }

        public static BaseCustomBot PerformBackTest(string market, string maincoin, decimal targetPercentage, decimal safetyPercentage)
        {

            HaasonlineClient haasonlineClient = new HaasonlineClient(ActionManager.mainConfig.IPAddress, ActionManager.mainConfig.Port, ActionManager.mainConfig.Secret);

            var task = Task.Run(async () => await haasonlineClient.CustomBotApi.SetupScalpingBot(BaseBotTemplateGuid, "BruteScalpe-Template", ActionManager.mainConfig.AccountGUID, market,
                maincoin, "", 0, 1000, ActionManager.mainConfig.Fee, maincoin, "LOCKEDLIMITORDERGUID", targetPercentage, safetyPercentage));

            task.Wait();
                        
            var task2 = Task.Run(async () => await haasonlineClient.CustomBotApi.BacktestBot<BaseCustomBot>(BaseBotTemplateGuid, 
                ActionManager.mainConfig.BackTestLength, ActionManager.mainConfig.AccountGUID, market, maincoin, ""));
            
            task2.Wait();

            return task2.Result.Result;
        }

        public static void CreatePersistentBot(string market, string maincoin, decimal targetPercentage, decimal safetyPercentage)
        {
            string botName = "BS-" + market + ":" + maincoin;

            HaasonlineClient haasonlineClient = new HaasonlineClient(ActionManager.mainConfig.IPAddress, ActionManager.mainConfig.Port, ActionManager.mainConfig.Secret);

            var createBot = Task.Run(async () => await haasonlineClient.CustomBotApi.NewBot<ScalperBot>(Haasonline.Public.LocalApi.CSharp.Enums.EnumCustomBotType.ScalperBot, 
                botName, ActionManager.mainConfig.AccountGUID, market, maincoin, ""));

            createBot.Wait();

            var setupScalpBotComplete = haasonlineClient.CustomBotApi.SetupScalpingBot(createBot.Result.Result.GUID, botName, 
                ActionManager.mainConfig.AccountGUID, market, maincoin,  "", 0, 1000, ActionManager.mainConfig.Fee, maincoin,
                "LOCKEDLIMIT", targetPercentage, safetyPercentage);

            setupScalpBotComplete.Wait();

            Thread.Sleep(1000);
        }

        public static bool PerformStartup()
        {
            if (LoadConfig())
            {
                return true;
            }
            else
            {
                ActionManager.mainConfig = new HaasConfig();
                return false;
            }
        }

    }
}
