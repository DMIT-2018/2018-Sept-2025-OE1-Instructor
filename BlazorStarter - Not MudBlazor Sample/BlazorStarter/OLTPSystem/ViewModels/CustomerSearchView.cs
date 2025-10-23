using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLTPSystem.ViewModels
{
    public class CustomerSearchView
    {
        //Building the Class to cut down and only return
        //	the information we need to display the required data

        //Almost ALWAYS have a reference of the Primary Key(s)
        //	This is used to make sure we are looking at the right record
        public int CustomerID { get; set; }
        //Calculated Field - Only a string but it doesn't 
        //	exist in the database
        public string FullName { get; set; } = string.Empty; //All Strings must default to an empty string
                                                             //Calculated Field
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        //Calculated Field
        public string Status { get; set; } = string.Empty;
        //Calculated Field
        public decimal TotalSales { get; set; }
    }
}
