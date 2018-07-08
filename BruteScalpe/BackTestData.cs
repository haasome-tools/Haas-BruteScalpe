using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Haasonline.Public.LocalApi.CSharp.Enums;

namespace BruteScalp
{
    public class BackTestHistory
    {
        public List<BackTestData> history { get; set; } = new List<BackTestData>();
    }

    public class BackTestData
    {
        public EnumPriceSource Exchange { get; set; }
        public string AccountGUID { get; set; }
        public string PrimaryCurrency { get; set; }
        public string SecondayCurrency { get; set; }
        public decimal ActivationROI { get; set; }
        public decimal ObservedHigh { get; set; }

    }
}
