using BYSResults;
using OLTPSystem.DAL;
using OLTPSystem.ViewModels;

namespace OLTPSystem.BLL
{
    public class LookupService
    {
        #region Data Context Setup
        // The LINQPad auto-generated TypedDataContext instance used to query and manipulate data.
        private readonly OLTPContext _context;

        // The TypedDataContext provided by LINQPad for database access.
        // Store the injected context for use in library methods
        // NOTE:  This constructor is simular to the constuctor in your service
        public LookupService(OLTPContext context)
        {
            _context = context
                        ?? throw new ArgumentNullException(nameof(context));
        }
        #endregion

        public Result<List<LookupView>> GetLookupValues(string categoryName)
        {
            var result = new Result<List<LookupView>>();
            //rule: categoryName must have a value
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                result.AddError(new Error("Missing Information", "Category Name must be provided."));
                return result;
            }

            var values = _context.Lookups
                .Where(x => x.Category.CategoryName.ToLower() == categoryName.ToLower()
                    && !x.RemoveFromViewFlag)
                .Select(x => new LookupView
                {
                    LookupID = x.LookupId,
                    Name = x.Name
                })
                .OrderBy(x => x.Name)
                .ToList();

            if (values.Count <= 0)
            {
                result.AddError(new Error("No Lookup Values", $"No lookup values found for the category name: {categoryName}"));
                return result;
            }

            return result.WithValue(values);
        }

        //Created an overloaded method so the user can search by Name or ID
        public Result<List<LookupView>> GetLookupValues(int categoryID)
        {
            var result = new Result<List<LookupView>>();
            //rule: categoryID must have a value
            if (categoryID <= 0)
            {
                result.AddError(new Error("Missing Information", "CategoryID must be provided."));
                return result;
            }

            var values = _context.Lookups
                .Where(x => x.CategoryId == categoryID
                    && !x.RemoveFromViewFlag)
                .Select(x => new LookupView
                {
                    LookupID = x.LookupId,
                    Name = x.Name
                })
                .OrderBy(x => x.Name)
                .ToList();

            if (values.Count <= 0)
            {
                result.AddError(new Error("No Lookup Values", $"No lookup values found for the categoryID: {categoryID}"));
                return result;
            }

            return result.WithValue(values);
        }

    }
}
