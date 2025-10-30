using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLTPSystem.ViewModels
{
    public class InvoiceView
    {
        public int InvoiceID { get; set; }
        public DateOnly InvoiceDate { get; set; }
        public int CustomerID { get; set; }
        public int EmployeeID { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public bool RemoveFromViewFlag { get; set; }
        //Calculated Fields
        public string CustomerName { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        //Read-Only Field (get only)
        //	After the lamda (=>) is what is returned when this field is called
        public decimal Total => SubTotal + Tax;
        //Related Records
        public List<InvoiceLineView> InvoiceLines { get; set; } = [];
    }
}
