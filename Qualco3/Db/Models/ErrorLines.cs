using System;
using System.Collections.Generic;
using System.Text;

namespace Db.Models
{
    public class ErrorLines
    {
        public int line { get; set; }
        public string ErrorMessage { get; set; }
        public string LineString { get; set; }
    }
}
