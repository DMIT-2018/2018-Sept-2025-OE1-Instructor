using Microsoft.AspNetCore.Components;
using MudBlazor;
using OLTPSystem.BLL;
using OLTPSystem.Entities;
using OLTPSystem.ViewModels;

namespace BlazorWebApp.Components.Pages.SamplePages
{
    public partial class CustomerEdit
    {
        #region Fields
        private CustomerEditView editCustomer = new();

        private string feedbackMessage = string.Empty;
        // collected error details.
        private List<string> errorDetails = new();
        // general error message.
        private string errorMessage = string.Empty;

        //Add this to the @ref for the form to now reference the form
        //  in our C# code
        private MudForm customerForm = new();
        //Add bool to bind to the form to see if all fields have valid values
        private bool isFormValid;
        //Add bool to bind to the form to see if the form has changed
        private bool hasFormChanged;
        #endregion

        #region Properties
        //Make sure we name this the same as in the page directive (capitalization matters!)
        [Parameter]
        public int CustomerID { get; set; }
        [Inject]
        protected CustomerService CustomerService { get; set; } = default!;

        private bool hasError => !string.IsNullOrEmpty(errorMessage) || errorDetails.Any();
        private string closeButtonText => hasFormChanged ? "Cancel" : "Close";
        #endregion

        #region Methods
        protected override void OnInitialized()
        {
            //clear any errors or feedback
            errorDetails.Clear();
            errorMessage = string.Empty;
            feedbackMessage = string.Empty;

            //Wrap the call to any service method in a try/catch
            try
            {
                var results = CustomerService.GetCustomer_ByID(CustomerID);
                if (results.IsSuccess)
                {
                    //Add a nullable default with ?? 
                    //  if results.Value is null set editCustomer to a new instance of the CustomerEditView
                    editCustomer = results.Value ?? new();
                }
                else
                    errorDetails = BlazorHelperClass.GetErrorMessages(results.Errors.ToList());
            }
            catch (Exception ex)
            {
                //capture any unexpected exceptions
                errorMessage = ex.Message;
            }
        }
        #endregion
    }
}
