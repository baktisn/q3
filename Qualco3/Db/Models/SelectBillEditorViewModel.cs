using System;
using System.Collections.Generic;
using System.Text;

namespace Db.Models
{
    public class SelectBillEditorViewModel
    {
        public bool Selected { get; set; }
        public int ID { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public string Bill_description { get; set; }
        public byte Status { get; set; }
        public decimal Total_Amount { get; set; }

    }
}
