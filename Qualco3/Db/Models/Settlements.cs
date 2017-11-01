using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Db.Models
{
    public class Settlements
    {

        public int ID { get; set; }

        [Display(Name = "Request Date")]
        public DateTime RequestDate
        {
            get
            {
                return this.dateCreated.HasValue
                   ? this.dateCreated.Value
                   : DateTime.Now;
            }

            set { this.dateCreated = value; }
        }
        private DateTime? dateCreated = null;
      

        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Display(Name = "Down Payment")]
        public decimal DownPayment { get; set; }

        public Int16 Installments { get; set; }

        public Int16 Interest { get; set; }

        public byte IsAccepted { get; set; }

        [ForeignKey("SettlementTypes")]
        public int SettlementTypeId { get; set; }

        public SettlementTypes SettlementType { get; set; }


    }
}
