using Microsoft.AspNetCore.Components;
using OLTPSystem.BLL;
using OLTPSystem.ViewModels;

namespace BlazorWebApp.Components.Pages.SamplePages
{
    public partial class InvoiceEdit
    {
        #region Fields
        private InvoiceView invoice = new();
        private List<InvoicePartView> parts = [];
        private int? categoryID;
        private InvoicePartView? selectedPart;
        private int quantity;

         private string feedbackMessage = string.Empty;
        // collected error details.
        private List<string> errorDetails = new();
        // general error message.
        private string errorMessage = string.Empty;
        #endregion

        #region Parameters
        [Parameter]
        public int InvoiceID { get; set; }
        [Parameter]
        public int CustomerID { get; set; }
        [Parameter]
        public int EmployeeID { get; set; }
        #endregion

        #region Properties
        [Inject]
        public InvoiceService InvoiceService { get; set; } = default!;
        [Inject]
        public PartService PartService { get; set; } = default!;
        #endregion

        #region Methods
        protected override void OnInitialized()
        {
            errorDetails.Clear();
            errorMessage = string.Empty;
            feedbackMessage = String.Empty;

            try
            {
                var invoiceResults = InvoiceService.GetInvoice(InvoiceID, CustomerID, EmployeeID);
                if (invoiceResults.IsSuccess)
                    invoice = invoiceResults.Value ?? new();
                else
                    errorDetails = BlazorHelperClass.GetErrorMessages(invoiceResults.Errors.ToList());

                var partResults = PartService.GetParts();
                if(partResults.IsSuccess)
                    parts = partResults.Value ?? new();
                else
                    errorDetails = BlazorHelperClass.GetErrorMessages(partResults.Errors.ToList());
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        private void CategoryChanged(int? newCategoryID)
        {
            categoryID = newCategoryID.HasValue ? newCategoryID.Value : null;
            selectedPart = null;
            quantity = 0;
        }

        private void AddPart()
        {
            if(selectedPart != null && quantity != 0)
            {
                InvoiceLineView newLine = new()
                {
                    PartID = selectedPart.PartID,
                    PartDescription = selectedPart.Description,
                    Price = selectedPart.Price,
                    Quantity = quantity,
                    Taxable = selectedPart.Taxable,
                };
                invoice.InvoiceLines.Add(newLine);
                //reset the values
                categoryID = null;
                selectedPart = null;
                quantity = 0;
                UpdateTotals();
            }
        }
        private void QuantityEdited(InvoiceLineView changedLine, int quantity)
        {
            changedLine.Quantity = quantity;
            UpdateTotals();
        }

        private void UpdateTotals()
        {
            invoice.SubTotal = invoice.InvoiceLines
                                .Where(x => !x.RemoveFromViewFlag)
                                .Sum(x => x.ExtentPrice);
            invoice.Tax = invoice.InvoiceLines
                            .Where(x => !x.RemoveFromViewFlag)
                            .Sum(x => x.Taxable ? x.ExtentPrice * 0.05m : 0);
        }
        #endregion
    }
}
