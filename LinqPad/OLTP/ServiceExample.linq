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
	private readonly CustomerService YourService = new CustomerService(context);
	#endregion

	#region Fields from Blazor Page Code-Behind
	// feedback message to display to the user.
	private string feedbackMessage = string.Empty;
	// collected error details.
	private List<string> errorDetails = new();
	// general error message.
	private string errorMessage = string.Empty;
	#endregion

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