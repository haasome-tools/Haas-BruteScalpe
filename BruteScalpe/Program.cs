using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;

using Newtonsoft.Json;
using Haasonline.Public.LocalApi.CSharp;

namespace BruteScalpe
{

    public class BruteScalpeConfig
    {
        public string IPAddress { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 8096;
        public string Secret { get; set; } = "SomeSecretHere";
        public string PrimarySecondaryCurrency { get; set; } = "BTC";
        public int MinutesToBackTest { get; set; } = 1440;

        public decimal Fee { get; set; } = 0.1m;
        public int DelayBTInMiliseconds = 1000;

        public decimal StartTargetPercentage { get; set; } = 0.1m;
        public decimal EndTargetPrecentage { get; set; } = 1.0m;

        public decimal StartSafteyPercentage { get; set; } = 0.1m;
        public decimal EndSafteyPrecentage { get; set; } = 1.0m;

        public decimal TargetPercentageStep { get; set; } = 0.1m;
        public decimal SafteyPercentageStep { get; set; } = 0.1m;

        public HashSet<string> MarketsToTest { get; set; } = new HashSet<string>();


    }

    public class Program
    {

        public static string configFileName = "BSConfig.json";

        public static BruteScalpeConfig mainConfig;

        public static void ShowBanner()
        {
            Console.WriteLine("    ____             __      _____            __         ");
            Console.WriteLine("   / __ )_______  __/ /____ / ___/_________ _/ /___  ___ ");
            Console.WriteLine("  / __  / ___/ / / / __/ _ \\__ \\/ ___/ __ `/ / __ \\/ _ \\");
            Console.WriteLine(" / /_/ / /  / /_/ / /_/  __/__/ / /__/ /_/ / / /_/ /  __/");
            Console.WriteLine("/_____/_/   \\__,_/\\__/\\___/____/\\___/\\__,_/_/ .___/\\___/ ");
            Console.WriteLine("                                           /_/           ");
            Console.WriteLine("By: R4stl1n && Cosmos");
            Console.WriteLine();
            Console.WriteLine();
        }

        public static bool CheckForConfig()
        {
            Program.mainConfig = new BruteScalpeConfig();

            if (File.Exists(Program.configFileName))
            {
                Program.mainConfig.MarketsToTest = new HashSet<string>();
                Program.mainConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<BruteScalpeConfig>(File.ReadAllText(Program.configFileName));
                return true;
            }

            // Create a config
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(Program.mainConfig);
            File.WriteAllText(Program.configFileName, json);
            Console.WriteLine("[*] BSConfig.json Created Succesfully");
            Console.WriteLine("[!] Config Not Found, config created please modify and then relaunch.");

            return false;
        }

        public static string GetWorkingAccountGUID()
        {
            HaasonlineClient haasonlineClient = new HaasonlineClient(mainConfig.IPAddress, mainConfig.Port, mainConfig.Secret);

            var accounts = haasonlineClient.AccountDataApi.GetEnabledAccounts();

            // Quick hacky to get a key
            foreach (string x in accounts.Result.Result.Keys)
            {
                return x;
            }

            return "";
        }

        public static bool CheckConnection()
        {
            try
            {
                Console.WriteLine("[*] Verifying API Connection and Credentials");
                HaasonlineClient haasonlineClient = new HaasonlineClient(mainConfig.IPAddress, mainConfig.Port, mainConfig.Secret);

                var results = haasonlineClient.TestCreditials();

                Thread.Sleep(1000);

                return results.Result;
            }
            catch
            {
                return false;
            }

        }


        static void Main(string[] args)
        {

            Program.ShowBanner();

            Console.WriteLine("[*] Starting up BruteScalpe for Haasonline");
            Console.WriteLine("[*] Checking for config");

            if (Program.CheckForConfig())
            {

                if (CheckConnection())
                {

                    var workingAccountGUID = Program.GetWorkingAccountGUID();

                    List<string> activeMarketsList = new List<string>();

                    HaasonlineClient haasonlineClient = new HaasonlineClient(mainConfig.IPAddress, mainConfig.Port, mainConfig.Secret);

                    var markets = haasonlineClient.MarketDataApi.GetPriceMarkets(Haasonline.Public.LocalApi.CSharp.Enums.EnumPriceSource.Binance);

                    foreach (var market in markets.Result.Result)
                    {
                        if (market.SecondaryCurrency.Equals(mainConfig.PrimarySecondaryCurrency))
                        {
                            activeMarketsList.Add(market.PrimaryCurrency);
                        }
                    }

                    Console.WriteLine("[*] Loaded {0} Markets From Exchange Against Primary Secondary Currency {1}", activeMarketsList.Count, mainConfig.PrimarySecondaryCurrency);
                    Console.WriteLine("[*] Loaded {0} Markets To Brute Scalpe From Config File", mainConfig.MarketsToTest.Count);

                    Console.WriteLine();
                    Console.WriteLine("[S] Target Percentage Start:{0} End:{1} Step: {2}", mainConfig.StartTargetPercentage, mainConfig.EndTargetPrecentage, mainConfig.TargetPercentageStep);
                    Console.WriteLine("[S] Saftey Percentage Start:{0} End:{1} Step: {2}", mainConfig.StartSafteyPercentage, mainConfig.EndSafteyPrecentage, mainConfig.TargetPercentageStep);

                    Console.WriteLine();

                    foreach (string market in mainConfig.MarketsToTest)
                    {
                        string botName = "BS-" + market + ":" + mainConfig.PrimarySecondaryCurrency;

                        var newBot = haasonlineClient.CustomBotApi.NewBot(Haasonline.Public.LocalApi.CSharp.Enums.EnumCustomBotType.ScalperBot, botName, workingAccountGUID, market, mainConfig.PrimarySecondaryCurrency, "");
                        Thread.Sleep(1000);
                        var allBots = haasonlineClient.CustomBotApi.GetAllBots();

                        Console.WriteLine("[*] Currently Testing: {0}:{1}", market, mainConfig.PrimarySecondaryCurrency);

                        decimal currentTargetPercentage = mainConfig.StartTargetPercentage;
                        decimal currentSafteyPercentage = mainConfig.StartSafteyPercentage;

                        int runEstimation = Convert.ToInt32((mainConfig.EndTargetPrecentage / mainConfig.TargetPercentageStep) * (mainConfig.EndSafteyPrecentage / mainConfig.SafteyPercentageStep));

                        // Find the Bot guid
                        foreach (var bot in allBots.Result.Result)
                        {

                            decimal winningTargetPercentage = 0.0m;
                            decimal winningSafteyPercentage = 0.0m;
                            decimal winningROIValue = -1000.0m;

                            int count = 0;

                            if (bot.Name.Equals(botName))
                            {

                                while (currentTargetPercentage < mainConfig.EndTargetPrecentage)
                                {
                                    while (currentSafteyPercentage < mainConfig.EndSafteyPrecentage)
                                    {
                                        count++;

                                        Console.Write("\r[+] Processing [{0} of {1}]", count, runEstimation);

                                        var setupScalpBot = haasonlineClient.CustomBotApi.SetupScalpingBot(bot.GUID, bot.Name, workingAccountGUID, market, mainConfig.PrimarySecondaryCurrency, "", 0, 1000, mainConfig.Fee, mainConfig.PrimarySecondaryCurrency, "LOCKEDLIMIT", currentTargetPercentage, currentSafteyPercentage);
                                        var backtestBot = haasonlineClient.CustomBotApi.BacktestBot<Haasonline.Public.LocalApi.CSharp.DataObjects.CustomBots.ScalperBot>(bot.GUID, mainConfig.MinutesToBackTest);
                                        Thread.Sleep(mainConfig.DelayBTInMiliseconds);
                                        var botResults = haasonlineClient.CustomBotApi.GetBot<Haasonline.Public.LocalApi.CSharp.DataObjects.CustomBots.ScalperBot>(bot.GUID);

                                        if (botResults.Result.Result.ROI > winningROIValue)
                                        {
                                            winningTargetPercentage = currentTargetPercentage;
                                            winningSafteyPercentage = currentSafteyPercentage;
                                            winningROIValue = botResults.Result.Result.ROI;
                                        }

                                        currentSafteyPercentage = currentSafteyPercentage + mainConfig.SafteyPercentageStep;
                                    }
                                    currentSafteyPercentage = mainConfig.StartSafteyPercentage;
                                    currentTargetPercentage = currentTargetPercentage + mainConfig.TargetPercentageStep;
                                }

                                var setupScalpBotComplete = haasonlineClient.CustomBotApi.SetupScalpingBot(bot.GUID, bot.Name, workingAccountGUID, market, mainConfig.PrimarySecondaryCurrency, "", 0, 1000, 0.1m, mainConfig.PrimarySecondaryCurrency, "LOCKEDLIMIT", winningTargetPercentage, winningSafteyPercentage);
                                var backtestBotComplete = haasonlineClient.CustomBotApi.BacktestBot<Haasonline.Public.LocalApi.CSharp.DataObjects.CustomBots.ScalperBot>(bot.GUID, mainConfig.MinutesToBackTest);

                                Console.WriteLine();
                                Console.WriteLine("[*] {0} - Target: {1} Saftey {2} ROI: {3:N4}%", botName, winningTargetPercentage, winningSafteyPercentage, winningROIValue);
                                Console.WriteLine();
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("[!] Connection failed please verify IP, Port and Secret in config");
                }


            }

            Console.ReadLine();
        }
    }
}
