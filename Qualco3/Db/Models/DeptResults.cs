using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Db.Models
{
   public  class  DeptResults
    {
       public int BillsCount { get; set;}
        public int NewUsers { get; set; }
        public string HttpStatus { get; set; }
        public List <ErrorLines> ErrorLines { get; set; }
        public string Message { get; set; }

    }
}

