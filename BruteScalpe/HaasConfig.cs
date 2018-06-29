using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace BruteScalp
{
    public class HaasConfig
    {
        // Default Configuration Settings
        public string IPAddress { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 8096;
        public string Secret { get; set; } = "SomeSecretHere";
        public string AccountGUID { get; set; } = "ReplaceMeWithGuid";

        // Custom Configuration Settings
        public int RetryCount = 10;
        public decimal KeepThreshold = 2.0m;
        public int BackTestDelayInMiliseconds = 500;
        public decimal Fee { get; set; } = 0.1m;
        public bool PersistBots { get; set; } = true;
        public int BackTestLength { get; set; } = 1440;
        public bool WriteResultsToFile { get; set; } = false;

        // Scalp Settings
        public decimal StartTargetPercentage { get; set; } = 0.1m;
        public decimal EndTargetPerecentage { get; set; } = 1.0m;
        public decimal TargetPercentageStep { get; set; } = 0.1m;

        public decimal StartSafetyPercentage { get; set; } = 0.1m;
        public decimal EndSafetyPercentage { get; set; } = 1.0m;
        public decimal SafetyPercentageStep { get; set; } = 0.1m;

        // Automation Settings
        public int MinutesBeforeAutoScalpeRetest { get; set; } = 1440;
        public decimal AmountOfCurrencyForAutoScalpeToUse { get; set; } = 0.1m;
        public int TimeBetweenGlobalSafetyCheck { get; set; } = 5;
        public decimal GlobalPercentageLossToDeactivate { get; set; } = 5m;
        public bool SellPositionWhenBotDeactivates { get; set; } = true;
        public decimal NeverReactivatePercentageLoss { get; set; } = 20.0m;

        public List<Tuple<string, string>> MarketsToTest { get; set; } = new List<Tuple<string, string>>();

    }
}
