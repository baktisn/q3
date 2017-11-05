using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Db.Models
{
    public class BillSelectionViewModel
    {
        public List<SelectBillEditorViewModel> Bills { get; set; }
        public decimal TotalAmount { get; set; }

        public BillSelectionViewModel()
        {
            this.Bills = new List<SelectBillEditorViewModel>();
        }
        public IEnumerable<int> getSelectedIds()
        {

            return (from p in this.Bills where p.Selected select p.ID).ToList();
        }

       


    }
}
