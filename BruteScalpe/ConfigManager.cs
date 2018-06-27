using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BruteScalp
{
    public static class ConfigManager
    {
        public static HaasConfig mainConfig;

        public static string DefaultConfigFileName = "HaasTool.config";

        public static bool LoadConfig()
        {

            if (File.Exists(DefaultConfigFileName))
            {
                ConfigManager.mainConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<HaasConfig>(File.ReadAllText(DefaultConfigFileName));
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
                ConfigManager.mainConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<HaasConfig>(File.ReadAllText(fileName));
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
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(ConfigManager.mainConfig, Formatting.Indented);

                File.WriteAllText(DefaultConfigFileName, json);

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
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(ConfigManager.mainConfig, Formatting.Indented);

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
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(ConfigManager.mainConfig));
        }

        public static void SetConfigIpAddress(string ipAddress)
        {
            ConfigManager.mainConfig.IPAddress = ipAddress;
        }

        public static void SetConfigPort(int port)
        {
            ConfigManager.mainConfig.Port = port;
        }

        public static void SetConfigSecret(string secret)
        {
            ConfigManager.mainConfig.Secret = secret;
        }

        public static void SetConfigAccountGuid(string accountGUID)
        {
            ConfigManager.mainConfig.AccountGUID = accountGUID;
        }

        public static void SetConfigKeepThreshold(decimal threshold)
        {
            ConfigManager.mainConfig.KeepThreshold = threshold;
        }

        public static void SetConfigBacktestDelay(int delay)
        {
            ConfigManager.mainConfig.BackTestDelayInMiliseconds = delay;
        }

        public static void SetConfigFee(decimal fee)
        {
            ConfigManager.mainConfig.Fee = fee;
        }

        public static void SetConfigBackTestLength(int length)
        {
            ConfigManager.mainConfig.BackTestLength = length;
        }

        public static void SetConfigWriteResultsToFile(bool writeToFile)
        {
            ConfigManager.mainConfig.WriteResultsToFile = writeToFile;
        }

        public static void SetConfigPersistBots(bool persist)
        {
            ConfigManager.mainConfig.PersistBots = persist;
        }

        public static void SetConfigRetryCount(int count)
        {
            ConfigManager.mainConfig.RetryCount = count;
        }

        public static void SetConfigStartTargetPercentage(decimal percentage)
        {
            ConfigManager.mainConfig.StartTargetPercentage = percentage;
        }

        public static void SetConfigEndTargetPercentage(decimal percentage)
        {
            ConfigManager.mainConfig.EndTargetPerecentage = percentage;
        }

        public static void SetConfigTargetPercentageStep(decimal percentage)
        {
            ConfigManager.mainConfig.TargetPercentageStep = percentage;
        }

        public static void SetConfigStartSafetyPercentage(decimal percentage)
        {
            ConfigManager.mainConfig.StartSafetyPercentage = percentage;
        }

        public static void SetConfigEndSafetyPercentage(decimal percentage)
        {
            ConfigManager.mainConfig.EndSafetyPercentage = percentage;
        }

        public static void SetConfigSafetyPercentageStep(decimal percentage)
        {
            ConfigManager.mainConfig.SafetyPercentageStep = percentage;
        }

        public static void AddMarketToTest(string market, string maincoin)
        {
            ConfigManager.mainConfig.MarketsToTest.Add(new Tuple<string, string>(market, maincoin));
        }

        public static void RemoveMarketToTest(string market, string maincoin)
        {
            ConfigManager.mainConfig.MarketsToTest.Remove(new Tuple<string, string>(market, maincoin));
        }
    }
}
