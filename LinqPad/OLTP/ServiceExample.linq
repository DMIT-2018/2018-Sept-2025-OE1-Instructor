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

// —————— PART 1: Main → UI ——————
//	Driver is responsible for orchestrating the flow by calling 
//	various methods and classes that contain the actual business logic 
//	or data processing operations.
void Main()
{
	CodeBehind codeBehind = new CodeBehind(this); // “this” is LINQPad’s auto Context
	
	//Change to true to see the resulting record
	bool verbose = false;
	
	//Fail Tests
	//Rule: either last name or phone must be provided
	//Simple Error Check
	codeBehind.GetCustomers(string.Empty, string.Empty);
	codeBehind.ErrorDetails.Dump("Fail Expected - Either a partial phone number or a last name must be provided");
	
	//Coloured Testing
	//strings are empty
	if(codeBehind.ErrorDetails.Any(x => x == "Missing Information: Either a partial phone number or a last name must be provided"))
	{
		Util.WithStyle("Pass - (Fail Expected) Empty Last Name and Phone Number","color:LimeGreen").Dump();
	}
	else 
	{
		Util.WithStyle("Fail - No error or incorrect error for Empty Last Name and Phone", "color:OrangeRed;font-weight:bold").Dump();
	}
	if(verbose)
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
	codeBehind.GetCustomers("Foster","");
	if (codeBehind.Customers.Count == 14)
	{
		Util.WithStyle("Pass - Last Name search returned the correct number of results.", "color:LimeGreen").Dump();
	}
	else
	{
		Util.WithStyle("Fail - Last name returned the incorrect number of results or an error was returned.", "color:OrangeRed;font-weight:bold").Dump();
		if(verbose)
			codeBehind.ErrorDetails.Dump();
	}
	if(verbose)
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
			if(results.IsSuccess)
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
				CustomerID = x.CustomerID,
				FullName = x.FirstName + " " + x.LastName,
				Address = x.Address1 + " " + x.City + ", " + x.ProvState.Name + " " + x.Country.Name + " " + x.PostalCode,
				Phone = x.Phone,
				Email = x.Email,
				Status = x.Status.Name,
				TotalSales = x.Invoices.Sum(i => i.SubTotal + i.Tax)
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
	public int CustomerID {get; set;}
	//Calculated Field - Only a string but it doesn't 
	//	exist in the database
	public string FullName {get; set;}
	//Calculated Field
	public string Address {get; set;}
	public string Phone {get; set;}
	public string Email {get; set;}
	//Calculated Field
	public string Status {get; set;}
	//Calculated Field
	public decimal TotalSales {get; set;}
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