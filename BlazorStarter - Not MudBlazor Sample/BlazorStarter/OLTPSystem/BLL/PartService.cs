using BYSResults;
using OLTPSystem.DAL;
using OLTPSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLTPSystem.BLL
{
    public class PartService
    {
        #region Data Context Setup
        // The LINQPad auto-generated TypedDataContext instance used to query and manipulate data.
        private readonly OLTPContext _context;

        // The TypedDataContext provided by LINQPad for database access.
        // Store the injected context for use in library methods
        // NOTE:  This constructor is simular to the constuctor in your service
        public PartService(OLTPContext context)
        {
            _context = context
                        ?? throw new ArgumentNullException(nameof(context));
        }
        #endregion

        public Result<List<InvoicePartView>> GetParts()
        {
            var result = new Result<List<InvoicePartView>>();

            var parts = _context.Parts
                            .Where(x => !x.RemoveFromViewFlag)
                            .Select(p => new InvoicePartView
                            {
                                PartID = p.PartId,
                                PartCategoryID = p.PartCategoryId,
                                CategoryName = p.PartCategory.Name,
                                Description = p.Description,
                                Price = p.Price,
                                QOH = p.Qoh,
                                Taxable = p.Taxable
                            })
                            .OrderBy(p => p.CategoryName)
                            .ThenBy(p => p.Description)
                            .ToList();
            if (parts == null || parts.Count == 0)
            {
                result.AddError(new Error("No Records Found", "No parts were found"));
                return result;
            }

            return result.WithValue(parts);
        }
    }
}
