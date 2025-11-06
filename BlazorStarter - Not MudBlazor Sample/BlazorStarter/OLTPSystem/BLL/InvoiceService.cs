using BYSResults;
using OLTPSystem.DAL;
using OLTPSystem.Entities;
using OLTPSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLTPSystem.BLL
{
    public class InvoiceService
{
	#region Data Context Setup
	// The LINQPad auto-generated TypedDataContext instance used to query and manipulate data.
	private readonly OLTPContext _context;

	// The TypedDataContext provided by LINQPad for database access.
	// Store the injected context for use in library methods
	// NOTE:  This constructor is simular to the constuctor in your service
	public InvoiceService(OLTPContext context)
	{
		_context = context
					?? throw new ArgumentNullException(nameof(context));
	}
	#endregion
	
	//This method will return an existing Invoice if found OR a new invoice (not saved to the database)
	//	When existing - will return the existing record as a InvoiceView (update the employee or customer
	//		as needed
	//	When new will return a InvoiceView with the customer and employee info provided.
	//		NOTE: This will not save the new invoice to the database!
	public Result<InvoiceView> GetInvoice(int invoiceID, int customerID, int employeeID)
	{
		var result = new Result<InvoiceView>();
		//rule: parameters are provided
		if(customerID == 0)
			result.AddError(new Error("Missing Information", "Customer ID must be provided."));
		if (employeeID == 0)
			result.AddError(new Error("Missing Information", "Employee ID must be provided."));
		//We will not check Invoice ID, because if it is 0 we will return a new invoice
		if(result.IsFailure)
			return result;
			
		//Handle both new and existing invoices
		//	For new invoices only the customer and employee id are needed
		InvoiceView? invoice = default!;
		//Check if the invoice is new and set up the default date as today and add the provided customer
		//	and employee ID.
		if(invoiceID == 0)
		{
			invoice = new InvoiceView
			{
				CustomerID = customerID,
				EmployeeID = employeeID,
				InvoiceDate = DateOnly.FromDateTime(DateTime.Now)
			};
		}
		else
		{
			invoice = _context.Invoices
				.Where(x => x.InvoiceId == invoiceID
					&& !x.RemoveFromViewFlag)
				.Select(x => new InvoiceView
				{
					InvoiceID = x.InvoiceId,
					//Using the provided customer and employee IDs
					CustomerID = customerID,
					EmployeeID = employeeID,
					InvoiceDate = x.InvoiceDate,
					SubTotal = x.SubTotal,
					Tax = x.Tax,
					RemoveFromViewFlag = x.RemoveFromViewFlag,
					//CustomerName = x.Customer.FirstName + " " + x.Customer.LastName,
					//EmployeeName = x.Employee.FirstName + " " + x.Employee.LastName,
					InvoiceLines = x.InvoiceLines
							.Where(i => !i.RemoveFromViewFlag)
							.Select(i => new InvoiceLineView
							{
								InvoiceLineID = i.InvoiceLineId,
								InvoiceID = i.InvoiceId,
								PartID = i.PartId,
								Price = i.Price,
								Quantity = i.Quantity,
								RemoveFromViewFlag = i.RemoveFromViewFlag,
								PartDescription = i.Part.Description,
								Taxable = i.Part.Taxable
							}).ToList()
				}).FirstOrDefault();
		}

		//We will populate the Employee and Customer Names after the LINQ query for 
		//	the rest of the information
		// ?? here means if the result of the query before the ?? is null, 
		//	use what is after the ?? as the value
		invoice.CustomerName = _context.Customers
				.Where(x => x.CustomerId == customerID)
				.Select(x => $"{x.FirstName} {x.LastName}").FirstOrDefault() ?? string.Empty;
		invoice.EmployeeName = _context.Employees
				.Where(x => x.EmployeeId == employeeID)
				.Select(x => $"{x.FirstName} {x.LastName}").FirstOrDefault() ?? string.Empty;
			
		//invoice could still be null if it didn't exist in the database or the invoice
		//	was logically deleted
		if(invoice == null)
		{
			result.AddError(new Error("No results", $"No invoice was found for the invoice ID {invoiceID}"));
			return result;
		}
		
		//NOTE: NOTHING WAS ADDED OR SAVED TO THE DATABASE IN THIS METHOD
		//	We only return an InvoiceView so we can edit or add to it
		//	Once all edits are done, we can add/edit in another method
		return result.WithValue(invoice);
	}
	
	public Result<InvoiceView> AddEditInvoice(InvoiceView invoiceView)
	{
		var result = new Result<InvoiceView>();
		
		//rule: parameters are provided
		if(invoiceView == null)
		{
			result.AddError(new Error("Missing Information", "An invoice must be provided."));
			return result;
		}
		
		#region Business Logic
		//rule: All required (not null) fields must be provided and valid
		if(invoiceView.CustomerID <= 0)
			result.AddError(new Error("Missing Information", "A valid customer ID is required."));
		if (invoiceView.EmployeeID <= 0)
			result.AddError(new Error("Missing Information", "A valid employee ID is required."));
		//Decimal fields we will not check because the default is 0 and an invoice could have no lines
		//	No lines or free items means the subtotal and tax could actually be 0
		
		//rule: there must be at least 1 invoice line
		if(invoiceView.InvoiceLines.Count == 0)
			result.AddError(new Error("Missing Information", "Invoice details are required."));
			
		//Since we have child records we need to check the child records as well!
		foreach(var invoiceLine in invoiceView.InvoiceLines)
		{
			//rule: for each invoice line, a part must be provided
			if(invoiceLine.PartID <= 0)
			{
				//hard stop, GTFO check
				//	We need a part ID for the remainder of our checks
				result.AddError(new Error("Missing Information", "Missing Part ID"));
				return result;
			}
			
			//rule: for each invoice line, the price must be greater than or equal to 0
			if(invoiceLine.Price < 0)
			{
				//Getting the Part Description to return a more user friendly message
				string partName = _context.Parts
						.Where(x => x.PartId == invoiceLine.PartID)
						.Select(x => x.Description)
						.FirstOrDefault() ?? string.Empty;
				result.AddError(new Error("Invalid Price", 
					$"Part {partName} has a price that is less than zero."));
			}
			
			//rule: for each invoice line, the quantity cannot be less than 1
			if(invoiceLine.Quantity < 1)
			{
				string partName = _context.Parts
						.Where(x => x.PartId == invoiceLine.PartID)
						.Select(x => x.Description)
						.FirstOrDefault();
				result.AddError(new Error("Invalid Quantity",
					$"Part {partName} has a quantity that is less than one."));
			}
		}

		//MAKE SURE YOU ARE OUTSIDE OF THE FOREACH LOOP BEFORE CHECKING
		//	ANYTHING GENERAL AGAIN
		//rule: parts cannot be duplicated for an invoice
		List<string?> duplicatedParts = invoiceView.InvoiceLines
			.GroupBy(x => x.PartID)
			//checking if the group has more than 1
			.Where(x => x.Count() > 1)
			//We cannot use navigational properties here because of the Group By
			//So we have to use a where clause in the nested query and go directly
			//	to the Parts table
			.Select(x => _context.Parts
				.Where(p => p.PartId == x.Key)
				.Select(p => p.Description)
				.FirstOrDefault()
			).ToList();
			
		if(duplicatedParts.Count > 0)
		{
			foreach(var partName in duplicatedParts)
			{
				result.AddError(new Error("Duplicate Invoice Line", $"Part {partName} can only be added to the invoice once."));
			}
		}
		
		//exit check if there are any errors
		if(result.IsFailure)
			return result;
		#endregion
		
		//Retrieve the actual database record (not as a ViewModel)
		Invoice? invoice = _context.Invoices
			.Where(x => x.InvoiceId == invoiceView.InvoiceID)
			.FirstOrDefault();
		//If the invoice doesn't exist, create it
		if(invoice == null && invoiceView.InvoiceID == 0)
		{
			invoice = new Invoice();
		}
		else if(invoice == null && invoiceView.InvoiceID != 0)
		{
			result.AddError(new Error("Invoice Not Found", $"An invoice with invoiceID {invoiceView.InvoiceID} could not be found to edit."));
			return result;
		}
			
		//Update the invoice properties from the supplied View Model
		invoice.CustomerId = invoiceView.CustomerID;
		invoice.EmployeeId = invoiceView.EmployeeID;
		invoice.InvoiceDate = invoiceView.InvoiceDate;
		//May be a delete, so update the logical delete flag
		invoice.RemoveFromViewFlag = invoiceView.RemoveFromViewFlag;
		
		//result the subtotal and tax as we need to update these values!
		invoice.SubTotal = 0;
		invoice.Tax = 0;
		
		//Loop through each child record to update or add them
		foreach(var invoiceLineView in invoiceView.InvoiceLines)
		{
			//Try and retrieve the child record from the database
			InvoiceLine? invoiceLine = _context.InvoiceLines
				.Where(x => x.InvoiceLineId == invoiceLineView.InvoiceLineID
						&& !x.RemoveFromViewFlag)
				.FirstOrDefault();
			//If the child record doesn't exist or had been previously deleted, create it.
			if(invoiceLine == null)
			{
				invoiceLine = new InvoiceLine();
				//We put the PartID update in the if() where we create a new record
				//	because we only change the identifying FK if it is a new record
				//	PartID would not get updated if the invoiceLine already exists
				invoiceLine.PartId = invoiceLineView.PartID;
			}
			//Update other properties from the ViewModel
			invoiceLine.Quantity = invoiceLineView.Quantity;
			invoiceLine.Price = invoiceLineView.Price;
			//Also update the logical delete flag in case it is flagged for deletion
			invoiceLine.RemoveFromViewFlag = invoiceLineView.RemoveFromViewFlag;
			
			//Inside the foreach loop update the subtotal and tax per record
			//	Only use it is the Subtotal and Tax calcs if the record isn't 
			//	logically deleted
			if(!invoiceLineView.RemoveFromViewFlag)
			{
				invoice.SubTotal += invoiceLine.Quantity * invoiceLine.Price;
				bool isTaxable = _context.Parts
						.Where(x => x.PartId == invoiceLine.PartId)
						.Select(x => x.Taxable)
						.FirstOrDefault();
				//Add the m after the tax amount to make it a decimal
				invoice.Tax += isTaxable ? invoiceLine.Quantity * invoiceLine.Price * 0.05m : 0;
			}
			
			//Check once more if it is existing or new
			//Remember if the ID is 0 it is new
			if(invoiceLine.InvoiceLineId == 0)
				//This is a child record so WE NEVER ADD DIRECTLY TO THE DATABASE
				//	We add the records to the collection of the parent record (navigational property)
				//	When the parent record is later saved to the database,
				//	the FK (invoiceID) of the parent records is automatically
				//	added to our new child record
				//	NOTE: If this was a NEW Invoice we have no idea what the InvoiceID is yet
				//		We cannot add this invoiceLine to the database without the FK
				invoice.InvoiceLines.Add(invoiceLine);
			else
				//Since it is an update, it already has the FK info it needs
				//	which means we can directly stage the update in the database
				_context.InvoiceLines.Update(invoiceLine);
		}
		
		//MAKE SURE YOU ARE OUTSIDE THE FOREACH LOOP
		//Once all child records are processed, you can check if the parent record
		//	is new or existing to stage our database changes
		if(invoice.InvoiceId == 0)
			_context.Invoices.Add(invoice);
		else
			_context.Invoices.Update(invoice); 
			
		//Always remember your try/catch
		try
		{
			_context.SaveChanges();
			//Use the previously programmed method to return the InvoiceView
			//Remember to use the database records for any values needed for this method
			return GetInvoice(invoice.InvoiceId, invoice.CustomerId, invoice.EmployeeId);
		}
		catch (Exception ex)
		{
			//Rollback any changes if there is an error
			_context.ChangeTracker.Clear();
			result.AddError(new Error("Error Saving Changes", ex.InnerException.Message));
			return result;
		}
	}
}
}
