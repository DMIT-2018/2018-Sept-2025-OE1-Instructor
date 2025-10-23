using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLTPSystem.ViewModels
{
    public class CustomerEditView
    {
        public int CustomerID { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string Address2 { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public int ProvStateID { get; set; }
        public int CountryID { get; set; }
        public string PostalCode { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int StatusID { get; set; }
        public bool RemoveFromViewFlag { get; set; }
        //Additional Fields
        //We can add additional calculated fields (fields not in the database) still
        public string OriginalFirstName { get; set; } = string.Empty;
        public bool HasInvoices { get; set; }
    }
}
