using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication3.Models
{
    public class Bills
    {
        public int ID { get; set; }
        public string userId { get; set; }
        public decimal amount { get; set; }
        public string description { get; set; }

       // public Users Users { get; set; }
    }
}
