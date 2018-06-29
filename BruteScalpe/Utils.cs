using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
            Console.WriteLine("By: R4stl1n - Special Thanks To Cosmos, Quintus, And ChrisOCC");
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

        public static T DeepCopy<T>(T obj)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new Exception("The source object must be serializable");
            }

            if (Object.ReferenceEquals(obj, null))
            {
                throw new Exception("The source object must not be null");
            }

            T result = default(T);

            using (var memoryStream = new System.IO.MemoryStream())
            {
                var formatter = new BinaryFormatter();

                formatter.Serialize(memoryStream, obj);
                memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
                result = (T)formatter.Deserialize(memoryStream);
                memoryStream.Close();

            }
            return result;
        }

        public static BackTestResult CreateBackTestResult(string market, string maincoin, decimal ROI, decimal targetPercentage, decimal safetyPercentage)
        {
            BackTestResult btResultI = new BackTestResult
            {
                AboveThreshold = (ROI >= ConfigManager.mainConfig.KeepThreshold),
                Date = DateTime.Now,
                Fee = ConfigManager.mainConfig.Fee,
                TargetPercentage = targetPercentage,
                SafetyPercentage = safetyPercentage,
                Pair = market + "/" + maincoin,
                ROI = ROI
            };

            return btResultI;
        }
    }
}
