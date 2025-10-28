using Microsoft.AspNetCore.Components;
using OLTPSystem.BLL;
using OLTPSystem.ViewModels;

namespace BlazorWebApp.Components.Pages.SamplePages
{
    public partial class Customers
    {
        #region Fields
        private string lastName = string.Empty;
        private string phoneNumber = string.Empty;

        private string feedbackMessage = string.Empty;
        // collected error details.
        private List<string> errorDetails = new();
        // general error message.
        private string errorMessage = string.Empty;
        private List<CustomerSearchView> CustomerList = [];
        #endregion

        #region Properties
        [Inject]
        protected CustomerService CustomerService { get; set; } = default!;
        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;
        private bool hasError => !string.IsNullOrEmpty(errorMessage) || errorDetails.Any();
        #endregion

        #region Methods
        //When we moved from LINQPAD we can remove the parameters since the
        //	fields are on this page
        public void GetCustomers()
        {
            //Always start by clearing messages (feedback and errors)
            //	You start with this so you don't have repeated messages
            errorDetails.Clear();
            errorMessage = string.Empty; //I use string.Empty and not "" because it is smaller
            feedbackMessage = string.Empty;

            //Wrap the call to any service method in a try/catch
            try
            {
                //change the reference to the Page Fields, not referencing any method parameters
                var results = CustomerService.GetCustomers(lastName, phoneNumber);
                if (results.IsSuccess)
                    CustomerList = results.Value ?? [];
                else
                    errorDetails = BlazorHelperClass.GetErrorMessages(results.Errors.ToList());
            }
            catch (Exception ex)
            {
                //capture any unexpected exceptions
                errorMessage = ex.Message;
            }
        }

        public void EditCustomer(int customerID)
        {
            NavigationManager.NavigateTo($"/SamplePages/CustomerEdit/{customerID}");
        }
        public void NewCustomer()
        {
            NavigationManager.NavigateTo("/SamplePages/CustomerEdit/0");
        }
        #endregion
    }
}
