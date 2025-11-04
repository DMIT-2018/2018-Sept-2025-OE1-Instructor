<Query Kind="Program">
  <Connection>
    <ID>fb35ac00-831f-498a-a4b8-24efd7072c36</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Driver Assembly="(internal)" PublicKeyToken="no-strong-name">LINQPad.Drivers.EFCore.DynamicDriver</Driver>
    <AllowDateOnlyTimeOnly>true</AllowDateOnlyTimeOnly>
    <Database>OLTP-DMIT2018</Database>
    <Server>MOMSDESKTOP\SQLEXPRESS</Server>
    <DriverData>
      <EncryptSqlTraffic>True</EncryptSqlTraffic>
      <PreserveNumeric1>True</PreserveNumeric1>
      <EFProvider>Microsoft.EntityFrameworkCore.SqlServer</EFProvider>
    </DriverData>
  </Connection>
  <NuGetReference>BYSResults</NuGetReference>
</Query>

// 	Lightweight result types for explicit success/failure 
//	 handling in .NET applications.
using BYSResults;


void Main()
{
	CodeBehind codeBehind = new CodeBehind(this); // “this” is LINQPad’s auto Context

	//Change to true to see the resulting record
	bool verbose = false;

	//Fail Tests
	//Rule: either last name or phone must be provided
	//Simple Error Check
	//codeBehind.GetCustomers(string.Empty, string.Empty);
	//codeBehind.ErrorDetails.Dump("Fail Expected - Either a partial phone number or a last name must be provided");

	//Coloured Testing
	//strings are empty
	#region GetCustomers Tests
	"=============== GetCustomers Tests ===================".Dump();
	codeBehind.GetCustomers(string.Empty, string.Empty);
	if (codeBehind.ErrorDetails.Any(x => x == "Missing Information: Either a partial phone number or a last name must be provided"))
	{
		Util.WithStyle("Pass - (Fail Expected) Empty Last Name and Phone Number", "color:LimeGreen").Dump();
	}
	else
	{
		Util.WithStyle("Fail - No error or incorrect error for Empty Last Name and Phone", "color:OrangeRed;font-weight:bold").Dump();
	}
	if (verbose)
		codeBehind.ErrorDetails.Dump();

	//strings are null
	codeBehind.GetCustomers(null, null);
	if (codeBehind.ErrorDetails.Any(x => x == "Missing Information: Either a partial phone number or a last name must be provided"))
	{
		Util.WithStyle("Pass - (Fail Expected) Null Last Name and Phone Number", "color:LimeGreen").Dump();
	}
	else
	{
		Util.WithStyle("Fail - No error or incorrect error for Null Last Name and Phone", "color:OrangeRed;font-weight:bold").Dump();
	}
	if (verbose)
		codeBehind.ErrorDetails.Dump();

	//strings are white space
	codeBehind.GetCustomers("     ", "    ");
	if (codeBehind.ErrorDetails.Any(x => x == "Missing Information: Either a partial phone number or a last name must be provided"))
	{
		Util.WithStyle("Pass - (Fail Expected) White space Last Name and Phone Number", "color:LimeGreen").Dump();
	}
	else
	{
		Util.WithStyle("Fail - No error or incorrect error for White space Last Name and Phone", "color:OrangeRed;font-weight:bold").Dump();
	}
	if (verbose)
		codeBehind.ErrorDetails.Dump();

	//Check: No records are returned
	codeBehind.GetCustomers("zzzzzz", "9999999");
	if (codeBehind.ErrorDetails.Any(x => x.Contains("No customers were found")))
	{
		Util.WithStyle("Pass - (Fail Expected) No customers found for provided values", "color:LimeGreen").Dump();
	}
	else
	{
		Util.WithStyle("Fail - No error or incorrect error for provided values, expected no customer were found", "color:OrangeRed;font-weight:bold").Dump();
	}
	if (verbose)
		codeBehind.ErrorDetails.Dump();

	//Pass Check - Returned Values by Last Name
	codeBehind.GetCustomers("Foster", "");
	if (codeBehind.Customers.Count == 14)
	{
		Util.WithStyle("Pass - Last Name search returned the correct number of results.", "color:LimeGreen").Dump();
	}
	else
	{
		Util.WithStyle("Fail - Last name returned the incorrect number of results or an error was returned.", "color:OrangeRed;font-weight:bold").Dump();
		if (verbose)
			codeBehind.ErrorDetails.Dump();
	}
	if (verbose)
		codeBehind.Customers.Dump();


	// Pass Check - Returned Values by exact phone
	codeBehind.GetCustomers("", "7804326565");
	if (codeBehind.Customers.Count == 1)
	{
		Util.WithStyle("Pass - Exact Phone search returned the correct number of results.", "color:LimeGreen").Dump();
	}
	else
	{
		Util.WithStyle("Fail - Exact phone returned the incorrect number of results or an error was returned.", "color:OrangeRed;font-weight:bold").Dump();
		if (verbose)
			codeBehind.ErrorDetails.Dump();
	}
	if (verbose)
		codeBehind.Customers.Dump();
	#endregion

	#region GetCustomer_ByID Tests
	"=============== Customer By ID Tests ===================".Dump();
	codeBehind.GetCustomer_ByID(0);
	if (codeBehind.ErrorDetails.Any(x => x.Contains("A valid CustomerID must be provided")))
	{
		Util.WithStyle("Pass - No customer was returned for the input 0.", "color:LimeGreen").Dump();
	}
	else
	{
		Util.WithStyle("Fail - No error or the incorrect error was thrown for 0 input.", "color:OrangeRed;font-weight:bold").Dump();
		if (verbose)
			codeBehind.EditCustomer.Dump();
	}
	if (verbose)
		codeBehind.ErrorDetails.Dump();

	codeBehind.GetCustomer_ByID(-2);
	if (codeBehind.ErrorDetails.Any(x => x.Contains("A valid CustomerID must be provided")))
	{
		Util.WithStyle("Pass - No customer was returned for the input -2.", "color:LimeGreen").Dump();
	}
	else
	{
		Util.WithStyle("Fail - No error or the incorrect error was thrown for -2 input.", "color:OrangeRed;font-weight:bold").Dump();
		if (verbose)
			codeBehind.EditCustomer.Dump();
	}
	if (verbose)
		codeBehind.ErrorDetails.Dump();

	//Good Scenario
	codeBehind.GetCustomer_ByID(6);
	if (codeBehind.EditCustomer != null
			&& codeBehind.EditCustomer.FirstName == "Fred"
			&& codeBehind.EditCustomer.LastName == "Foster")
	{
		Util.WithStyle("Pass - CustomerID 6 returned the correct customer.", "color:LimeGreen").Dump();
	}
	else
	{
		Util.WithStyle("Fail - Exact phone returned the incorrect Customer or an error was returned.", "color:OrangeRed;font-weight:bold").Dump();
		if (verbose)
			codeBehind.ErrorDetails.Dump();
	}
	if (verbose)
		codeBehind.EditCustomer.Dump();
	#endregion

	#region Lookup Tests
	"=============== Lookup Tests ===================".Dump();
	codeBehind.GetLookupValues(0);
	if (codeBehind.ErrorDetails.Any(x => x.Contains("CategoryID must be provided.")))
	{
		Util.WithStyle("Pass - No lookups were returned for the input 0.", "color:LimeGreen").Dump();
	}
	else
	{
		Util.WithStyle("Fail - No error or the incorrect error was thrown for 0 input.", "color:OrangeRed;font-weight:bold").Dump();
		if (verbose)
			codeBehind.Lookups.Dump();
	}
	if (verbose)
		codeBehind.ErrorDetails.Dump();
	codeBehind.GetLookupValues(string.Empty);
	if (codeBehind.ErrorDetails.Any(x => x.Contains("Category Name must be provided.")))
	{
		Util.WithStyle("Pass - No lookups were returned for an empty string.", "color:LimeGreen").Dump();
	}
	else
	{
		Util.WithStyle("Fail - No error or the incorrect error was thrown for an empty string.", "color:OrangeRed;font-weight:bold").Dump();
		if (verbose)
			codeBehind.Lookups.Dump();
	}
	if (verbose)
		codeBehind.ErrorDetails.Dump();

	//Good Scenarios
	codeBehind.GetLookupValues("Country");
	if (codeBehind.Lookups.Count == 3
		&& codeBehind.Lookups[0].Name == "Canada")
	{
		Util.WithStyle("Pass - Lookup Category Country returned 3 results correctly.", "color:LimeGreen").Dump();
	}
	else
	{
		Util.WithStyle("Fail - Lookup Category Name returned the incorrect results or an error", "color:OrangeRed;font-weight:bold").Dump();
		if (verbose)
			codeBehind.ErrorDetails.Dump();
	}
	if (verbose)
		codeBehind.Lookups.Dump();

	codeBehind.GetLookupValues(3);
	if (codeBehind.Lookups.Count == 4
		&& codeBehind.Lookups[0].Name == "Bronze")
	{
		Util.WithStyle("Pass - Lookup CategoryID 3 returned 4 results correctly.", "color:LimeGreen").Dump();
	}
	else
	{
		Util.WithStyle("Fail - Lookup CategoryID 3 returned the incorrect results or an error", "color:OrangeRed;font-weight:bold").Dump();
		if (verbose)
			codeBehind.ErrorDetails.Dump();
	}
	if (verbose)
		codeBehind.Lookups.Dump();
	#endregion
}

// ———— PART 2: Code Behind → Code Behind Method ————
// This region contains methods used to test the functionality
// of the application's business logic and ensure correctness.
// NOTE: This class functions as the code-behind for your Blazor pages
#region Code Behind Methods
public class CodeBehind(TypedDataContext context)
{
	#region Supporting Members (Do not modify)
	// exposes the collected error details
	public List<string> ErrorDetails => errorDetails;

	// Mock injection of the service into our code-behind.
	// You will need to refactor this for proper dependency injection.
	// NOTE: The TypedDataContext must be passed in.
	private readonly CustomerService CustomerService = new CustomerService(context);
	private readonly LookupService LookupService = new LookupService(context);
	#endregion

	#region Fields from Blazor Page Code-Behind
	// feedback message to display to the user.
	private string feedbackMessage = string.Empty;
	// collected error details.
	private List<string> errorDetails = new();
	// general error message.
	private string errorMessage = string.Empty;
	#endregion

	//Need to create a variable to hold any returned values
	//default sets the value to the data type default
	//	! says to the code (compiler), ignore any nulls, I don't care
	//	Note: Never use ! unless you know it does not matter!
	//		This means at this point is your education/careers,
	//		don't use it unless you ask or have been told to use it!
	public List<CustomerSearchView> Customers = default!;
	public CustomerEditView EditCustomer = default!;
	public List<LookupView> Lookups = default!;

	//Same name and the same parameters as the method in your library (service)
	public void GetCustomers(string lastName, string phone)
	{
		//Always start by clearing messages (feedback and errors)
		//	You start with this so you don't have repeated messages
		errorDetails.Clear();
		errorMessage = string.Empty; //I use string.Empty and not "" because it is smaller
		feedbackMessage = string.Empty;

		//Wrap the call to any service method in a try/catch
		try
		{
			var results = CustomerService.GetCustomers(lastName, phone);
			if (results.IsSuccess)
				Customers = results.Value;
			else
				errorDetails = GetErrorMessages(results.Errors.ToList());
		}
		catch (Exception ex)
		{
			//capture any unexpected exceptions
			errorMessage = ex.Message;
		}
	}
	public void GetCustomer_ByID(int customerID)
	{
		//Always start by clearing messages (feedback and errors)
		//	You start with this so you don't have repeated messages
		errorDetails.Clear();
		errorMessage = string.Empty;
		feedbackMessage = string.Empty;

		//Wrap the call to any service method in a try/catch
		try
		{
			var results = CustomerService.GetCustomer_ByID(customerID);
			if (results.IsSuccess)
				EditCustomer = results.Value;
			else
				errorDetails = GetErrorMessages(results.Errors.ToList());
		}
		catch (Exception ex)
		{
			//capture any unexpected exceptions
			errorMessage = ex.Message;
		}
	}
	public void GetLookupValues(int categoryID)
	{
		errorDetails.Clear();
		errorMessage = string.Empty;
		feedbackMessage = string.Empty;

		try
		{
			var results = LookupService.GetLookupValues(categoryID);
			if (results.IsSuccess)
				Lookups = results.Value;
			else
				errorDetails = GetErrorMessages(results.Errors.ToList());
		}
		catch (Exception ex)
		{
			//capture any unexpected exceptions
			errorMessage = ex.Message;
		}
	}
	//Can also overload in the codebehind
	public void GetLookupValues(string categoryName)
	{
		errorDetails.Clear();
		errorMessage = string.Empty;
		feedbackMessage = string.Empty;

		try
		{
			var results = LookupService.GetLookupValues(categoryName);
			if (results.IsSuccess)
				Lookups = results.Value;
			else
				errorDetails = GetErrorMessages(results.Errors.ToList());
		}
		catch (Exception ex)
		{
			//capture any unexpected exceptions
			errorMessage = ex.Message;
		}
	}

	public void AddEditCustomer(CustomerEditView editCustomer)
	{
		errorDetails.Clear();
		errorMessage = string.Empty;
		feedbackMessage = string.Empty;

		try
		{
			var result = CustomerService.AddEditCustomer(editCustomer);
			if (result.IsSuccess)
				EditCustomer = result.Value;
			else
				errorDetails = GetErrorMessages(result.Errors.ToList());
		}
		catch (Exception ex)
		{
			errorMessage = ex.Message;
		}
	}
}
#endregion

// ———— PART 3: Database Interaction Method → Service Library Method ————
//	This region contains support methods for testing
#region Methods
public class CustomerService
{
	#region Data Context Setup
	// The LINQPad auto-generated TypedDataContext instance used to query and manipulate data.
	private readonly TypedDataContext _context;

	// The TypedDataContext provided by LINQPad for database access.
	// Store the injected context for use in library methods
	// NOTE:  This constructor is simular to the constuctor in your service
	public CustomerService(TypedDataContext context)
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
		if (string.IsNullOrWhiteSpace(lastName) && string.IsNullOrWhiteSpace(phone))
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
				CustomerID = x.CustomerID,
				FullName = x.FirstName + " " + x.LastName,
				Address = x.Address1 + " " + x.City + ", " + x.ProvState.Name + " " + x.Country.Name + " " + x.PostalCode,
				Phone = x.Phone,
				Email = x.Email,
				Status = x.Status.Name,
				TotalSales = x.Invoices.Sum(i => i.SubTotal + i.Tax)
			})
			.ToList();

		if (!customer.Any())
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
				&& x.CustomerID == customerID)
			.Select(x => new CustomerEditView
			{
				CustomerID = x.CustomerID,
				FirstName = x.FirstName,
				LastName = x.LastName,
				Address1 = x.Address1,
				Address2 = x.Address2,
				City = x.City,
				ProvStateID = x.ProvStateID,
				CountryID = x.CountryID,
				PostalCode = x.PostalCode,
				Phone = x.Phone,
				Email = x.Email,
				StatusID = x.StatusID,
				RemoveFromViewFlag = x.RemoveFromViewFlag,
				OriginalFirstName = x.FirstName,
				HasInvoices = x.Invoices.Any(),
				Invoices = x.Invoices.Select(i => new InvoiceListView
				{
					InvoiceID = i.InvoiceID,
					InvoiceDate = i.InvoiceDate,
					CustomerID = i.CustomerID,
					EmployeeID = i.EmployeeID,
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
		if (editCustomer == null)
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
		if (string.IsNullOrWhiteSpace(editCustomer.FirstName))
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
					&& x.CustomerID != editCustomer.CustomerID);

		if (existingCustomer)
		{
			result.AddError(new Error("Existing Customer Data", $"A customer with the name {editCustomer.FirstName} {editCustomer.LastName} and the phone number {editCustomer.Phone} already exists in the database and cannot be entered again."));
		}

		//check if there are any errors so far
		//	if there are return the errors and get out
		if (result.IsFailure)
			return result;
		#endregion

		//If no errors were found we can proceed to add or edit
		//First we check if there is an existingCustomer in the database
		//	We need the existing customer to edit the database record
		//	NOTE: You are returning the actual Database Type (Customer)
		Customer customer = _context.Customers
			.Where(x => x.CustomerID == editCustomer.CustomerID)
			.FirstOrDefault();

		//Check if the results are null
		//	If null AND the provided record has an ID of 0
		//	we are creating a new customer record
		//	If the customerID is 0, then this is a new record
		//	If the customerID is not 0, then we are editing a record
		if (customer == null && editCustomer.CustomerID == 0)
		{
			//Create a new Customer record
			customer = new();
		}
		else
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
		customer.ProvStateID = editCustomer.ProvStateID;
		customer.CountryID = editCustomer.CountryID;
		customer.PostalCode = editCustomer.PostalCode;
		customer.Phone = editCustomer.Phone;
		customer.Email = editCustomer.Email;
		customer.StatusID = editCustomer.StatusID;
		//This may be a logical delete in the edit
		//	We still can use the AddEdit method to logically delete
		//	We don't need a different method unless for specific reasons
		customer.RemoveFromViewFlag = editCustomer.RemoveFromViewFlag;

		//check again if it is a new customer
		//	THIS IS LOCAL ONLY
		//	NOTHING SAVES TO THE DATABASE YET
		if (customer.CustomerID == 0)
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
			return GetCustomer_ByID(customer.CustomerID);
		}
		catch (Exception ex)
		{
			//if something goes wrong
			//	return error and clear the change tracker
			// NOTE: It is suuuuuppppper important to clear the change tracker
			//	If you do not, then any database change are still staged (Add or Update) and will continue to fail
			//	whenever you call this method again from the same page.
			_context.ChangeTracker.Clear();
			result.AddError(new Error("Error Saving Changes", ex.InnerException.Message));
			return result;
		}
	}
}

//If access a different and not directly related (not a child record) create a different service
public class LookupService
{
	#region Data Context Setup
	// The LINQPad auto-generated TypedDataContext instance used to query and manipulate data.
	private readonly TypedDataContext _context;

	// The TypedDataContext provided by LINQPad for database access.
	// Store the injected context for use in library methods
	// NOTE:  This constructor is simular to the constuctor in your service
	public LookupService(TypedDataContext context)
	{
		_context = context
					?? throw new ArgumentNullException(nameof(context));
	}
	#endregion

	public Result<List<LookupView>> GetLookupValues(string categoryName)
	{
		var result = new Result<List<LookupView>>();
		//rule: categoryName must have a value
		if (string.IsNullOrWhiteSpace(categoryName))
		{
			result.AddError(new Error("Missing Information", "Category Name must be provided."));
			return result;
		}

		var values = _context.Lookups
			.Where(x => x.Category.CategoryName.ToLower() == categoryName.ToLower()
				&& !x.RemoveFromViewFlag)
			.Select(x => new LookupView
			{
				LookupID = x.LookupID,
				Name = x.Name
			})
			.OrderBy(x => x.Name)
			.ToList();

		if (values.Count <= 0)
		{
			result.AddError(new Error("No Lookup Values", $"No lookup values found for the category name: {categoryName}"));
			return result;
		}

		return result.WithValue(values);
	}

	//Created an overloaded method so the user can search by Name or ID
	public Result<List<LookupView>> GetLookupValues(int categoryID)
	{
		var result = new Result<List<LookupView>>();
		//rule: categoryID must have a value
		if (categoryID <= 0)
		{
			result.AddError(new Error("Missing Information", "CategoryID must be provided."));
			return result;
		}

		var values = _context.Lookups
			.Where(x => x.CategoryID == categoryID
				&& !x.RemoveFromViewFlag)
			.Select(x => new LookupView
			{
				LookupID = x.LookupID,
				Name = x.Name
			})
			.OrderBy(x => x.Name)
			.ToList();

		if (values.Count <= 0)
		{
			result.AddError(new Error("No Lookup Values", $"No lookup values found for the categoryID: {categoryID}"));
			return result;
		}

		return result.WithValue(values);
	}

}

public class InvoiceService
{
	#region Data Context Setup
	// The LINQPad auto-generated TypedDataContext instance used to query and manipulate data.
	private readonly TypedDataContext _context;

	// The TypedDataContext provided by LINQPad for database access.
	// Store the injected context for use in library methods
	// NOTE:  This constructor is simular to the constuctor in your service
	public InvoiceService(TypedDataContext context)
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
		InvoiceView invoice = default!;
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
				.Where(x => x.InvoiceID == invoiceID
					&& !x.RemoveFromViewFlag)
				.Select(x => new InvoiceView
				{
					InvoiceID = x.InvoiceID,
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
								InvoiceLineID = i.InvoiceLineID,
								InvoiceID = i.InvoiceID,
								PartID = i.PartID,
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
				.Where(x => x.CustomerID == customerID)
				.Select(x => $"{x.FirstName} {x.LastName}").FirstOrDefault() ?? string.Empty;
		invoice.EmployeeName = _context.Employees
				.Where(x => x.EmployeeID == employeeID)
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
						.Where(x => x.PartID == invoiceLine.PartID)
						.Select(x => x.Description)
						.FirstOrDefault();
				result.AddError(new Error("Invalid Price", 
					$"Part {partName} has a price that is less than zero."));
			}
			
			//rule: for each invoice line, the quantity cannot be less than 1
			if(invoiceLine.Quantity < 1)
			{
				string partName = _context.Parts
						.Where(x => x.PartID == invoiceLine.PartID)
						.Select(x => x.Description)
						.FirstOrDefault();
				result.AddError(new Error("Invalid Quantity",
					$"Part {partName} has a quantity that is less than one."));
			}
		}

		//MAKE SURE YOU ARE OUTSIDE OF THE FOREACH LOOP BEFORE CHECKING
		//	ANYTHING GENERAL AGAIN
		//rule: parts cannot be duplicated for an invoice
		List<string> duplicatedParts = invoiceView.InvoiceLines
			.GroupBy(x => x.PartID)
			//checking if the group has more than 1
			.Where(x => x.Count() > 1)
			//We cannot use navigational properties here because of the Group By
			//So we have to use a where clause in the nested query and go directly
			//	to the Parts table
			.Select(x => _context.Parts
				.Where(p => p.PartID == x.Key)
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
		Invoice invoice = _context.Invoices
			.Where(x => x.InvoiceID == invoiceView.InvoiceID)
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
		invoice.CustomerID = invoiceView.CustomerID;
		invoice.EmployeeID = invoiceView.EmployeeID;
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
			InvoiceLine invoiceLine = _context.InvoiceLines
				.Where(x => x.InvoiceLineID == invoiceLineView.InvoiceLineID
						&& !x.RemoveFromViewFlag)
				.FirstOrDefault();
			//If the child record doesn't exist or had been previously deleted, create it.
			if(invoiceLine == null)
			{
				invoiceLine = new InvoiceLine();
				//We put the PartID update in the if() where we create a new record
				//	because we only change the identifying FK if it is a new record
				//	PartID would not get updated if the invoiceLine already exists
				invoiceLine.PartID = invoiceLineView.PartID;
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
						.Where(x => x.PartID == invoiceLine.PartID)
						.Select(x => x.Taxable)
						.FirstOrDefault();
				//Add the m after the tax amount to make it a decimal
				invoice.Tax += isTaxable ? invoiceLine.Quantity * invoiceLine.Price * 0.05m : 0;
			}
			
			//Check once more if it is existing or new
			//Remember if the ID is 0 it is new
			if(invoiceLine.InvoiceLineID == 0)
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
		if(invoice.InvoiceID == 0)
			_context.Invoices.Add(invoice);
		else
			_context.Invoices.Update(invoice); 
			
		//Always remember your try/catch
		try
		{
			_context.SaveChanges();
			//Use the previously programmed method to return the InvoiceView
			//Remember to use the database records for any values needed for this method
			return GetInvoice(invoice.InvoiceID, invoice.CustomerID, invoice.EmployeeID);
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
	#endregion

	// ———— PART 4: View Models → Service Library View Model ————
//	This region includes the view models used to 
//	represent and structure data for the UI.
#region View Models
public class CustomerSearchView
{
	//Building the Class to cut down and only return
	//	the information we need to display the required data

	//Almost ALWAYS have a reference of the Primary Key(s)
	//	This is used to make sure we are looking at the right record
	public int CustomerID { get; set; }
	//Calculated Field - Only a string but it doesn't 
	//	exist in the database
	public string FullName { get; set; } = string.Empty; //All Strings must default to an empty string
														 //Calculated Field
	public string Address { get; set; } = string.Empty;
	public string Phone { get; set; } = string.Empty;
	public string Email { get; set; } = string.Empty;
	//Calculated Field
	public string Status { get; set; } = string.Empty;
	//Calculated Field
	public decimal TotalSales { get; set; }
}

//We need to get the Customer for Editing differently then for lists or the search
public class CustomerEditView
{
	public int CustomerID { get; set; }
	public string FirstName { get; set; } = string.Empty;
	public string LastName { get; set; } = string.Empty;
	public string Address1 { get; set; } = string.Empty;
	public string Address2 { get; set; } = string.Empty;
	public string City { get; set; } = string.Empty;
	public int ProvStateID { get; set; }
	public int CountryID { get; set; }
	public string PostalCode { get; set; } = string.Empty;
	public string Phone { get; set; } = string.Empty;
	public string Email { get; set; } = string.Empty;
	public int StatusID { get; set; }
	public bool RemoveFromViewFlag { get; set; }
	//Additional Fields
	//We can add additional calculated fields (fields not in the database) still
	public string OriginalFirstName { get; set; } = string.Empty;
	public bool HasInvoices { get; set; }
	//Whenever a view has a collection, make sure to default it to an empty collection!
	public List<InvoiceListView> Invoices { get; set; } = [];
}

public class InvoiceListView 
{
	public int InvoiceID { get; set; }
	public DateOnly InvoiceDate { get; set; }
	public int CustomerID { get; set; }
	public int EmployeeID { get; set; }
	//Calculated Fields
	public string CustomerName { get; set; } = string.Empty;
	public string EmployeeName { get; set; } = string.Empty;
	public decimal Total { get; set; }
}

public class InvoiceView 
{
	public int InvoiceID { get; set; }
	public DateOnly InvoiceDate { get; set; }
	public int CustomerID { get; set; }
	public int EmployeeID { get; set; }
	public decimal SubTotal { get; set; }
	public decimal Tax { get; set; }
	public bool RemoveFromViewFlag { get; set; }
	//Calculated Fields
	public string CustomerName { get; set; } = string.Empty;
	public string EmployeeName { get; set; } = string.Empty;
	//Read-Only Field (get only)
	//	After the lamda (=>) is what is returned when this field is called
	public decimal Total => SubTotal + Tax;
	//Related Records
	public List<InvoiceLineView> InvoiceLines { get; set; } = [];
}

public class InvoiceLineView
{
	public int InvoiceLineID { get; set; }
	public int InvoiceID { get; set; }
	public int PartID { get; set; }
	public int Quantity { get; set; }
	public decimal Price { get; set; }
	public bool RemoveFromViewFlag { get; set; }
	//Calculated Fields
	public string PartDescription { get; set; } = string.Empty;
	public bool Taxable { get; set; }
	//Read-Only Field (get only)
	public decimal ExtentPrice => Price * Quantity;
}

public class LookupView
{
	public int LookupID { get; set; }
	public string Name { get; set; } = string.Empty;
}
#endregion

//	This region includes support methods
#region Support Method
// Converts a list of error objects into their string representations.
public static List<string> GetErrorMessages(List<Error> errorMessage)
{
	// Initialize a new list to hold the extracted error messages
	List<string> errorList = new();

	// Iterate over each Error object in the incoming list
	foreach (var error in errorMessage)
	{
		// Convert the current Error to its string form and add it to errorList
		errorList.Add(error.ToString());
	}

	// Return the populated list of error message strings
	return errorList;
}
#endregion
