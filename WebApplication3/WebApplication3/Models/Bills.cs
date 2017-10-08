using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication3.Models
{
    public class Bills
    {
        public int ID { get; set; }
        public string UserId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }

       // public Users Users { get; set; }
    }
}
