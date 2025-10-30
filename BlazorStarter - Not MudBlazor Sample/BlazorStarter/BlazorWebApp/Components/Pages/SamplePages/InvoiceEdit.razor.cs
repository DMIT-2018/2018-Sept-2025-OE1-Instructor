using Microsoft.AspNetCore.Components;
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
    }
}
