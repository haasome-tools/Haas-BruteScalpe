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
        private const string HistFileName = "ppscreener-cmd";

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
            ActionManager.ShowConfig();
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
                        ActionManager.SetConfigIpAddress(arguments[1]);
                        Console.WriteLine("[*] Haas Ip Address Set To {0}", arguments[1]);
                        break;

                    case "port":
                        int port_dead = 0;
                        if (Int32.TryParse(arguments[1], out port_dead))
                        {
                            ActionManager.SetConfigPort(Convert.ToInt32(arguments[1]));
                            Console.WriteLine("[*] Haas Port Set To {0}", arguments[1]);
                        }
                        else
                        {
                            Console.WriteLine("[!] Argument Is Not A Number");
                        }
                        break;

                    case "secret":
                        ActionManager.SetConfigSecret(arguments[1]);
                        Console.WriteLine("[*] Haas Secret Set To {0}", arguments[1]);
                        break;

                    case "accountguid":
                        ActionManager.SetConfigSecret(arguments[1]);
                        Console.WriteLine("[*] Haas Account GUID Set To {0}", arguments[1]);
                        break;

                    case "keepthreshold":
                        decimal keepthreshold_dead = 0;
                        if (Decimal.TryParse(arguments[1], out keepthreshold_dead))
                        {
                            ActionManager.SetConfigKeepThreshold(Convert.ToDecimal(arguments[1]));
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
                            ActionManager.SetConfigBacktestDelay(Convert.ToInt32(arguments[1]));
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
                            ActionManager.SetConfigKeepThreshold(Convert.ToDecimal(arguments[1]));
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
                            ActionManager.SetConfigBacktestDelay(Convert.ToInt32(arguments[1]));
                            Console.WriteLine("[*] Backtest Length Set To {0}", arguments[1]);
                        }
                        else
                        {
                            Console.WriteLine("[!] Argument Is Not A Number");
                        }
                        break;

                    case "writeresultstofile":
                        ActionManager.SetConfigWriteResultsToFile(Convert.ToBoolean(arguments[1]));
                        Console.WriteLine("[*] Write Results To File Set To {0}", Convert.ToBoolean(arguments[1]));
                        break;

                    case "persistbots":
                        ActionManager.SetConfigPersistBots(Convert.ToBoolean(arguments[1]));
                        Console.WriteLine("[*] Persist Bots Set To {0}", Convert.ToBoolean(arguments[1]));
                        break;

                    case "retrycount":
                        int retrycount_dead = 0;
                        if (Int32.TryParse(arguments[1], out retrycount_dead))
                        {
                            ActionManager.SetConfigRetryCount(Convert.ToInt32(arguments[1]));
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
                            ActionManager.SetConfigStartTargetPercentage(Convert.ToDecimal(arguments[1]));
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
                            ActionManager.SetConfigEndTargetPercentage(Convert.ToDecimal(arguments[1]));
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
                            ActionManager.SetConfigTargetPercentageStep(Convert.ToDecimal(arguments[1]));
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
                            ActionManager.SetConfigStartSafetyPercentage(Convert.ToDecimal(arguments[1]));
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
                            ActionManager.SetConfigEndSafetyPercentage(Convert.ToDecimal(arguments[1]));
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
                            ActionManager.SetConfigSafetyPercentageStep(Convert.ToDecimal(arguments[1]));
                            Console.WriteLine("[*] Safety Percentage Step Set To {0}", arguments[1]);
                        }
                        else
                        {
                            Console.WriteLine("[!] Argument Is Not A Decimal");
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
                if (ActionManager.SaveConfig(arguments[0]))
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
                if (ActionManager.SaveConfig())
                {
                    Console.WriteLine("[*] Saved Default Config File {0}", ActionManager.DefaultConfigName);
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
                if (ActionManager.LoadConfig(arguments[0]))
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
                if (ActionManager.LoadConfig())
                {
                    Console.WriteLine("[*] Loaded Default Config File {0}", ActionManager.DefaultConfigName);
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

            if (ActionManager.CheckHaasConnection())
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

            foreach (var account in ActionManager.GetAccountGUIDS())
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
                    var accounts = ActionManager.GetAccountGUIDS();

                    var accountPair = new Tuple<string, string>("", "");

                    if (index > accounts.Count || index == 0)
                    {
                        Console.WriteLine("[!] Invalid Account Selection");
                    }
                    else
                    {
                        accountPair = ActionManager.GetAccountGUIDS()[Convert.ToInt32(arguments[0]) - 1];
                        ActionManager.SetConfigAccountGuid(accountPair.Item2);
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
            var markets = ActionManager.GetMarkets();

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
            var markets = ActionManager.GetMarkets();

            string[] arguments = Utils.SplitArgumentsSaftley(arg);

            if (arguments.Length == 2)
            {
                if (markets.FindIndex(s => s.Item1.Equals(arguments[0].ToUpper()) == true && s.Item2.Equals(arguments[1].ToUpper()) == true) != -1)
                {
                    ActionManager.AddMarketToTest(arguments[0].ToUpper(), arguments[1].ToUpper());
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
            var markets = ActionManager.GetMarkets();

            string[] arguments = Utils.SplitArgumentsSaftley(arg);

            if (arguments.Length == 2)
            {
                if (markets.FindIndex(s => s.Item1.Equals(arguments[0].ToUpper()) == true && s.Item2.Equals(arguments[1].ToUpper()) == true) != -1)
                {
                    ActionManager.RemoveMarketToTest(arguments[0].ToUpper(), arguments[1].ToUpper());
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
            var markets = ActionManager.mainConfig.MarketsToTest;

            Console.WriteLine("\n---- Currently Selected Markets ----");

            foreach (var market in markets)
            {
                Console.WriteLine("{0}/{1}", market.Item1, market.Item2);
            }

            Console.WriteLine();

        }

        [CmdCommand(Command = "start", Description = StaticStrings.START_SCREENER_HELP_TEXT)]
        public async Task StartScalpingCommand(string arg)
        {

            Console.WriteLine("[*] Starting Brute Scalpe Process");
            Console.WriteLine("[*] Bot Persistance Enabled");

            var markets = ActionManager.mainConfig.MarketsToTest;

            List<BackTestResult> backTestResults = new List<BackTestResult>();

            if (ActionManager.CreateTemplateBot().Equals(""))
            {
                Console.WriteLine("[!] Could Not Create Template Bot");
            }
            else
            {
                int index = 0;

                decimal currentTargetPercentage = ActionManager.mainConfig.StartTargetPercentage;
                decimal currentSafetyPercentage = ActionManager.mainConfig.StartSafetyPercentage;

                decimal winningTargetPercentage = 0.0m;
                decimal winningSafetyPercentage = 0.0m;
                decimal winningROIValue = -1000.0m;

                int runEstimation = Convert.ToInt32(((ActionManager.mainConfig.EndTargetPerecentage - ActionManager.mainConfig.StartTargetPercentage) / ActionManager.mainConfig.TargetPercentageStep) * ((ActionManager.mainConfig.EndSafetyPercentage - ActionManager.mainConfig.StartSafetyPercentage) / ActionManager.mainConfig.SafetyPercentageStep));
                int count = 0;

                foreach (var market in markets)
                {

                    BaseCustomBot bot = new BaseCustomBot();

                    int retryCount = 0;

                    Console.WriteLine("[*] Testing Market: {0}/{1}", market.Item1, market.Item2);

                    var res = ActionManager.GrabMarketData(market.Item1, market.Item2);
                    if (!res)
                    {
                        Console.WriteLine($"[x] Skipping {market.Item1}/{market.Item2}. Failed to load history");
                        continue;
                    }

                    bot = ActionManager.PerformBackTest(market.Item1, market.Item2, currentTargetPercentage, currentSafetyPercentage);

                    backTestResults.Add(Utils.CreateBackTestResult(markets[index].Item1, markets[index].Item2, bot.ROI, currentTargetPercentage, currentSafetyPercentage));

                    while (currentTargetPercentage < ActionManager.mainConfig.EndTargetPerecentage)
                    {
                        while (currentSafetyPercentage < ActionManager.mainConfig.EndSafetyPercentage)
                        {
                            count++;

                            var botResults = ActionManager.PerformBackTest(market.Item1, market.Item2, currentTargetPercentage, currentSafetyPercentage);

                            Console.Write("\r[+] Processing [{0} of {1}] - Target: {2} Safety: {3} ROI: {4}", count, runEstimation, currentTargetPercentage, currentSafetyPercentage, botResults.ROI);

                            if (botResults.ROI > winningROIValue)
                            {
                                winningTargetPercentage = currentTargetPercentage;
                                winningSafetyPercentage = currentSafetyPercentage;
                                winningROIValue = botResults.ROI;
                            }

                            Thread.Sleep(ActionManager.mainConfig.BackTestDelayInMiliseconds);

                            backTestResults.Add(Utils.CreateBackTestResult(markets[index].Item1, markets[index].Item2, botResults.ROI, currentTargetPercentage, currentSafetyPercentage));

                            currentSafetyPercentage = currentSafetyPercentage + ActionManager.mainConfig.SafetyPercentageStep;

                        }

                        currentSafetyPercentage = ActionManager.mainConfig.StartSafetyPercentage;

                        currentTargetPercentage = currentTargetPercentage + ActionManager.mainConfig.TargetPercentageStep;

                    }

                    string details = "";

                    if (ActionManager.mainConfig.PersistBots)
                    {
                        if (winningROIValue >= ActionManager.mainConfig.KeepThreshold)
                        {
                            details = "Persisted";
                            ActionManager.CreatePersistentBot(market.Item1, market.Item2, winningTargetPercentage, winningSafetyPercentage);
                        }
                        else
                        {
                            details = "Ignored";
                        }
                    }

                    Console.WriteLine();
                    Console.WriteLine("[*] Winning {0} - Target: {1} Saftey {2} ROI: {3:N4}% - {4}", "BS-" + market.Item1 + ":" + market.Item2, winningTargetPercentage, winningSafetyPercentage, winningROIValue, details);
                }

                if (ActionManager.mainConfig.WriteResultsToFile)
                {
                    using (TextWriter writer = new StreamWriter(@"BackTestResults.csv"))
                    {
                        var csv = new CsvWriter(writer);
                        csv.WriteRecords(backTestResults);
                        writer.Flush();
                    }
                }
                ActionManager.DeleteTemplateBot();
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
            if (ActionManager.PerformStartup())
            {
                Console.WriteLine("[*] Succesfully Loaded Default Config {0} ", ActionManager.DefaultConfigName);
            }
            else
            {
                Console.WriteLine("[!] Failed To Load Default Config {0} ", ActionManager.DefaultConfigName);
                Console.WriteLine("[!] Generated New Config Config {0} ", ActionManager.DefaultConfigName);
                ActionManager.SaveConfig();
            }

        }

        public override void PostLoop()
        {

        }
    }
}
