using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Db.Models
{
    public class SettlementTypes
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public Int16 DownPaymentPercentage { get; set; }
        public Int16 MaxNoInstallments { get; set; }
        public decimal Interest { get; set; }

    }
}
