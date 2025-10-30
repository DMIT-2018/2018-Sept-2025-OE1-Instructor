using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLTPSystem.ViewModels
{
    public class InvoiceListView
    {
        public int InvoiceID { get; set; }
        public DateOnly InvoiceDate { get; set; }
        public int CustomerID { get; set; }
        public int EmployeeID { get; set; }
        //Calculated Fields
        public string CustomerName { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public decimal Total { get; set; }
    }
}
