
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using Haasonline.Public.LocalApi.CSharp.DataObjects.CustomBots;
using NCmd;

namespace BruteScalp
{
    class InteractiveShell : Cmd
    {
        private const string HistFileName = "brutescalp-cmd";
        private static System.Timers.Timer autoScalpeTimer;
        private static System.Timers.Timer autoSafetyTimer;

        private bool autoScalpeStarted = false;
        private bool autoSafetyStarted = false;

        [CmdCommand(Command = "exit", Description = StaticStrings.EXIT_HELP_TEXT)]
        public void ExitCommand(string arg)
        {

            ExitLoop();
        }

        [CmdCommand(Command = "clear", Description = StaticStrings.CLEAR_HELP_TEXT)]
        public void ClearCommand(string arg)
        {
            Console.Clear();
        }

        [CmdCommand(Command = "version", Description = StaticStrings.VERSION_HELP_TEXT)]
        public void ShowVersion(string arg)
        {
            WriteVersionStatement(new AutoProgramMetaData(GetType().Assembly), Console.Out);
        }

        [CmdCommand(Command = "show-config", Description = StaticStrings.SHOW_CONFIG_HELP_TEXT)]
        public void ShowConfigCommand(string arg)
        {
            ConfigManager.ShowConfig();
        }

        [CmdCommand(Command = "set-config", Description = StaticStrings.SET_CONFIG_HELP_TEXT)]
        public void SetConfigCommand(string arg)
        {
            string[] arguments = Utils.SplitArgumentsSaftley(arg);

            if (arguments.Length >= 2)
            {
                switch (arguments[0])
                {
                    case "ipaddress":
                        ConfigManager.SetConfigIpAddress(arguments[1]);
                        Console.WriteLine("[*] Haas Ip Address Set To {0}", arguments[1]);
                        break;

                    case "port":
                        int port_dead = 0;
                        if (Int32.TryParse(arguments[1], out port_dead))
                        {
                            ConfigManager.SetConfigPort(Convert.ToInt32(arguments[1]));
                            Console.WriteLine("[*] Haas Port Set To {0}", arguments[1]);
                        }
                        else
                        {
                            Console.WriteLine("[!] Argument Is Not A Number");
                        }
                        break;

                    case "secret":
                        ConfigManager.SetConfigSecret(arguments[1]);
                        Console.WriteLine("[*] Haas Secret Set To {0}", arguments[1]);
                        break;

                    case "accountguid":
                        ConfigManager.SetConfigSecret(arguments[1]);
                        Console.WriteLine("[*] Haas Account GUID Set To {0}", arguments[1]);
                        break;

                    case "keepthreshold":
                        decimal keepthreshold_dead = 0;
                        if (Decimal.TryParse(arguments[1], out keepthreshold_dead))
                        {
                            ConfigManager.SetConfigKeepThreshold(Convert.ToDecimal(arguments[1]));
                            Console.WriteLine("[*] Keep Threshold Set To {0}", arguments[1]);
                        }
                        else
                        {
                            Console.WriteLine("[!] Argument Is Not A Decimal");
                        }
                        break;

                    case "backtestdelay":
                        int backtestdelay_dead = 0;
                        if (Int32.TryParse(arguments[1], out backtestdelay_dead))
                        {
                            ConfigManager.SetConfigBacktestDelay(Convert.ToInt32(arguments[1]));
                            Console.WriteLine("[*] Backtest Delay Set To {0}", arguments[1]);
                        }
                        else
                        {
                            Console.WriteLine("[!] Argument Is Not A Number");
                        }
                        break;

                    case "fee":
                        decimal fee_dead = 0;
                        if (Decimal.TryParse(arguments[1], out fee_dead))
                        {
                            ConfigManager.SetConfigKeepThreshold(Convert.ToDecimal(arguments[1]));
                            Console.WriteLine("[*] Fee Set To {0}", arguments[1]);
                        }
                        else
                        {
                            Console.WriteLine("[!] Argument Is Not A Decimal");
                        }
                        break;

                    case "backtestlength":
                        int backtestlength_dead = 0;
                        if (Int32.TryParse(arguments[1], out backtestlength_dead))
                        {
                            ConfigManager.SetConfigBacktestDelay(Convert.ToInt32(arguments[1]));
                            Console.WriteLine("[*] Backtest Length Set To {0}", arguments[1]);
                        }
                        else
                        {
                            Console.WriteLine("[!] Argument Is Not A Number");
                        }
                        break;

                    case "writeresultstofile":
                        ConfigManager.SetConfigWriteResultsToFile(Convert.ToBoolean(arguments[1]));
                        Console.WriteLine("[*] Write Results To File Set To {0}", Convert.ToBoolean(arguments[1]));
                        break;

                    case "persistbots":
                        ConfigManager.SetConfigPersistBots(Convert.ToBoolean(arguments[1]));
                        Console.WriteLine("[*] Persist Bots Set To {0}", Convert.ToBoolean(arguments[1]));
                        break;

                    case "retrycount":
                        int retrycount_dead = 0;
                        if (Int32.TryParse(arguments[1], out retrycount_dead))
                        {
                            ConfigManager.SetConfigRetryCount(Convert.ToInt32(arguments[1]));
                            Console.WriteLine("[*] Retry Count Set To {0}", arguments[1]);
                        }
                        else
                        {
                            Console.WriteLine("[!] Argument Is Not A Number");
                        }
                        break;

                    case "starttargetpercentage":
                        decimal starttargetpercentage_dead = 0;
                        if (Decimal.TryParse(arguments[1], out starttargetpercentage_dead))
                        {
                            ConfigManager.SetConfigStartTargetPercentage(Convert.ToDecimal(arguments[1]));
                            Console.WriteLine("[*] Start Target Percentage Set To {0}", arguments[1]);
                        }
                        else
                        {
                            Console.WriteLine("[!] Argument Is Not A Decimal");
                        }
                        break;

                    case "endtargetpercentage":
                        decimal endtargetpercentage_dead = 0;
                        if (Decimal.TryParse(arguments[1], out endtargetpercentage_dead))
                        {
                            ConfigManager.SetConfigEndTargetPercentage(Convert.ToDecimal(arguments[1]));
                            Console.WriteLine("[*] End Target Percentage Set To {0}", arguments[1]);
                        }
                        else
                        {
                            Console.WriteLine("[!] Argument Is Not A Decimal");
                        }
                        break;

                    case "targetpercentagestep":
                        decimal targetpercentagestep_dead = 0;
                        if (Decimal.TryParse(arguments[1], out targetpercentagestep_dead))
                        {
                            ConfigManager.SetConfigTargetPercentageStep(Convert.ToDecimal(arguments[1]));
                            Console.WriteLine("[*] Target Percentage Step Set To {0}", arguments[1]);
                        }
                        else
                        {
                            Console.WriteLine("[!] Argument Is Not A Decimal");
                        }
                        break;

                    case "startsafetypercentage":
                        decimal startsafetypercentage_dead = 0;
                        if (Decimal.TryParse(arguments[1], out startsafetypercentage_dead))
                        {
                            ConfigManager.SetConfigStartSafetyPercentage(Convert.ToDecimal(arguments[1]));
                            Console.WriteLine("[*] Start Safety Percentage Set To {0}", arguments[1]);
                        }
                        else
                        {
                            Console.WriteLine("[!] Argument Is Not A Decimal");
                        }
                        break;

                    case "endsafetypercentage":
                        decimal endsafetypercentage_dead = 0;
                        if (Decimal.TryParse(arguments[1], out endsafetypercentage_dead))
                        {
                            ConfigManager.SetConfigEndSafetyPercentage(Convert.ToDecimal(arguments[1]));
                            Console.WriteLine("[*] End Safety Percentage Set To {0}", arguments[1]);
                        }
                        else
                        {
                            Console.WriteLine("[!] Argument Is Not A Decimal");
                        }
                        break;

                    case "safetypercentagestep":
                        decimal safetypercentagestep_dead = 0;
                        if (Decimal.TryParse(arguments[1], out safetypercentagestep_dead))
                        {
                            ConfigManager.SetConfigSafetyPercentageStep(Convert.ToDecimal(arguments[1]));
                            Console.WriteLine("[*] Safety Percentage Step Set To {0}", arguments[1]);
                        }
                        else
                        {
                            Console.WriteLine("[!] Argument Is Not A Decimal");
                        }
                        break;

                    case "minutesbeforeautoscalperetest":
                        int timebeforeretest_dead = 0;
                        if (Int32.TryParse(arguments[1], out timebeforeretest_dead))
                        {
                            ConfigManager.SetMinutesBeforeAutoScalpeRetest(Convert.ToInt32(arguments[1]));
                            Console.WriteLine("[*] Time Before Retest Count Set To {0}", arguments[1]);
                        }
                        else
                        {
                            Console.WriteLine("[!] Argument Is Not A Number");
                        }
                        break;

                    case "autoscalpecurrencyuse":
                        decimal amountofcurrencyforautoscalpetouse_dead = 0;
                        if (Decimal.TryParse(arguments[1], out amountofcurrencyforautoscalpetouse_dead))
                        {
                            ConfigManager.SetAmountOfCurrencyForAutoScalpeToUse(Convert.ToDecimal(arguments[1]));
                            Console.WriteLine("[*] Amount Of Currency For Auto Scalpe To Use Set To {0}", arguments[1]);
                        }
                        else
                        {
                            Console.WriteLine("[!] Argument Is Not A Number");
                        }
                        break;

                    case "timeglobalcheck":
                        int timeglobalcheck_dead = 0;
                        if (Int32.TryParse(arguments[1], out timeglobalcheck_dead))
                        {
                            ConfigManager.SetTimeBetweenGlobalSafetyCheck(Convert.ToInt32(arguments[1]));
                            Console.WriteLine("[*] Time Between Global Safety Check Set To {0}", arguments[1]);
                        }
                        else
                        {
                            Console.WriteLine("[!] Argument Is Not A Number");
                        }
                        break;

                    case "globalpercentagesafety":
                        decimal globalpercentagesafety_dead = 0;
                        if (Decimal.TryParse(arguments[1], out globalpercentagesafety_dead))
                        {
                            ConfigManager.SetGlobalPercentageLossToDeactivate(Convert.ToDecimal(arguments[1]));
                            Console.WriteLine("[*] Global Percentage Loss To Deactivate Set To {0}", arguments[1]);
                        }
                        else
                        {
                            Console.WriteLine("[!] Argument Is Not A Number");
                        }
                        break;

                    case "sellwhenbotdeactivates":
                        ConfigManager.SetSellPositionWhenBotDeactivates(Convert.ToBoolean(arguments[1]));
                        Console.WriteLine("[*] Global Percentage Loss To Deactivate Set To {0}", Convert.ToBoolean(arguments[1]));

                        break;

                    case "neverreativateperecentageloss":
                        decimal neverreativateperecentageloss_dead = 0;

                        if (Decimal.TryParse(arguments[1], out neverreativateperecentageloss_dead))
                        {
                            ConfigManager.SetNeverReactivatePercentageLoss(Convert.ToDecimal(arguments[1]));
                            Console.WriteLine("[*] Never Reactivate Percentage Loss Set To {0}", arguments[1]);
                        }
                        else
                        {
                            Console.WriteLine("[!] Argument Is Not A Number");
                        }
                        break;

                    default:
                        Console.WriteLine("Argument not valid");
                        break;

                }

            }
            else
            {
                Console.WriteLine("[!] Not Enough Arguments Specified");
                Console.WriteLine("Ex. set-config <configOption> <value>");
            }

        }

        [CmdCommand(Command = "save-config", Description = StaticStrings.SAVE_CONFIG_HELP_TEXT)]
        public void SaveConfigCommand(string arg)
        {
            string[] arguments = Utils.SplitArgumentsSaftley(arg);

            if (arguments.Length == 1)
            {
                if (ConfigManager.SaveConfig(arguments[0]))
                {
                    Console.WriteLine("[*] Saved Config With Filename {0}", arg[0]);
                }
                else
                {
                    Console.WriteLine("[!] Could Not Save Config {0}", arg[0]);
                }
            }
            else
            {
                if (ConfigManager.SaveConfig())
                {
                    Console.WriteLine("[*] Saved Default Config File {0}", ConfigManager.DefaultConfigFileName);
                }
                else
                {
                    Console.WriteLine("[!] Could Not Save Default Config File {0}", arg[0]);
                }
            }
        }

        [CmdCommand(Command = "load-config", Description = StaticStrings.LOAD_CONFIG_HELP_TEXT)]
        public void LoadConfigCommand(string arg)
        {
            string[] arguments = Utils.SplitArgumentsSaftley(arg);

            if (arguments.Length == 1)
            {
                if (ConfigManager.LoadConfig(arguments[0]))
                {
                    Console.WriteLine("[*] Loaded Config With Filename {0}", arg[0]);
                }
                else
                {
                    Console.WriteLine("[!] Could Not Load Config {0}", arg[0]);
                }
            }
            else
            {
                if (ConfigManager.LoadConfig())
                {
                    Console.WriteLine("[*] Loaded Default Config File {0}", ConfigManager.DefaultConfigFileName);
                }
                else
                {
                    Console.WriteLine("[!] Could Not Load Default Config File {0}", arg[0]);
                }
            }
        }

        [CmdCommand(Command = "test-creds", Description = StaticStrings.TEST_CREDS_HELP_TEXT)]
        public void TestCredsCommand(string arg)
        {
            Console.WriteLine("[*] Verifying API Connection and Credentials");

            if (HaasActionManager.CheckHaasConnection())
            {
                Console.WriteLine("[*] Connection Succesfull");
            }
            else
            {
                Console.WriteLine("[!] Connection Failed Check ip:port/credentials");
            }

        }

        [CmdCommand(Command = "show-accounts", Description = StaticStrings.SHOW_ACCOUNTS_HELP_TEXT)]
        public void ShowAccountGuidsCommand(string arg)
        {
            int count = 1;

            Console.WriteLine("\n---- Current Active Accounts ----");

            foreach (var account in HaasActionManager.GetAccountGUIDS())
            {
                Console.WriteLine("#{0} - {1} : {2}", count, account.Item1, account.Item2);
                count++;
            }

            Console.WriteLine();

        }

        [CmdCommand(Command = "set-account", Description = StaticStrings.SET_ACCOUNT_HELP_TEXT)]
        public void SetAccountCommand(string arg)
        {
            string[] arguments = Utils.SplitArgumentsSaftley(arg);

            if (arguments.Length == 1)
            {
                int dead = 0;
                if (Int32.TryParse(arguments[0], out dead))
                {
                    int index = Convert.ToInt32(arguments[0]);
                    var accounts = HaasActionManager.GetAccountGUIDS();

                    var accountPair = new Tuple<string, string>("", "");

                    if (index > accounts.Count || index == 0)
                    {
                        Console.WriteLine("[!] Invalid Account Selection");
                    }
                    else
                    {
                        accountPair = HaasActionManager.GetAccountGUIDS()[Convert.ToInt32(arguments[0]) - 1];
                        ConfigManager.SetConfigAccountGuid(accountPair.Item2);
                        Console.WriteLine("[*] Haas Account Set To {0} : {1}", accountPair.Item1, accountPair.Item2);
                    }

                }
                else
                {
                    Console.WriteLine("[!] Argument Is Not A Number");
                }
            }
            else
            {
                Console.WriteLine("Not Enough Arguments");
                Console.WriteLine("Ex. set-account <account-num>");
            }
        }

        [CmdCommand(Command = "show-markets", Description = StaticStrings.SHOW_MARKETS_HELP_TEXT)]
        public void ShowMarketsCommand(string arg)
        {
            var markets = HaasActionManager.GetMarkets();

            if (markets.Count == 0)
            {
                Console.WriteLine("[!] Could Not Obtain Market Information");
            }
            else
            {
                Console.WriteLine("\n---- Current Markets ----");
                foreach (var market in markets)
                {
                    Console.WriteLine("{0}/{1}", market.Item1, market.Item2);
                }
            }

        }

        [CmdCommand(Command = "add-test-market", Description = StaticStrings.ADD_TEST_MARKET_HELP_TEXT)]
        public void AddTestMarketCommand(string arg)
        {
            var markets = HaasActionManager.GetMarkets();

            string[] arguments = Utils.SplitArgumentsSaftley(arg);

            if (arguments.Length == 2)
            {
                if (markets.FindIndex(s => s.Item1.Equals(arguments[0].ToUpper()) == true && s.Item2.Equals(arguments[1].ToUpper()) == true) != -1)
                {
                    ConfigManager.AddMarketToTest(arguments[0].ToUpper(), arguments[1].ToUpper());
                    Console.WriteLine("[*] Market {0}/{1} Added To Test List", arguments[0].ToUpper(), arguments[1].ToUpper());
                }
                else
                {
                    Console.WriteLine("[!] Market Does Not Exist On Exchange");
                }
            }
            else
            {
                Console.WriteLine("[!] Not Enough Arguments Specified");
                Console.WriteLine("Ex. add-test-market <market> <maincoin>");
            }

        }

        [CmdCommand(Command = "remove-test-market", Description = StaticStrings.REMOVE_TEST_MARKET_HELP_TEXT)]
        public void RemoveTestMarketCommand(string arg)
        {
            var markets = HaasActionManager.GetMarkets();

            string[] arguments = Utils.SplitArgumentsSaftley(arg);

            if (arguments.Length == 2)
            {
                if (markets.FindIndex(s => s.Item1.Equals(arguments[0].ToUpper()) == true && s.Item2.Equals(arguments[1].ToUpper()) == true) != -1)
                {
                    ConfigManager.RemoveMarketToTest(arguments[0].ToUpper(), arguments[1].ToUpper());
                    Console.WriteLine("[*] Market {0}/{1} Removed From Test List", arguments[0].ToUpper(), arguments[1].ToUpper());
                }
                else
                {
                    Console.WriteLine("[!] Market Does Not Exist On Exchange");
                }
            }
            else
            {
                Console.WriteLine("[!] Not Enough Arguments Specified");
                Console.WriteLine("Ex. remove-test-market <market> <maincoin>");
            }

        }

        [CmdCommand(Command = "show-test-markets", Description = StaticStrings.SHOW_TEST_MARKETS_HELP_TEXT)]
        public void ShowTestMarketsCommand(string arg)
        {
            var markets = ConfigManager.mainConfig.MarketsToTest;

            Console.WriteLine("\n---- Currently Selected Markets ----");

            foreach (var market in markets)
            {
                Console.WriteLine("{0}/{1}", market.Item1, market.Item2);
            }

            Console.WriteLine();

        }

        [CmdCommand(Command = "start", Description = StaticStrings.START_SCREENER_HELP_TEXT)]
        public void StartScalpingCommand(string arg)
        {

            Console.WriteLine("[*] Starting Brute Scalpe Process");
            Console.WriteLine("[*] Bot Persistance Enabled");

            var markets = ConfigManager.mainConfig.MarketsToTest;

            List<BackTestResult> backTestResults = new List<BackTestResult>();

            if (HaasActionManager.CreateTemplateBot().Equals(""))
            {
                Console.WriteLine("[!] Could Not Create Template Bot");
            }
            else
            {

                int index = 0;

                BaseCustomBot botWinning = new BaseCustomBot();

                int runEstimation = Convert.ToInt32(((ConfigManager.mainConfig.EndTargetPerecentage - ConfigManager.mainConfig.StartTargetPercentage) / ConfigManager.mainConfig.TargetPercentageStep) * ((ConfigManager.mainConfig.EndSafetyPercentage - ConfigManager.mainConfig.StartSafetyPercentage) / ConfigManager.mainConfig.SafetyPercentageStep));

                foreach (var market in markets)
                {
                    try
                    {
                        int count = 0;

                        decimal winningTargetPercentage = 0.0m;
                        decimal winningSafetyPercentage = 0.0m;
                        decimal winningROIValue = -1000.0m;

                        decimal currentTargetPercentage = ConfigManager.mainConfig.StartTargetPercentage;
                        decimal currentSafetyPercentage = ConfigManager.mainConfig.StartSafetyPercentage;

                        Console.WriteLine("[*] Testing Market: {0}/{1}", market.Item1, market.Item2);


                        var res = HaasActionManager.GrabMarketData(market.Item1, market.Item2);
                        if (!res)
                        {
                            Console.WriteLine($"[x] Skipping {market.Item1}/{market.Item2}. Failed to load history");
                            continue;
                        }

                        while (currentTargetPercentage < ConfigManager.mainConfig.EndTargetPerecentage)
                        {
                            while (currentSafetyPercentage < ConfigManager.mainConfig.EndSafetyPercentage)
                            {
                                count++;

                                var botResults = HaasActionManager.PerformBackTest(market.Item1, market.Item2, currentTargetPercentage, currentSafetyPercentage);

                                Console.Write("\r[+] Processing [{0} of {1}] - Target: {2} Safety: {3} ROI: {4}", count, runEstimation, currentTargetPercentage, currentSafetyPercentage, botResults.ROI);

                                if (botResults.ROI > winningROIValue)
                                {
                                    winningTargetPercentage = currentTargetPercentage;
                                    winningSafetyPercentage = currentSafetyPercentage;
                                    winningROIValue = botResults.ROI;

                                    botWinning = botResults;
                                }

                                Thread.Sleep(ConfigManager.mainConfig.BackTestDelayInMiliseconds);

                                backTestResults.Add(Utils.CreateBackTestResult(markets[index].Item1, markets[index].Item2, botResults.ROI, currentTargetPercentage, currentSafetyPercentage));

                                currentSafetyPercentage = currentSafetyPercentage + ConfigManager.mainConfig.SafetyPercentageStep;

                            }

                            currentSafetyPercentage = ConfigManager.mainConfig.StartSafetyPercentage;

                            currentTargetPercentage = currentTargetPercentage + ConfigManager.mainConfig.TargetPercentageStep;

                        }

                        string details = "";

                        if (ConfigManager.mainConfig.PersistBots)
                        {
                            if (winningROIValue >= ConfigManager.mainConfig.KeepThreshold)
                            {
                                details = "Persisted";

                                string[] accountGuidSplit = ConfigManager.mainConfig.AccountGUID.Split('-');

                                string botName = "BS-" + accountGuidSplit[0] + "-" + market.Item1 + ":" + market.Item2;

                                BackTestHistoryManager.UpdateHistoryEntry(ConfigManager.mainConfig.AccountGUID, botWinning);

                                HaasActionManager.CreatePersistentBot(botName, market.Item1, market.Item2, winningTargetPercentage, winningSafetyPercentage);
                            }
                            else
                            {
                                details = "Ignored";
                            }
                        }

                        Console.WriteLine();
                        Console.WriteLine("[*] Winning {0} - Target: {1} Saftey {2} ROI: {3}% - {4}", "BS-" + market.Item1 + ":" + market.Item2, winningTargetPercentage, winningSafetyPercentage, winningROIValue, details);

                        index++;
                    } catch (Exception e ) {
                        Console.WriteLine("[!] Exception occured StackTrace Follows:\n {0}", e.ToString());
                    }
                }

                if (ConfigManager.mainConfig.WriteResultsToFile)
                {
                    using (TextWriter writer = new StreamWriter(@"BackTestResults.csv"))
                    {
                        var csv = new CsvWriter(writer);
                        csv.WriteRecords(backTestResults);
                        writer.Flush();
                    }
                }

                BackTestHistoryManager.SaveBackTestHistory();

                HaasActionManager.DeleteTemplateBot();
            }
        }

        [CmdCommand(Command = "start-auto-scalpe", Description = StaticStrings.START_AUTO_SCALPE_HELP_TEXT)]
        public void StartAutoScalpeCommand(string arg)
        {

            if (!autoScalpeStarted)
            {
                Console.WriteLine("[*] Starting Auto Scalpe Process");
                Console.WriteLine("[*] Performing Initial AutoScalpe Update");

                ProcessAutoScalpeUpdate();

                Console.WriteLine("[*] Scheduled Reoccuring Auto Retest To {0} Minutes", TimeSpan.FromMinutes(ConfigManager.mainConfig.MinutesBeforeAutoScalpeRetest));

                autoScalpeTimer = new System.Timers.Timer(Convert.ToInt32(TimeSpan.FromMinutes(ConfigManager.mainConfig.MinutesBeforeAutoScalpeRetest).TotalMilliseconds));
                autoScalpeTimer.Elapsed += async (sender, e) => await ProcessAutoScalpeUpdate();

                autoScalpeTimer.Start();

                autoScalpeStarted = true;
            }
            else
            {
                Console.WriteLine("[*] Auto Scalpe Already Running");
            }
        }

        [CmdCommand(Command = "stop-auto-scalpe", Description = StaticStrings.STOP_AUTO_SCALPE_HELP_TEXT)]
        public void StopAutoScalpeCommand(string arg)
        {
            Console.WriteLine("[*] Stopping The Auto Scalpe Process");
            if (autoScalpeTimer != null)
            {
                autoScalpeTimer.Stop();
                autoScalpeStarted = false;
            }
        }

        [CmdCommand(Command = "start-auto-safety", Description = StaticStrings.START_AUTO_SAFETY_HELP_TEXT)]
        public void StartAutoSafetyCommand(string arg)
        {
            if (!autoSafetyStarted)
            {
                Console.WriteLine("[*] Starting Auto Safety Process");

                Console.WriteLine("[*] Scheduled Reoccuring Auto Saftey Test To {0} Minutes", TimeSpan.FromMinutes(ConfigManager.mainConfig.TimeBetweenGlobalSafetyCheck));

                autoSafetyTimer = new System.Timers.Timer(Convert.ToInt32(TimeSpan.FromMinutes(ConfigManager.mainConfig.TimeBetweenGlobalSafetyCheck).TotalMilliseconds));
                autoSafetyTimer.Elapsed += async (sender, e) => await ProcessAutoSafetyUpdate();

                autoSafetyTimer.Start();

                autoSafetyStarted = true;
            }
            else
            {
                Console.WriteLine("[*] Auto Safety Already Running");
            }
        }

        [CmdCommand(Command = "stop-auto-safety", Description = StaticStrings.STOP_AUTO_SAFETY_HELP_TEXT)]
        public void StopAutoSafetyCommand(string arg)
        {
            Console.WriteLine("[*] Stopping The Auto Safety Process");
            if (autoSafetyTimer != null)
            {
                autoSafetyTimer.Stop();
                autoSafetyStarted = false;
            }
        }

        [CmdCommand(Command = "deactivate-all-bots", Description = StaticStrings.DEACTIVATE_ALL_BOTS_HELP_TEXT)]
        public void DeactivateAllBotsCommand(string arg) {

            string[] accountGuidSplit = ConfigManager.mainConfig.AccountGUID.Split('-');


            var customBots = HaasActionManager.GetAllCustomBots();

            var markets = AutoScalpeManager.GetMarketsPrioritized();

            Console.WriteLine("[*] Deactivating All Bots And Closing Open Orders");

            foreach (var market in markets)
            {

                string botName = "BS-" + accountGuidSplit[0] + "-" + market.Item1 + ":" + market.Item2;

                var customBot = HaasActionManager.GetCustomBotByName(botName);

                if (customBot != null)
                {
                    HaasActionManager.RemoveOpenOrder(customBot.OpenOrderId);
                    HaasActionManager.DeactivateCustomBot(customBot.GUID);

                }

            }
        }

        [CmdCommand(Command = "sell-all-bot-positions", Description = StaticStrings.SELL_ALL_BOT_POSITIONS_HELP_TEXT)]
        public void SellAllBotPositionsCommand(string arg)
        {
            string[] accountGuidSplit = ConfigManager.mainConfig.AccountGUID.Split('-');

            var customBots = HaasActionManager.GetAllCustomBots();

            var markets = AutoScalpeManager.GetMarketsPrioritized();

            Console.WriteLine("[*] Market Selling All Bots Positions Managed By BruteScalpe");

            foreach (var market in markets)
            {

                string botName = "BS-" + accountGuidSplit[0] + "-" + market.Item1 + ":" + market.Item2;

                var customBot = HaasActionManager.GetScalperBotByName(botName);

                if (customBot != null)
                {

                    if (customBot.CoinPosition == Haasonline.Public.LocalApi.CSharp.Enums.EnumCoinsPosition.Bought)
                    {
                        var currentPosition = customBot.PriceMarket.SecondaryCurrency;


                        Console.WriteLine("[*] Auto Management - Placing Market Sell For Bot {0} Position", botName);

                        // Sell the position using market.
                        HaasActionManager.MarketSellPosition(market.Item1, market.Item2, customBot.CurrentTradeAmount);

                    }


                    HaasActionManager.UpdateScalperBot(customBot.GUID, botName, market.Item1, market.Item2, 
                                                       customBot.PriceMarket.SecondaryCurrency, customBot.CurrentTradeAmount, 
                                                       customBot.MinimumTargetChange, customBot.MaxAllowedReverseChange);

                }

            }
        }

        [CmdCommand(Command = "remove-all-bots", Description = StaticStrings.REMOVE_ALL_BOTS_HELP_TEXT)]
        public void RemoveAllBotsCommand(string arg)
        {
            string[] accountGuidSplit = ConfigManager.mainConfig.AccountGUID.Split('-');


            var customBots = HaasActionManager.GetAllCustomBots();

            var markets = AutoScalpeManager.GetMarketsPrioritized();

            Console.WriteLine("[*] Deleteing All Bots Managed By BruteScalpe");

            foreach (var market in markets)
            {

                string botName = "BS-" + accountGuidSplit[0] + "-" + market.Item1 + ":" + market.Item2;

                var customBot = HaasActionManager.GetScalperBotByName(botName);

                if (customBot != null)
                {
                    HaasActionManager.DeleteBot(customBot.GUID);
                }

            }
        }

        [CmdCommand(Command = "reset-brute-scalpe", Description = StaticStrings.RESET_BRUTE_SCALPE_HELP_TEXT)]
        public void ResetBruteScalpeCommand(string arg)
        {
            Console.WriteLine("[!!!!] Reseting Brute Scalpe");
            StopAutoSafetyCommand("");
            StopAutoScalpeCommand("");
            DeactivateAllBotsCommand("");
            SellAllBotPositionsCommand("");
            RemoveAllBotsCommand("");

            Console.WriteLine("[*] Deleting Backtest History");
            BackTestHistoryManager.DeleteBackTestHistory();
            BackTestHistoryManager.PerformStartup();
 
        }

        [CmdCommand(Command = "reset-test-market", Description = StaticStrings.RESET_TEST_MARKET_HELP_TEXT)]
        public void ResetTestMarketCommand(string arg)
        {
            string[] arguments = Utils.SplitArgumentsSaftley(arg);

            string[] accountGuidSplit = ConfigManager.mainConfig.AccountGUID.Split('-');

            if (arguments.Length >= 2)
            {
                string botName = "BS-" + accountGuidSplit[0] + "-" + arguments[0] + ":" + arguments[1];

                var history = BackTestHistoryManager.GetHistoryForAccount(ConfigManager.mainConfig.AccountGUID);

                var customBot = HaasActionManager.GetScalperBotByName(botName);

                if (customBot != null)
                {
                    var btData = AutoScalpeManager.GetHistoryForMarket(ConfigManager.mainConfig.AccountGUID, history, new Tuple<string, string>(arguments[0], arguments[1]));

                    HaasActionManager.DeactivateCustomBot(customBot.GUID);
                
                    if (customBot.CoinPosition == Haasonline.Public.LocalApi.CSharp.Enums.EnumCoinsPosition.Bought)
                    {
                        Console.WriteLine("[*] Market Retest - Placing Market Sell For Bot {0} Position", botName);

                        // Sell the position using market.
                        HaasActionManager.MarketSellPosition(arguments[0], arguments[1], customBot.CurrentTradeAmount);

                    }
                    BackTestHistoryManager.RemoveHistoryEntry(btData);
                    HaasActionManager.DeleteBot(customBot.GUID);

                    PerformRetest(arguments[0], arguments[1]);
                }
            }
            else
            {
                Console.WriteLine("[!] Not Enough Arguments Specified");
                Console.WriteLine("Ex. retest-test-market <primaryCoin> <SecondaryCoin>");
            }
        }

        public Task<string> ProcessAutoScalpeUpdate()
        {
            Console.WriteLine();

            string[] accountGuidSplit = ConfigManager.mainConfig.AccountGUID.Split('-');

            string botNamePrefixToMatch = "BS-" + accountGuidSplit[0];

            var markets = AutoScalpeManager.GetMarketsPrioritized();

            var customBots = HaasActionManager.GetAllCustomBotsWithPrefix(botNamePrefixToMatch);

            var accountInfo = HaasActionManager.GetAccountInformation();

            var history = BackTestHistoryManager.GetHistoryForAccount(ConfigManager.mainConfig.AccountGUID);

            decimal activationROI = 0.0m;

            List<BackTestResult> backTestResults = new List<BackTestResult>();

            if (HaasActionManager.CreateTemplateBot().Equals(""))
            {
                Console.WriteLine("[!] Could Not Create Template Bot");
            }
            else
            {
                foreach (var market in markets)
                {
                    try
                    {
                        string botName = "BS-" + accountGuidSplit[0] + "-" + market.Item1 + ":" + market.Item2;

                        Console.WriteLine("[*] Auto Management - Testing Market: {0}/{1}", market.Item1, market.Item2);

                        var res = HaasActionManager.GrabMarketData(market.Item1, market.Item2);

                        if (!res)
                        {
                            Console.WriteLine($"[x] Skipping {market.Item1}/{market.Item2}. Failed to load history");
                            continue;
                        }

                        var winningTrade = HaasActionManager.PerformFullTest(market.Item1, market.Item2);

                        // Check if we are above the threshold
                        if (winningTrade.roi >= ConfigManager.mainConfig.KeepThreshold)
                        {
                            var btData = AutoScalpeManager.GetHistoryForMarket(ConfigManager.mainConfig.AccountGUID, history, market);

                            Console.WriteLine("[*] Auto Management - Market Backtest Above Set Keep Threshold");

                            var customBot = HaasActionManager.GetCustomBotByName(botName);


                            if (customBot != null)
                            {
                                // Check if we should reactivate the bot at all
                                if (customBot.ROI > (-ConfigManager.mainConfig.NeverReactivatePercentageLoss))
                                {

                                    Console.WriteLine("[*] Auto Management - Better Settings Found For {0}", botName);

                                    // Need to stop bot and update settings then start
                                    HaasActionManager.DeactivateCustomBot(customBot.GUID);

                                    Console.WriteLine("[*] Auto Management - Deactivate Bot {0}", botName);

                                    // Position of bot
                                    string currentPosition = "";

                                    if (customBot.CoinPosition == Haasonline.Public.LocalApi.CSharp.Enums.EnumCoinsPosition.Bought)
                                    {
                                        currentPosition = customBot.PriceMarket.PrimaryCurrency;
                                    }
                                    else
                                    {
                                        currentPosition = customBot.PriceMarket.SecondaryCurrency;
                                    }

                                    var updatedBot = HaasActionManager.UpdateScalperBot(customBot.GUID, botName, market.Item1, market.Item2, currentPosition, customBot.CurrentTradeAmount, winningTrade.targetPercentage, winningTrade.safetyPercentage);

                                    Console.WriteLine("[*] Auto Management - Updated Settings Found For {0}", botName);

                                    HaasActionManager.ActivateCustomBot(customBot.GUID);

                                    Console.WriteLine("[*] Auto Management - Reactivated Bot {0}", botName);

                                    BackTestHistoryManager.UpdateHistoryEntry(ConfigManager.mainConfig.AccountGUID, updatedBot);
                                }
                                else
                                {
                                    Console.WriteLine("[*] Auto Management - Ignoring {0} has permanently disabled", botName);
                                }
                            }
                            else
                            {
                                Console.WriteLine("[*] Auto Management - Creating New Scalper Bot {0}", botName);

                                var priceTicker = HaasActionManager.GetOneMinutePriceDataForMarket(accountInfo.ConnectedPriceSource, market.Item1, market.Item2);

                                var amount = ConfigManager.mainConfig.AmountOfCurrencyForAutoScalpeToUse / priceTicker.Close;

                                // No History create bot
                                var newBot = HaasActionManager.CreateAutoPersistentBot(botName, market.Item1, market.Item2, Math.Round(amount), winningTrade.targetPercentage, winningTrade.safetyPercentage);

                                HaasActionManager.ActivateCustomBot(newBot.GUID);

                                Console.WriteLine("[*] Auto Management - Activated Bot {0}", botName);
                            }
                        }
                        else
                        {
                            Console.WriteLine("[*] Auto Management - Market Backtest Below Set Keep Threshold");

                            var customBot = HaasActionManager.GetScalperBotByName(botName);

                            if (customBot != null)
                            {
                                if (customBot.Activated)
                                {
                                    Console.WriteLine("[*] Auto Management - Deactivating Bot {0} Due To Past Market Test", botName);

                                    // We Need To Stop The Bot And Check To Sell Position
                                    HaasActionManager.DeactivateCustomBot(customBot.GUID);

                                    // Remove any open orders the bot might have
                                    HaasActionManager.RemoveOpenOrder(customBot.OpenOrderId);

                                    // Reset Position of bot
                                    string currentPosition = "";

                                    // If we should sell and reset the bots position when deactivated
                                    if (ConfigManager.mainConfig.SellPositionWhenBotDeactivates)
                                    {
                                        if (customBot.CoinPosition == Haasonline.Public.LocalApi.CSharp.Enums.EnumCoinsPosition.Bought)
                                        {
                                            currentPosition = customBot.PriceMarket.SecondaryCurrency;

                                            Console.WriteLine("[*] Auto Management - Placing Market Sell For Bot {0} Position", botName);

                                            // Sell the position using market.
                                            HaasActionManager.MarketSellPosition(market.Item1, market.Item2, customBot.CurrentTradeAmount);

                                        }

                                        Console.WriteLine("[*] Auto Management - Bot {0} Reset", botName);

                                        // Need to record this for the auto saftey
                                        activationROI = customBot.ROI;

                                        // Update the position of the bot
                                        HaasActionManager.UpdateScalperBot(customBot.GUID, botName, market.Item1, market.Item2, currentPosition, customBot.CurrentTradeAmount, customBot.MinimumTargetChange, customBot.MaxAllowedReverseChange);

                                    }

                                    // Just deactivate
                                    if (customBot.CoinPosition == Haasonline.Public.LocalApi.CSharp.Enums.EnumCoinsPosition.Bought)
                                    {
                                        currentPosition = customBot.PriceMarket.PrimaryCurrency;
                                    }
                                    else
                                    {
                                        currentPosition = customBot.PriceMarket.SecondaryCurrency;
                                    }

                                    // Need to record this for the auto saftey
                                    activationROI = customBot.ROI;

                                    HaasActionManager.UpdateScalperBot(customBot.GUID, botName, market.Item1, market.Item2, currentPosition, customBot.CurrentTradeAmount, customBot.MinimumTargetChange, customBot.MaxAllowedReverseChange);
                                }
                            }
                        }

                        BackTestHistoryManager.UpdateHistoryEntry(ConfigManager.mainConfig.AccountGUID, accountInfo.ConnectedPriceSource, market.Item1, market.Item2, activationROI);

                        Console.WriteLine("[*] Auto Management - Winning {0} - Target: {1} Saftey {2} ROI: {3:N4}%", "BS-" + market.Item1 + ":" + market.Item2, winningTrade.targetPercentage, winningTrade.safetyPercentage, winningTrade.roi);

                    } 
                    catch ( Exception e) 
                    {
                        Console.WriteLine("[!] Exception occured StackTrace Follows:\n {0}", e.ToString());
                    }
                }

                Console.WriteLine("[*] Auto Management - Auto Retest Complete");

                BackTestHistoryManager.SaveBackTestHistory();

                HaasActionManager.DeleteTemplateBot();

            }

            return Task.FromResult(string.Empty);
        }

        public Task<string> ProcessAutoSafetyUpdate()
        {

            string[] accountGuidSplit = ConfigManager.mainConfig.AccountGUID.Split('-');

            string botNamePrefixToMatch = "BS-" + accountGuidSplit[0];

            var customBots = HaasActionManager.GetAllCustomBotsWithPrefix(botNamePrefixToMatch);

            Console.WriteLine("[*] Auto Management - Processing Auto Safety Test");

            decimal activationRoi = 0.0m;


            foreach (var customBot in customBots)
            {
                try
                {
                    if(customBot.Activated) {
                        
                        var backTestHistory = BackTestHistoryManager.GetHistoryForBot(customBot);

                        if (backTestHistory != null)
                        {
                            activationRoi = backTestHistory.ActivationROI;
                        }

                        if (customBot.ROI < (activationRoi + (-ConfigManager.mainConfig.GlobalPercentageLossToDeactivate)))
                        {
                            Console.WriteLine("[*] Auto Management - Deactivating Bot {0} Due To Auto Safety Check", customBot.Name);

                            // We Need To Stop The Bot And Check To Sell Position
                            HaasActionManager.DeactivateCustomBot(customBot.GUID);

                            // Remove any open orders the bot might have
                            HaasActionManager.RemoveOpenOrder(customBot.OpenOrderId);

                            var scalperBot = HaasActionManager.GetScalperBotByName(customBot.Name);

                            // Reset Position of bot
                            string currentPosition = "";

                            // If we should sell and reset the bots position when deactivated
                            if (ConfigManager.mainConfig.SellPositionWhenBotDeactivates)
                            {
                                if (customBot.CoinPosition == Haasonline.Public.LocalApi.CSharp.Enums.EnumCoinsPosition.Bought)
                                {
                                    currentPosition = customBot.PriceMarket.SecondaryCurrency;

                                    Console.WriteLine("[*] Auto Management - Placing Market Sell For Bot {0} Position", customBot.Name);

                                    // Sell the position using market.
                                    HaasActionManager.MarketSellPosition(customBot.PriceMarket.PrimaryCurrency, customBot.PriceMarket.SecondaryCurrency, customBot.CurrentTradeAmount);

                                }

                                Console.WriteLine("[*] Auto Management - Bot {0} Reset", customBot.Name);

                                // Update the position of the bot
                                HaasActionManager.UpdateScalperBot(customBot.GUID, customBot.Name, customBot.PriceMarket.PrimaryCurrency, customBot.PriceMarket.SecondaryCurrency, currentPosition, customBot.CurrentTradeAmount, scalperBot.MinimumTargetChange, scalperBot.MaxAllowedReverseChange);

                            }

                            // Just deactivate
                            if (customBot.CoinPosition == Haasonline.Public.LocalApi.CSharp.Enums.EnumCoinsPosition.Bought)
                            {
                                currentPosition = customBot.PriceMarket.PrimaryCurrency;
                            }
                            else
                            {
                                currentPosition = customBot.PriceMarket.SecondaryCurrency;
                            }

                            HaasActionManager.UpdateScalperBot(customBot.GUID, customBot.Name, customBot.PriceMarket.PrimaryCurrency, customBot.PriceMarket.SecondaryCurrency, currentPosition, customBot.CurrentTradeAmount, scalperBot.MinimumTargetChange, scalperBot.MaxAllowedReverseChange);

                            var backTestData = BackTestHistoryManager.GetHistoryForBot(customBot);

                            BackTestHistoryManager.RemoveHistoryEntry(backTestData);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("[!] Exception occured StackTrace Follows:\n {0}", e.ToString());
                }
            }

            return Task.FromResult(string.Empty);
        }

        public void PerformRetest(string primaryCoin, string secondaryCoin)
        {

            Console.WriteLine("[*] Market Retest - Performing Retest on Market {0}/{1}", primaryCoin, secondaryCoin);

            string[] accountGuidSplit = ConfigManager.mainConfig.AccountGUID.Split('-');
            
            var accountInfo = HaasActionManager.GetAccountInformation();
            string botName = "BS-" + accountGuidSplit[0] + "-" + primaryCoin + ":" + secondaryCoin;

            try
            {
                var winningTrade = HaasActionManager.PerformFullTest(primaryCoin, secondaryCoin);

                // Check if we are above the threshold
                if (winningTrade.roi >= ConfigManager.mainConfig.KeepThreshold)
                {

                    Console.WriteLine("[*] Market Retest - Market Backtest Above Set Keep Threshold");


                    Console.WriteLine("[*] Market Retest - Creating New Scalper Bot {0}", botName);

                    var priceTicker = HaasActionManager.GetOneMinutePriceDataForMarket(accountInfo.ConnectedPriceSource, primaryCoin, secondaryCoin);

                    var amount = ConfigManager.mainConfig.AmountOfCurrencyForAutoScalpeToUse / priceTicker.Close;

                    // No History create bot
                    var newBot = HaasActionManager.CreateAutoPersistentBot(botName, primaryCoin, secondaryCoin, Math.Round(amount), winningTrade.targetPercentage, winningTrade.safetyPercentage);

                    HaasActionManager.ActivateCustomBot(newBot.GUID);

                    Console.WriteLine("[*] Market Retest - Activated Bot {0}", botName);

                }
                else
                {
                    Console.WriteLine("[*] Market Retest Below Threshold");
                }

                BackTestHistoryManager.UpdateHistoryEntry(ConfigManager.mainConfig.AccountGUID, accountInfo.ConnectedPriceSource, primaryCoin, secondaryCoin, 0.0m);
            }
            catch (Exception e)
            {
                Console.WriteLine("[!] Exception occured StackTrace Follows:\n {0}", e.ToString());
            }

        }

        public InteractiveShell()
        {

            Utils.ShowBanner();

            Intro = "";
            CommandPrompt = "$> ";
            HistoryFileName = HistFileName;
        }

        public override void PostCmd(string line)
        {
            base.PostCmd(line);
            //Console.WriteLine();
        }

        public override string PreCmd(string line)
        {
            return base.PreCmd(line);
        }

        public override void EmptyLine()
        {
            Console.WriteLine("Please enter a command or type 'help' for assitance.");
        }

        public override void PreLoop()
        {
            if (HaasActionManager.PerformStartup())
            {
                Console.WriteLine("[*] Succesfully Loaded Default Config {0} ", ConfigManager.DefaultConfigFileName);
            }
            else
            {
                Console.WriteLine("[!] Failed To Load Default Config {0} ", ConfigManager.DefaultConfigFileName);
                Console.WriteLine("[!] Generated New Config Config {0} ", ConfigManager.DefaultConfigFileName);
                ConfigManager.SaveConfig();
            }

            if (BackTestHistoryManager.PerformStartup())
            {
                Console.WriteLine("[*] Succesfully Loaded Backtest History");
            }
            else
            {
                Console.WriteLine("[!] Failed To Load Backtest History");
                Console.WriteLine("[!] Generated New Backtest History File");
                BackTestHistoryManager.SaveBackTestHistory();
            }

        }

        public override void PostLoop()
        {

        }
    }
}
