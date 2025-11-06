using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLTPSystem.ViewModels
{
    public class InvoicePartView
    {
        public int PartID { get; set; }
        public int PartCategoryID { get; set; }
        //CalcField
        public string CategoryName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int QOH { get; set; } //QuantityOnHand
        public bool Taxable { get; set; }
    }
}
