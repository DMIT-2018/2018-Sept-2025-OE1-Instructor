using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLTPSystem.ViewModels
{
    public class InvoiceLineView
    {
        public int InvoiceLineID { get; set; }
        public int InvoiceID { get; set; }
        public int PartID { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public bool RemoveFromViewFlag { get; set; }
        //Calculated Fields
        public string PartDescription { get; set; } = string.Empty;
        public bool Taxable { get; set; }
        //Read-Only Field (get only)
        public decimal ExtentPrice => Price * Quantity;
    }
}
