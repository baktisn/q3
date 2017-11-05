using System;
using System.Collections.Generic;
using System.Text;

namespace Db.Models
{
    public class SubmitSelected
    {
        public IEnumerable<Bills> Bills { get; set; }
        public decimal TotalAmount { get; set; }
        public IEnumerable<SettlementTypes> SettlementTypes {get; set;}

    }
}
