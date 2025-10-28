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
        private List<LookupView> provinces = new();
        private List<LookupView> countries = new();
        private List<LookupView> statuses = new();

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
        [Inject]
        protected LookupService LookupService { get; set; } = default!;
        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;
        //Must be included to show a dialogue in MudBlazor
        [Inject]
        protected IDialogService DialogService { get; set; } = default!;

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
                if (CustomerID != 0)
                {
                    //Get Customer
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


                //Populate Lookups
                //provinces
                var provinceResults = LookupService.GetLookupValues("Province");
                if (provinceResults.IsSuccess)
                    provinces = provinceResults.Value ?? [];
                else
                    errorDetails = BlazorHelperClass.GetErrorMessages(provinceResults.Errors.ToList());
                //countries
                var countryResults = LookupService.GetLookupValues("Country");
                if (countryResults.IsSuccess)
                    countries = countryResults.Value ?? [];
                else
                    errorDetails = BlazorHelperClass.GetErrorMessages(countryResults.Errors.ToList());
                //statuses
                var statusResults = LookupService.GetLookupValues("Customer Status");
                if (statusResults.IsSuccess)
                    statuses = statusResults.Value ?? [];
                else
                    errorDetails = BlazorHelperClass.GetErrorMessages(statusResults.Errors.ToList());
            }
            catch (Exception ex)
            {
                //capture any unexpected exceptions
                errorMessage = ex.Message;
            }
        }

        //Make any method that we want to show a dialogue with Async
        public async void Cancel()
        {
            if (hasFormChanged)
            {
                bool? result = await DialogService.ShowMessageBox(
                                    "Confirm Cancel",
                                    "Are you sure you want to cancel editing the customer? All unsaved changes will be lost",
                                    yesText: "Cancel",
                                    noText: "No");
                //results will be:
                //  - true if the user select the Yes button
                //  - false if the user selects the No button
                //  - null if the user dismiss the dialogue or select the cancel button (unused in this dialogue)
                // e.g. click the close button 'x'
                if (result != true)
                {
                    return;
                }
            }

            NavigationManager.NavigateTo("/SamplePages/Customers");
        }

        public void AddEditCustomer()
        {
            errorDetails.Clear();
            errorMessage = string.Empty;
            feedbackMessage = string.Empty;

            try
            {
                var result = CustomerService.AddEditCustomer(editCustomer);
                if (result.IsSuccess)
                {
                    editCustomer = result.Value ?? new();
                    if (editCustomer.CustomerID > 0)
                    {
                        feedbackMessage = "Customer was successfully saved!";

                        //reset trackers
                        hasFormChanged = false;
                        isFormValid = false;
                        customerForm.ResetTouched();
                    }
                }
                else
                    errorDetails = BlazorHelperClass.GetErrorMessages(result.Errors.ToList());
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }
        #endregion
    }
}
