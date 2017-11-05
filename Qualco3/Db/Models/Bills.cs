using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Db.Models
{
    public class Bills
    {
        public int ID { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public string Bill_description  { get; set; }
        public byte Status { get; set; }

        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }
        [ForeignKey("PaymentMethods")]
        public Int16 PaymentMethodId { get; set; }
        [ForeignKey("SettlementId")]
        public int SettlementId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
        public PaymentMethods PaymentMethods { get; set; }
        public Settlements Settlement { get; set; }

    }
}
