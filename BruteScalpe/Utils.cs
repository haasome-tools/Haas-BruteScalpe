using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BruteScalp
{
    class Utils
    {
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
        }

        public static string[] SplitArgumentsSaftley(string arg)
        {
            string[] arguments = arg.Split(' ');

            if (arg.Equals(""))
            {
                arguments = new string[] { };
            }

            return arguments;
        }

        public static BackTestResult CreateBackTestResult(string market, string maincoin, decimal ROI, decimal targetPercentage, decimal safetyPercentage)
        {
            BackTestResult btResultI = new BackTestResult();
            btResultI.AboveThreshold = (ROI >= ActionManager.mainConfig.KeepThreshold);
            btResultI.Date = DateTime.Now;
            btResultI.Fee = ActionManager.mainConfig.Fee;
            btResultI.TargetPercentage = targetPercentage;
            btResultI.SafetyPercentage = safetyPercentage;
            btResultI.Pair = market + "/" + maincoin;
            btResultI.ROI = ROI;

            return btResultI;
        }
    }
}
