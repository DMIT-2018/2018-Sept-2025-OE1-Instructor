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
    public class CustomerService
{
	#region Data Context Setup
	// The LINQPad auto-generated TypedDataContext instance used to query and manipulate data.
	private readonly OLTPContext _context;

	// The TypedDataContext provided by LINQPad for database access.
	// Store the injected context for use in library methods
	// NOTE:  This constructor is simular to the constuctor in your service
	public CustomerService(OLTPContext context)
	{
		_context = context
					?? throw new ArgumentNullException(nameof(context));
	}
	#endregion

	//Always for this class be returning a Result<>
	//	The Result<> can be given a type (single record or a collection)
	//	The result type is what we will return if the method is successful
	public Result<List<CustomerSearchView>> GetCustomers(string lastName, string phone)
	{
		//Result is set up at the top of the method
		//	Give the result the same type as the Result return type
		var result = new Result<List<CustomerSearchView>>();
		
		//rule: Must have either a phone number or a last name
		if(string.IsNullOrWhiteSpace(lastName) && string.IsNullOrWhiteSpace(phone))
		{
			result.AddError(new Error("Missing Information", "Either a partial phone number or a last name must be provided"));
			//Since this is missing information, there is no point in continuing
			//	So we GTFO
			return result;
		}
		
		//rule: (Standard Rule) RemoveFromViewFlag cannot be true
		//	This is a typical rule to follow when returning data
		var customer = _context.Customers
			.Where(x => x.RemoveFromViewFlag == false
				&&
				(
					(!string.IsNullOrWhiteSpace(lastName)
						&&
					x.LastName.ToLower().Contains(lastName.ToLower())
					)
					||
					(!string.IsNullOrWhiteSpace(phone)
						&&
					x.Phone.Contains(phone)
					)
				)	
			)
			.Select(x => new CustomerSearchView
			{
				CustomerID = x.CustomerId,
				FullName = x.FirstName + " " + x.LastName,
				Address = x.Address1 + " " + x.City + ", " + x.ProvState.Name + " " + x.Country.Name + " " + x.PostalCode,
				Phone = x.Phone,
				Email = x.Email,
				Status = x.Status.Name,
				TotalSales = x.Invoices
								.Where(i => !i.RemoveFromViewFlag)
								.Sum(i => i.SubTotal + i.Tax)
			})
			.ToList();
			
		if(!customer.Any())
		{
			result.AddError(new Error("No Customer", "No customers were found"));
			return result;
		}
		
		//Happy path - we got customers and no business rules broken
		//Remember to return the happy path results WithValue!
		return result.WithValue(customer);
	}

	//Returning a single record for editing
	public Result<CustomerEditView> GetCustomer_ByID(int customerID)
	{
		//create the results first
		var result = new Result<CustomerEditView>();
		//rule: CustomerID must be valid
		if (customerID <= 0)
		{
			result.AddError(new Error("Missing Information", "A valid CustomerID must be provided."));
			//For parameter checks we just get out right away
			return result;
		}

		var customer = _context.Customers
			.Where(x => !x.RemoveFromViewFlag
				&& x.CustomerId == customerID)
			.Select(x => new CustomerEditView
			{
				CustomerID = x.CustomerId,
				FirstName = x.FirstName,
				LastName = x.LastName,
				Address1 = x.Address1,
				Address2 = x.Address2,
				City = x.City,
				ProvStateID = x.ProvStateId,
				CountryID = x.CountryId,
				PostalCode = x.PostalCode,
				Phone = x.Phone,
				Email = x.Email,
				StatusID = x.StatusId,
				RemoveFromViewFlag = x.RemoveFromViewFlag,
				OriginalFirstName = x.FirstName,
				HasInvoices = x.Invoices.Any(),
				Invoices = x.Invoices
					.Where(i => !i.RemoveFromViewFlag)
					.Select(i => new InvoiceListView
					{
						InvoiceID = i.InvoiceId,
						InvoiceDate = i.InvoiceDate,
						CustomerID = i.CustomerId,
						EmployeeID = i.EmployeeId,
						CustomerName = i.Customer.FirstName + " " + i.Customer.LastName,
						EmployeeName = i.Employee.FirstName + " " + i.Employee.LastName,
						Total = i.SubTotal + i.Tax
					}).ToList()
			})
			.FirstOrDefault();

		if (customer == null)
		{
			result.AddError(new Error("No Customer", $"No customer was found with ID: {customerID}"));
			return result;
		}

		return result.WithValue(customer);
	}
	
	//Because the validation for editing and adding is similar, we are making an AddEdit Method
	public Result<CustomerEditView> AddEditCustomer(CustomerEditView editCustomer)
	{
		var result = new Result<CustomerEditView>();
		
		//Rule: editCustomer cannot be null
		if(editCustomer == null)
		{
			result.AddError(new Error("Missing Information", "No customer was provided."));
			return result;
		}
		
		#region Business Logic
		//rule: Mandatory Fields were provided
		//	Check the database if fields have not null and no provided default then we 
		//	want to prevent the database directly giving the user an error.
		//We are to collect all errors, to tell the users everything that is 
		//	incorrect at one time.
		if(string.IsNullOrWhiteSpace(editCustomer.FirstName))
			result.AddError(new Error("Missing Information", "First Name is required."));
		if (string.IsNullOrWhiteSpace(editCustomer.LastName))
			result.AddError(new Error("Missing Information", "Last Name is required."));
		if (string.IsNullOrWhiteSpace(editCustomer.Address1))
			result.AddError(new Error("Missing Information", "Address1 is required."));
		if (string.IsNullOrWhiteSpace(editCustomer.City))
			result.AddError(new Error("Missing Information", "City is required."));
		if (string.IsNullOrWhiteSpace(editCustomer.PostalCode))
			result.AddError(new Error("Missing Information", "Postal Code is required."));
		if (string.IsNullOrWhiteSpace(editCustomer.Phone))
			result.AddError(new Error("Missing Information", "Phone is required."));
		if (string.IsNullOrWhiteSpace(editCustomer.Email))
			result.AddError(new Error("Missing Information", "Email is required."));
		if (editCustomer.ProvStateID <= 0)
			result.AddError(new Error("Missing Information", "A Province or State is required."));
		if (editCustomer.CountryID <= 0)
			result.AddError(new Error("Missing Information", "A Country is required."));
		if (editCustomer.StatusID <= 0)
			result.AddError(new Error("Missing Information", "A Customer Status is required."));
		
		//rule: Duplicate check
		//	FirstName, LastName, and Phone cannot match an existing record

		bool existingCustomer = _context.Customers
				.Any(x => x.FirstName.ToLower() == editCustomer.FirstName.ToLower()
					&& x.LastName.ToLower() == editCustomer.LastName.ToLower()
					&& x.Phone == editCustomer.Phone
					//This makes sure if the same customer exists in the database already with
					//	the existing ID then we can still edit that customer.
					&& x.CustomerId != editCustomer.CustomerID);
		
		if(existingCustomer)
		{
			result.AddError(new Error("Existing Customer Data", $"A customer with the name {editCustomer.FirstName} {editCustomer.LastName} and the phone number {editCustomer.Phone} already exists in the database and cannot be entered again."));
		}
		
		//check if there are any errors so far
		//	if there are return the errors and get out
		if(result.IsFailure)
			return result;
		#endregion

		//If no errors were found we can proceed to add or edit
		//First we check if there is an existingCustomer in the database
		//	We need the existing customer to edit the database record
		//	NOTE: You are returning the actual Database Type (Customer)
		Customer? customer = _context.Customers
			.Where(x => x.CustomerId == editCustomer.CustomerID)
			.FirstOrDefault();

		//Check if the results are null
		//	If null AND the provided record has an ID of 0
		//	we are creating a new customer record
		//	If the customerID is 0, then this is a new record
		//	If the customerID is not 0, then we are editing a record
		if(customer == null && editCustomer.CustomerID == 0)
		{
			//Create a new Customer record
			customer = new();
		}
		else if(customer == null && editCustomer.CustomerID != 0)
		{
			result.AddError(new Error("Cannot find Record to Edit", $"CustomerID {editCustomer.CustomerID} cannot be found, edits cannot be made."));
			return result;
		}
		
		//Now we have the existing customer or the new customer record
		//	If editing, we are just updating the database values
		//	If new, we are adding new values to the record
		//	Both of these steps are done the same way
		customer.FirstName = editCustomer.FirstName;
		customer.LastName = editCustomer.LastName;
		customer.Address1 = editCustomer.Address1;
		customer.Address2 = editCustomer.Address2;
		customer.City = editCustomer.City;
		customer.ProvStateId = editCustomer.ProvStateID ?? 0;
		customer.CountryId = editCustomer.CountryID ?? 0;
		customer.PostalCode = editCustomer.PostalCode;
		customer.Phone = editCustomer.Phone;
		customer.Email = editCustomer.Email;
		customer.StatusId = editCustomer.StatusID ?? 0;
		//This may be a logical delete in the edit
		//	We still can use the AddEdit method to logically delete
		//	We don't need a different method unless for specific reasons
		customer.RemoveFromViewFlag = editCustomer.RemoveFromViewFlag;
		
		//check again if it is a new customer
		//	THIS IS LOCAL ONLY
		//	NOTHING SAVES TO THE DATABASE YET
		if(customer.CustomerId == 0)
			//If new we add the record
			_context.Customers.Add(customer);
		else
			//if not update the record
			_context.Customers.Update(customer);
		
		//Now we want to save the record or changes to the database
		//We wrap this in a try/catch always so if there is an unexpected error
		//	we can tell the user and not just crash our app
		try
		{
			//Commit the change to the database (save them)
			_context.SaveChanges();
			//Use the method we already coded to return the newly add or editing record
			//NOTE: make sure to use the database entity ID field
			//	Why? If it was a new data entry, the editCustomer CustomerID will still be 0
			//	But the customer.CustomerID will have the new database assigned CustomerID
			//THIS IS ONE OF THE BIGGEST MISTAKE I SEE
			return GetCustomer_ByID(customer.CustomerId);
		}
		catch(Exception ex)
		{
			//if something goes wrong
			//	return error and clear the change tracker
			// NOTE: It is suuuuuppppper important to clear the change tracker
			//	If you do not, then any database change are still staged (Add or Update) and will continue to fail
			//	whenever you call this method again from the same page.
			_context.ChangeTracker.Clear();
			result.AddError(new Error("Error Saving Changes", ex.InnerException?.Message ?? string.Empty));
			return result;
		}
	}
}
}
