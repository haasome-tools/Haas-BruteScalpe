using Haasonline.Public.LocalApi.CSharp;
using Haasonline.Public.LocalApi.CSharp.DataObjects.AccountData;
using Haasonline.Public.LocalApi.CSharp.DataObjects.CustomBots;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Haasonline.Public.LocalApi.CSharp.DataObjects.MarketData;
using Haasonline.Public.LocalApi.CSharp.Enums;

namespace BruteScalp
{
    // Meat of the application
    // Performs all real actions
    public static class HaasActionManager
    {

        public static string BaseBotTemplateGuid = "";

        public static bool CheckHaasConnection()
        {
            try
            {
                HaasonlineClient haasonlineClient = new HaasonlineClient(ConfigManager.mainConfig.IPAddress, ConfigManager.mainConfig.Port, ConfigManager.mainConfig.Secret);


                var accounts = haasonlineClient.AccountDataApi.GetEnabledAccounts();

                if (accounts.Result.ErrorCode == EnumErrorCode.Success)
                {
                    if (HaasActionManager.GetAccountGUIDS().Count > 0)
                    {
                        return true;
                    }
                }
            }
            catch
            {
                // Just to supress the stack error that occurs for failed connection
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

        public static ScalperBot GetScalperBotByName(string botName)
        {
            List<BaseCustomBot> results = new List<BaseCustomBot>();

            if (HaasActionManager.CheckHaasConnection())
            {
                // Find active bot markets
                foreach (var bot in HaasActionManager.GetAllCustomBots())
                {
                    if (bot.Name.Equals(botName))
                    {
                        return HaasActionManager.RetrieveScalperObjectFromAPI(bot.GUID);
                    }
                }
            }

            return null;
        }

        public static ScalperBot RetrieveScalperObjectFromAPI(string guid)
        {
            if (HaasActionManager.CheckHaasConnection())
            {
                HaasonlineClient haasonlineClient = new HaasonlineClient(ConfigManager.mainConfig.IPAddress, ConfigManager.mainConfig.Port, ConfigManager.mainConfig.Secret);

                var getScalperBotTask = Task.Run(async () => await haasonlineClient.CustomBotApi.GetBot<ScalperBot>(guid));

                getScalperBotTask.Wait();

                return getScalperBotTask.Result.Result;
            }
            else
            {
                return null;
            }
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
                    var newBot = Task.Run(async () => await haasonlineClient.CustomBotApi.NewBot<ScalperBot>(Haasonline.Public.LocalApi.CSharp.Enums.EnumCustomBotType.ScalperBot,
                        "BruteScalpe-Template", ConfigManager.mainConfig.AccountGUID, markets[0].Item1, markets[0].Item2, ""));

                    newBot.Wait();

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
                var deleteTask = Task.Run(async () => await haasonlineClient.CustomBotApi.RemoveBot(HaasActionManager.BaseBotTemplateGuid));

                deleteTask.Wait();
            }
        }

        public static void DeleteBot(string botGuid)
        {
            if (HaasActionManager.CheckHaasConnection())
            {
                HaasonlineClient haasonlineClient = new HaasonlineClient(ConfigManager.mainConfig.IPAddress, ConfigManager.mainConfig.Port, ConfigManager.mainConfig.Secret);
                var deleteTask = Task.Run(async () => await haasonlineClient.CustomBotApi.RemoveBot(botGuid));

                deleteTask.Wait();
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

                Thread.Sleep(3000);
            }

            return true;
        }

        public static BaseCustomBot PerformBackTest(string market, string maincoin, decimal targetPercentage, decimal safetyPercentage)
        {

            HaasonlineClient haasonlineClient = new HaasonlineClient(ConfigManager.mainConfig.IPAddress, ConfigManager.mainConfig.Port, ConfigManager.mainConfig.Secret);

            var task = Task.Run(async () => await haasonlineClient.CustomBotApi.SetupScalpingBot(BaseBotTemplateGuid, "BruteScalpe-Template", ConfigManager.mainConfig.AccountGUID, market,
                maincoin, "", 0, EnumBotTradeAmount.Static, 1000, ConfigManager.mainConfig.Fee, maincoin, "LOCKEDLIMITORDERGUID", targetPercentage, safetyPercentage));

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

            int runEstimation = Convert.ToInt32(((ConfigManager.mainConfig.EndTargetPerecentage - ConfigManager.mainConfig.StartTargetPercentage) / ConfigManager.mainConfig.TargetPercentageStep) * ((ConfigManager.mainConfig.EndSafetyPercentage - ConfigManager.mainConfig.StartSafetyPercentage) / ConfigManager.mainConfig.SafetyPercentageStep));


            while (currentTargetPercentage < ConfigManager.mainConfig.EndTargetPerecentage)
            {
                while (currentSafetyPercentage < ConfigManager.mainConfig.EndSafetyPercentage)
                {
                    count++;

                    Console.Write("\r[+] Processing [{0} of {1}] - Target: {2} Safety: {3} ROI: {4}", count, runEstimation, currentTargetPercentage, currentSafetyPercentage, winningTrade.roi);


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

            Console.WriteLine();

            return winningTrade;
        }

        public static void CreatePersistentBot(string botName, string market, string maincoin, decimal targetPercentage, decimal safetyPercentage)
        {

            HaasonlineClient haasonlineClient = new HaasonlineClient(ConfigManager.mainConfig.IPAddress, ConfigManager.mainConfig.Port, ConfigManager.mainConfig.Secret);

            var createBot = Task.Run(async () => await haasonlineClient.CustomBotApi.NewBot<ScalperBot>(Haasonline.Public.LocalApi.CSharp.Enums.EnumCustomBotType.ScalperBot, 
                botName, ConfigManager.mainConfig.AccountGUID, market, maincoin, ""));

            createBot.Wait();

            var setupScalpBotComplete = haasonlineClient.CustomBotApi.SetupScalpingBot(createBot.Result.Result.GUID, botName, 
                ConfigManager.mainConfig.AccountGUID, market, maincoin,  "", 0, EnumBotTradeAmount.Static, 1000, ConfigManager.mainConfig.Fee, maincoin,
                "LOCKEDLIMITORDERGUID", targetPercentage, safetyPercentage);

            setupScalpBotComplete.Wait();

            Thread.Sleep(1000);
        }

        public static ScalperBot CreateAutoPersistentBot(string botName, string market, string maincoin, decimal amount,  decimal targetPercentage, decimal safetyPercentage)
        {

            HaasonlineClient haasonlineClient = new HaasonlineClient(ConfigManager.mainConfig.IPAddress, ConfigManager.mainConfig.Port, ConfigManager.mainConfig.Secret);

            var createBot = Task.Run(async () => await haasonlineClient.CustomBotApi.NewBot<ScalperBot>(Haasonline.Public.LocalApi.CSharp.Enums.EnumCustomBotType.ScalperBot,
                botName, ConfigManager.mainConfig.AccountGUID, market, maincoin, ""));

            createBot.Wait();

            var setupScalpBotComplete = Task.Run(async () => await haasonlineClient.CustomBotApi.SetupScalpingBot(createBot.Result.Result.GUID, botName,
                ConfigManager.mainConfig.AccountGUID, market, maincoin, "", 0, EnumBotTradeAmount.Static, amount, ConfigManager.mainConfig.Fee, maincoin,
                "LOCKEDLIMITORDERGUID", targetPercentage, safetyPercentage));

            setupScalpBotComplete.Wait();

            Thread.Sleep(1000);

            return setupScalpBotComplete.Result.Result;
        }

        public static ScalperBot UpdateScalperBot(string guid, string botName, string primaryCurrency, string secondaryCurrency, string position, decimal amount ,  decimal targetPercentage, decimal safetyPercentage)
        {

            HaasonlineClient haasonlineClient = new HaasonlineClient(ConfigManager.mainConfig.IPAddress, ConfigManager.mainConfig.Port, ConfigManager.mainConfig.Secret);

            var task = Task.Run(async () => await haasonlineClient.CustomBotApi.SetupScalpingBot(guid, botName, ConfigManager.mainConfig.AccountGUID, primaryCurrency,
                secondaryCurrency, "", 0, EnumBotTradeAmount.Static , amount, ConfigManager.mainConfig.Fee, position, "LOCKEDLIMITORDERGUID", targetPercentage, safetyPercentage));

            task.Wait();

            return task.Result.Result;
        }

        public static PriceTick GetOneMinutePriceDataForMarket(EnumPriceSource priceSource, string primaryCoin, string secondaryCoin)
        {
            if (HaasActionManager.CheckHaasConnection())
            {

                HaasonlineClient haasonlineClient = new HaasonlineClient(ConfigManager.mainConfig.IPAddress, ConfigManager.mainConfig.Port, ConfigManager.mainConfig.Secret);

                var priceTickTask = Task.Run(async () => await haasonlineClient.MarketDataApi.GetMinutePriceTicker(priceSource, primaryCoin, secondaryCoin, ""));

                priceTickTask.Wait();

                return priceTickTask.Result.Result;

            }

            return null;
        }

        public static void MarketSellPosition(string primaryCoin, string secondaryCoin, decimal amount)
        {
            if (HaasActionManager.CheckHaasConnection())
            {

                HaasonlineClient haasonlineClient = new HaasonlineClient(ConfigManager.mainConfig.IPAddress, ConfigManager.mainConfig.Port, ConfigManager.mainConfig.Secret);

                var priceTickTask = Task.Run(async () => await haasonlineClient.TradeApi.PlaceSpotSellOrder(ConfigManager.mainConfig.AccountGUID, primaryCoin, secondaryCoin, 0, amount));

            }
        }

        public static void RemoveOpenOrder(string orderId)
        {
            if (HaasActionManager.CheckHaasConnection())
            {

                HaasonlineClient haasonlineClient = new HaasonlineClient(ConfigManager.mainConfig.IPAddress, ConfigManager.mainConfig.Port, ConfigManager.mainConfig.Secret);

                var cancelOpenOrderCommand = Task.Run(async () => await haasonlineClient.TradeApi.CancelTemplate(orderId));

            }
        }

        public static void Test()
        {
            if (HaasActionManager.CheckHaasConnection())
            {

                HaasonlineClient haasonlineClient = new HaasonlineClient(ConfigManager.mainConfig.IPAddress, ConfigManager.mainConfig.Port, ConfigManager.mainConfig.Secret);

                var resultsTask = Task.Run(async () => await haasonlineClient.AccountDataApi.GetOrderTemplates());

                resultsTask.Wait();

                foreach(var result in resultsTask.Result.Result)
                {
                    Console.WriteLine("TEST: {0} - {1}", result.Key, result.Value);

                }

            }
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
