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
    public static class HaasActionManager
    {

        public static string BaseBotTemplateGuid = "";

        public static bool CheckHaasConnection()
        {
            HaasonlineClient haasonlineClient = new HaasonlineClient(ConfigManager.mainConfig.IPAddress, ConfigManager.mainConfig.Port, ConfigManager.mainConfig.Secret);

            var accounts = haasonlineClient.AccountDataApi.GetEnabledAccounts();

            if (accounts.Result.ErrorCode == Haasonline.LocalApi.CSharp.Enums.EnumErrorCode.Success)
            {
                if (HaasActionManager.GetAccountGUIDS().Count > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public static List<Tuple<string, string>> GetAccountGUIDS()
        {

            List<Tuple<string, string>> results = new List<Tuple<string, string>>();


            HaasonlineClient haasonlineClient = new HaasonlineClient(ConfigManager.mainConfig.IPAddress, ConfigManager.mainConfig.Port, ConfigManager.mainConfig.Secret);

            var accountGUIDTasks = Task.Run(async () => await haasonlineClient.AccountDataApi.GetEnabledAccounts());

            accountGUIDTasks.Wait();

            // Quick hacky to get a key
            foreach (string x in accountGUIDTasks.Result.Result.Keys)
            {
                results.Add(new Tuple<string, string>(accountGUIDTasks.Result.Result[x], x));
            }

            return results;

        }

        public static List<Tuple<string, string>> GetMarkets()
        {

            List<Tuple<string, string>> results = new List<Tuple<string, string>>();

            if (HaasActionManager.CheckHaasConnection())
            {
                try
                {
                    HaasonlineClient haasonlineClient = new HaasonlineClient(ConfigManager.mainConfig.IPAddress, ConfigManager.mainConfig.Port, ConfigManager.mainConfig.Secret);

                    AccountInformation accountInformation = haasonlineClient.AccountDataApi.GetAccountDetails(ConfigManager.mainConfig.AccountGUID).Result.Result;

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

        public static List<BaseCustomBot> GetAllCustomBots()
        {
            if (HaasActionManager.CheckHaasConnection())
            {
                HaasonlineClient haasonlineClient = new HaasonlineClient(ConfigManager.mainConfig.IPAddress, ConfigManager.mainConfig.Port, ConfigManager.mainConfig.Secret);

                var getAllCustomBotsTask = Task.Run(async () => await haasonlineClient.CustomBotApi.GetAllBots());

                getAllCustomBotsTask.Wait();
                
                return getAllCustomBotsTask.Result.Result;
            }
            else
            {
                return null;
            }
        }

        public static List<BaseCustomBot> GetAllCustomBotsWithPrefix(string prefix)
        {
            string[] accountGuidSplit = ConfigManager.mainConfig.AccountGUID.Split('-');

            string botNamePrefixToMatch = "BS-" + accountGuidSplit[0];

            List<BaseCustomBot> results = new List<BaseCustomBot>();

            if (HaasActionManager.CheckHaasConnection())
            {
                // Find active bot markets
                foreach (var bot in HaasActionManager.GetAllCustomBots())
                {
                    if (bot.Name.StartsWith(botNamePrefixToMatch))
                    {
                        results.Add(bot);
                    }
                }
            }

            return results;
        }

        public static AccountInformation GetAccountInformation()
        {


            if (HaasActionManager.CheckHaasConnection())
            {
                HaasonlineClient haasonlineClient = new HaasonlineClient(ConfigManager.mainConfig.IPAddress, ConfigManager.mainConfig.Port, ConfigManager.mainConfig.Secret);

                var accountInformationTask = Task.Run(async () => await haasonlineClient.AccountDataApi.GetAccountDetails(ConfigManager.mainConfig.AccountGUID));

                accountInformationTask.Wait();

                return accountInformationTask.Result.Result;
            }
            else
            {
                return null;
            }

        }

        public static BaseCustomBot GetCustomBotByName(string botName)
        {

            List<BaseCustomBot> results = new List<BaseCustomBot>();

            if (HaasActionManager.CheckHaasConnection())
            {
                // Find active bot markets
                foreach (var bot in HaasActionManager.GetAllCustomBots())
                {
                    if (bot.Name.Equals(botName))
                    {
                        return bot;
                    }
                }
            }

            return null;
        }

        public static void ActivateCustomBot(string botGuid)
        {
            HaasonlineClient haasonlineClient = new HaasonlineClient(ConfigManager.mainConfig.IPAddress, ConfigManager.mainConfig.Port, ConfigManager.mainConfig.Secret);
            var task = Task.Run(async () => await haasonlineClient.CustomBotApi.ActivateBots(botGuid, false));
            task.Wait();
                        
        }

        public static void DeactivateCustomBot(string botGuid)
        {
            HaasonlineClient haasonlineClient = new HaasonlineClient(ConfigManager.mainConfig.IPAddress, ConfigManager.mainConfig.Port, ConfigManager.mainConfig.Secret);
            var task = Task.Run(async () => await haasonlineClient.CustomBotApi.DeactivateBots(botGuid, false));
            task.Wait();
        }

        public static Wallet GetWalletInformation()
        {

            if (HaasActionManager.CheckHaasConnection())
            {

                HaasonlineClient haasonlineClient = new HaasonlineClient(ConfigManager.mainConfig.IPAddress, ConfigManager.mainConfig.Port, ConfigManager.mainConfig.Secret);

                var walletTasks = Task.Run(async () => await haasonlineClient.AccountDataApi.GetWallet(ConfigManager.mainConfig.AccountGUID));

                walletTasks.Wait();

                return walletTasks.Result.Result;

            }

            return null;

        }

        public static string CreateTemplateBot()
        {

            List<Tuple<string, string>> markets = HaasActionManager.GetMarkets();

            if (HaasActionManager.CheckHaasConnection())
            {
                if (markets.Count > 0)
                {
                    HaasonlineClient haasonlineClient = new HaasonlineClient(ConfigManager.mainConfig.IPAddress, ConfigManager.mainConfig.Port, ConfigManager.mainConfig.Secret);
                    var newBot = haasonlineClient.CustomBotApi.NewBot<ScalperBot>(Haasonline.Public.LocalApi.CSharp.Enums.EnumCustomBotType.ScalperBot,
                        "BruteScalpe-Template", ConfigManager.mainConfig.AccountGUID, markets[0].Item1, markets[0].Item2, "");

                    HaasActionManager.BaseBotTemplateGuid = newBot.Result.Result.GUID;
                }
            }

            return HaasActionManager.BaseBotTemplateGuid;
        }

        public static void DeleteTemplateBot()
        {
            if (HaasActionManager.CheckHaasConnection())
            {
                HaasonlineClient haasonlineClient = new HaasonlineClient(ConfigManager.mainConfig.IPAddress, ConfigManager.mainConfig.Port, ConfigManager.mainConfig.Secret);
                haasonlineClient.CustomBotApi.RemoveBot(HaasActionManager.BaseBotTemplateGuid);
            }
        }

        public static bool GrabMarketData(string market, string maincoin)
        {
            HaasonlineClient haasonlineClient = new HaasonlineClient(ConfigManager.mainConfig.IPAddress, ConfigManager.mainConfig.Port, ConfigManager.mainConfig.Secret);

            AccountInformation accountInformation = haasonlineClient.AccountDataApi.GetAccountDetails(ConfigManager.mainConfig.AccountGUID).Result.Result;

            var keepPolling = true;
            var counter = 0;
            while (keepPolling)
            {
                
                var res = haasonlineClient.MarketDataApi.GetHistory(accountInformation.ConnectedPriceSource, market, maincoin, "", 1, ConfigManager.mainConfig.BackTestLength * 2).Result;

                if (res.ErrorCode == EnumErrorCode.Success && res.Result.Count >= ConfigManager.mainConfig.BackTestLength)
                    break;

                counter++;
                if (counter > 30)
                    return false;

                Thread.Sleep(1000);
            }

            return true;
        }

        public static BaseCustomBot PerformBackTest(string market, string maincoin, decimal targetPercentage, decimal safetyPercentage)
        {

            HaasonlineClient haasonlineClient = new HaasonlineClient(ConfigManager.mainConfig.IPAddress, ConfigManager.mainConfig.Port, ConfigManager.mainConfig.Secret);

            var task = Task.Run(async () => await haasonlineClient.CustomBotApi.SetupScalpingBot(BaseBotTemplateGuid, "BruteScalpe-Template", ConfigManager.mainConfig.AccountGUID, market,
                maincoin, "", 0, 1000, ConfigManager.mainConfig.Fee, maincoin, "LOCKEDLIMITORDERGUID", targetPercentage, safetyPercentage));

            task.Wait();
                        
            var task2 = Task.Run(async () => await haasonlineClient.CustomBotApi.BacktestBot<BaseCustomBot>(BaseBotTemplateGuid, 
                ConfigManager.mainConfig.BackTestLength, ConfigManager.mainConfig.AccountGUID, market, maincoin, ""));
            
            task2.Wait();



            return task2.Result.Result;
        }

        public static WinningTrade PerformFullTest(string primaryCurrency, string secondaryCurrency)
        {

            WinningTrade winningTrade = new WinningTrade();

            int count = 0;

            decimal currentTargetPercentage = ConfigManager.mainConfig.StartTargetPercentage;
            decimal currentSafetyPercentage = ConfigManager.mainConfig.StartSafetyPercentage;

            while (currentTargetPercentage < ConfigManager.mainConfig.EndTargetPerecentage)
            {
                while (currentSafetyPercentage < ConfigManager.mainConfig.EndSafetyPercentage)
                {
                    count++;

                    var botResults = HaasActionManager.PerformBackTest(primaryCurrency, secondaryCurrency, currentTargetPercentage, currentSafetyPercentage);

                    if (botResults.ROI > winningTrade.roi)
                    {
                        winningTrade.targetPercentage = currentTargetPercentage;
                        winningTrade.safetyPercentage = currentSafetyPercentage;
                        winningTrade.roi = botResults.ROI;
                    }

                    Thread.Sleep(ConfigManager.mainConfig.BackTestDelayInMiliseconds);

                    currentSafetyPercentage = currentSafetyPercentage + ConfigManager.mainConfig.SafetyPercentageStep;

                }

                currentSafetyPercentage = ConfigManager.mainConfig.StartSafetyPercentage;

                currentTargetPercentage = currentTargetPercentage + ConfigManager.mainConfig.TargetPercentageStep;

            }

            return winningTrade;
        }

        public static void CreatePersistentBot(string botName, string market, string maincoin, decimal targetPercentage, decimal safetyPercentage)
        {

            HaasonlineClient haasonlineClient = new HaasonlineClient(ConfigManager.mainConfig.IPAddress, ConfigManager.mainConfig.Port, ConfigManager.mainConfig.Secret);

            var createBot = Task.Run(async () => await haasonlineClient.CustomBotApi.NewBot<ScalperBot>(Haasonline.Public.LocalApi.CSharp.Enums.EnumCustomBotType.ScalperBot, 
                botName, ConfigManager.mainConfig.AccountGUID, market, maincoin, ""));

            createBot.Wait();

            var setupScalpBotComplete = haasonlineClient.CustomBotApi.SetupScalpingBot(createBot.Result.Result.GUID, botName, 
                ConfigManager.mainConfig.AccountGUID, market, maincoin,  "", 0, 1000, ConfigManager.mainConfig.Fee, maincoin,
                "LOCKEDLIMIT", targetPercentage, safetyPercentage);

            setupScalpBotComplete.Wait();

            Thread.Sleep(1000);
        }

        public static void CreateAutoPersistentBot(string botName, string market, string maincoin, decimal targetPercentage, decimal safetyPercentage)
        {

            HaasonlineClient haasonlineClient = new HaasonlineClient(ConfigManager.mainConfig.IPAddress, ConfigManager.mainConfig.Port, ConfigManager.mainConfig.Secret);

            var createBot = Task.Run(async () => await haasonlineClient.CustomBotApi.NewBot<ScalperBot>(Haasonline.Public.LocalApi.CSharp.Enums.EnumCustomBotType.ScalperBot,
                botName, ConfigManager.mainConfig.AccountGUID, market, maincoin, ""));

            createBot.Wait();

            var setupScalpBotComplete = haasonlineClient.CustomBotApi.SetupScalpingBot(createBot.Result.Result.GUID, botName,
                ConfigManager.mainConfig.AccountGUID, market, maincoin, "", 0, 1000, ConfigManager.mainConfig.Fee, maincoin,
                "LOCKEDLIMIT", targetPercentage, safetyPercentage);

            setupScalpBotComplete.Wait();

            Thread.Sleep(1000);
        }

        public static void UpdateScalperBot(string guid, string botName, string primaryCurrency, string secondaryCurrency, decimal targetPercentage, decimal safetyPercentage)
        {
            int amount = 1000;

            HaasonlineClient haasonlineClient = new HaasonlineClient(ConfigManager.mainConfig.IPAddress, ConfigManager.mainConfig.Port, ConfigManager.mainConfig.Secret);

            var task = Task.Run(async () => await haasonlineClient.CustomBotApi.SetupScalpingBot(guid, botName, ConfigManager.mainConfig.AccountGUID, primaryCurrency,
                secondaryCurrency, "", 0, amount, ConfigManager.mainConfig.Fee, secondaryCurrency, "LOCKEDLIMITORDERGUID", targetPercentage, safetyPercentage));
        }


        public static bool PerformStartup()
        {
            if (ConfigManager.LoadConfig())
            {
                return true;
            }
            else
            {
                ConfigManager.mainConfig = new HaasConfig();
                return false;
            }
        }
    }
}
