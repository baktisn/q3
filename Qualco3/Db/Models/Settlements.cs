using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;



namespace Db.Models
{
    public class Settlements
    {

        public int ID { get; set; }
       
        public DateTime RequestDate { get; set; }
        public DateTime LastName { get; set; }
        public decimal DownPayment { get; set; }
        public Int16 Installments { get; set; }
        public Int16 Interest { get; set; }
        public byte IsAccepted { get; set; }
        [ForeignKey("SettlementTypes")]
        public int SettlementTypeId { get; set; }

        public SettlementTypes SettlementType { get; set; }


    }
}
