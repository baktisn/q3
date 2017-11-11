using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Db.Models
{
    public class CitizenDepts
    {
        public int ID { get; set; } 
        public string VAT { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string County { get; set; }
        public string BillId { get; set; }
        public string Bill_description { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public string UserGUId { get; set; }

    }
}
