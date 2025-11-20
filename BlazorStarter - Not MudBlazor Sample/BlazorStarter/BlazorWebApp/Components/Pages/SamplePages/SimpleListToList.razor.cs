using Microsoft.AspNetCore.Components;
using OLTPSystem.BLL;
using OLTPSystem.ViewModels;

namespace BlazorWebApp.Components.Pages.SamplePages
{
    public partial class SimpleListToList
    {
        #region fields
        private List<InvoicePartView> inventory = [];
        private List<InvoiceLineView> shoppingCart = [];
        private string errorMessage = string.Empty;
        private List<string> errorDetails = [];
        private string feedbackMessage = string.Empty;
        #endregion

        #region Properties
        [Inject]
        public PartService PartService { get; set; } = default!;
        #endregion

        #region Methods
        protected override void OnInitialized()
        {
            try
            {
                var result = PartService.GetParts();
                if(result.IsSuccess)
                {
                    inventory = result.Value ?? [];
                }
                else
                {
                    errorDetails = BlazorHelperClass.GetErrorMessages(result.Errors.ToList());
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        private void AddPartToCart(InvoicePartView part)
        {
            if(part != null)
            {
                shoppingCart.Add(new InvoiceLineView
                {
                    PartID = part.PartID,
                    PartDescription = part.Description,
                    Price = part.Price,
                    Quantity = 1,
                    Taxable = part.Taxable
                });
            }
        }
        private void RemoveFromCart(InvoiceLineView invoiceLineView)
        {
            shoppingCart.Remove(invoiceLineView);
        }
        #endregion
    }
}
