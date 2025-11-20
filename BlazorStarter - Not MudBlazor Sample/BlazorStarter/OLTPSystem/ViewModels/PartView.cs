using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLTPSystem.ViewModels
{
    public class PartView
	{
		public int PartID { get; set; }
		public int PartCategoryID { get; set; }
		public string CategoryName { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public decimal Price { get; set; }
		public decimal Cost { get; set; }
		public int ROL { get; set; }
		public int QOH { get; set; }
		public bool Taxable { get; set; }
		public bool RemoveFromViewFlag { get; set; }
	}
}
