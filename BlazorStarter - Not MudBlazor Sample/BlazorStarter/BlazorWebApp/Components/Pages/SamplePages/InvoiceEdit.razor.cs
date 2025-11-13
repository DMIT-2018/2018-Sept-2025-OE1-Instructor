using Microsoft.AspNetCore.Components;
using MudBlazor;
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
        private bool edited;
        private string randomMessage = string.Empty;

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
        [Inject]
        public IDialogService DialogService { get; set; } = default!;
        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;
        [Inject]
        private ISnackbar Snackbar { get; set; } = default!;
        #endregion

        #region Methods
        protected override void OnInitialized()
        {
            errorDetails.Clear();
            errorMessage = string.Empty;
            feedbackMessage = String.Empty;

            RandomTimerExample();

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

        private void RandomTimerExample()
        {
            System.Timers.Timer timer = new System.Timers.Timer(5000);
            timer.Start();
            timer.Elapsed += async (_, __) =>
            {
                randomMessage = "- It's Invoice Time!";
                await InvokeAsync(StateHasChanged);
            };

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
        private void PriceEdited(InvoiceLineView changedLine, decimal price)
        {
            changedLine.Price = price;
            UpdateTotals();
        }

        private void SyncPrice(InvoiceLineView invoiceLine)
        {
            //Find the original Part Price
            decimal originalPrice = parts.Where(x => x.PartID == invoiceLine.PartID).Select(x => x.Price).FirstOrDefault();
            invoiceLine.Price = originalPrice;
            UpdateTotals();
        }

        private async Task DeletePart(InvoiceLineView invoiceLine)
        {
            bool? results = await DialogService.ShowMessageBox("Confirm Delete", $"Are you sure you want to delete {invoiceLine.PartDescription} from the invoice?", yesText: "Delete", cancelText: "Cancel");
            if (results == true)
            {
                invoice.InvoiceLines.Remove(invoiceLine);
                UpdateTotals();
            }
        }

        private async Task DeleteInvoice()
        {
            bool? result = await DialogService.ShowMessageBox("Confirm Delete", $"Are you sure you want to delete Invoice No {invoice.InvoiceID}?", yesText: "Yes", noText: "No");
            if (result == true)
            {
                try
                {
                    invoice.RemoveFromViewFlag = true;
                    var invoiceResult = InvoiceService.DeleteInvoice(invoice.InvoiceID);
                    if(invoiceResult.IsSuccess)
                    {
                        Snackbar.Add($"Invoice No {invoice.InvoiceID} was successfully deleted.", severity: Severity.Success, config => { config.ShowCloseIcon = false; config.VisibleStateDuration = 7000; });
                        NavigationManager.NavigateTo($"/SamplePages/CustomerEdit/{CustomerID}");
                    }
                    else
                    {
                        errorDetails = BlazorHelperClass.GetErrorMessages(invoiceResult.Errors.ToList());
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                }
            }
        }

        private async Task CloseInvoice()
        {
            if(edited)
            {
                bool? result = await DialogService.ShowMessageBox("Confirm Cancel", "Are you sure you want to cancel editing the invoice? All unsaved changes will be lost.", yesText: "Cancel", noText: "No");
                if (result != true)
                {
                    return;
                }
            }
            NavigationManager.NavigateTo($"/SamplePages/CustomerEdit/{CustomerID}");
        }

        private void SaveInvoice()
        {
            errorDetails.Clear();
            errorMessage = string.Empty;
            feedbackMessage = string.Empty;

            bool isNewInvoice;

            try
            {
                var result = InvoiceService.AddEditInvoice(invoice);
                if(result.IsSuccess)
                {
                    isNewInvoice = invoice.InvoiceID == 0;
                    invoice = result.Value ?? new();
                    feedbackMessage = isNewInvoice
                        ? $"New Invoice No {invoice.InvoiceID} was created!"
                        : $"Invoice No {invoice.InvoiceID} was updated!";
                    edited = false;
                    //This is the least resource intensive StateHasChanged
                    //However, you often don't need to call StateHasChanged
                    //If you have 2-ways binding, Child Component Changes (example: error/feedback display), OnClick, OnValueChanges, or other event driven code
                    //  You do not need StateHasChanged!
                    // You only need StateHasChanged when you have a timer driven event with a background task, Service raising an event, Javascript is editing the DOM,
                    // There are some complex tasks that also need this, but in general it is not needed.
                    //await InvokeAsync(StateHasChanged);
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

        private void UpdateTotals()
        {
            invoice.SubTotal = invoice.InvoiceLines
                                .Where(x => !x.RemoveFromViewFlag)
                                .Sum(x => x.ExtentPrice);
            invoice.Tax = invoice.InvoiceLines
                            .Where(x => !x.RemoveFromViewFlag)
                            .Sum(x => x.Taxable ? x.ExtentPrice * 0.05m : 0);
            edited = true;
        }
        #endregion
    }
}
