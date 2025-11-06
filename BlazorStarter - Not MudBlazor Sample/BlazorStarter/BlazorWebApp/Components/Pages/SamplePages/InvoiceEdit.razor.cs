using Microsoft.AspNetCore.Components;
using OLTPSystem.BLL;
using OLTPSystem.ViewModels;

namespace BlazorWebApp.Components.Pages.SamplePages
{
    public partial class InvoiceEdit
    {
        #region Fields
        public InvoiceView invoice = new();

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
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }
        #endregion
    }
}
