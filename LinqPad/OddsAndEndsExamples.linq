<Query Kind="Statements">
  <Connection>
    <ID>7b4c5e97-3cd6-4f59-b757-2e7509eac599</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Server>MOMSDESKTOP\SQLEXPRESS</Server>
    <AllowDateOnlyTimeOnly>true</AllowDateOnlyTimeOnly>
    <DeferDatabasePopulation>true</DeferDatabasePopulation>
    <Database>Chinook-2025</Database>
    <NoCapitalization>true</NoCapitalization>
    <DriverData>
      <LegacyMFA>false</LegacyMFA>
    </DriverData>
  </Connection>
</Query>

//First
//Returns the first item in a collection
//We will never use First, always use FirstOrDefault!!!
Albums.First().Dump();

//First throws an error when there are no items in a collection
//	This includes if nothing is found when using a where clause
//Albums
//	.Where(x => x.AlbumId == 1000000)
//	.First().Dump();

//We use .FirstOrDefault()
//	This returns a null for an object when nothing is found
Albums
	.Where(x => x.AlbumId == 1000000)
	.FirstOrDefault().Dump();

//Default means the default of the datatype
//	For objects this is null, for numerical types this is 0
int[] testInts = [1, 2, 3, 4, 5];

testInts.Where(x => x == 6)
	.FirstOrDefault().Dump();

//For a string it is also null
string[] testStrings = ["Test", "Test1", "Test2"];

testStrings.Where(x => x == "Test3")
	.FirstOrDefault().Dump();

//Remember when selecting one piece of information (or a field) that it is no longer an object
//	For example, if we select just the ReleaseYear, the results are an integer
//	The default of an integer is a 0
//Note: Don't always look for a null in this case, look at the datatype
Albums
	.Where(x => x.AlbumId == 1000000)
	.Select(x => x.ReleaseYear)
	.FirstOrDefault().Dump();

//In this case we get a null, because Title in a string
Albums
	.Where(x => x.AlbumId == 1000000)
	.Select(x => x.Title)
	.FirstOrDefault().Dump();
	
//Be aware of datatypes that are changed to nullable
//	Even though ReportsTo is an integer, it is marked as nullable with the ?
//	This returns null when something isn't found
//Note: If the field in the database is nullable (allows nulls)
//	Entity Framework will change any not nullable datatype (Dates, numbers, booleans, etc.) 
//	to nullable with the ? when nulls are allows for the field in the database
Employees
	.Where(x => x.EmployeeId == 1000000)
	.Select(x => x.ReportsTo)
	.FirstOrDefault().Dump();
	
//Even if we create a default for a ViewModel, if the database doesn't have a record
//	that matches our Where clause, we still get nothing back
//	in this case the default of the object is null
Albums
	.Where(x => x.AlbumId == 1000000)
	.Select(x => new AlbumView
	{
		AlbumID = x.AlbumId	
	})
	.FirstOrDefault().Dump();

//============= DISTINCT ===============
//Remove all fully duplicated records
//Example, we want to find all labels that released an album for each year
Albums //300 records
	.Where(x => x.ReleaseYear > 1980)
	.Select(x => new
	{
		Year = x.ReleaseYear,
		Label = x.ReleaseLabel
	})
	.OrderBy(x => x.Year)
	.ThenBy(x => x.Label)
	.Dump();

Albums //263 records
	.Where(x => x.ReleaseYear > 1980)
	.Select(x => new
	{
		Year = x.ReleaseYear,
		Label = x.ReleaseLabel
	})
	.OrderBy(x => x.Year)
	.ThenBy(x => x.Label)
	//Remember .Distinct needs to be after you select your data
	.Distinct().Dump();

Albums //300 records
	//Because each Album is unique here this distinct does nothing
	.Distinct()
	.Where(x => x.ReleaseYear > 1980)
	.Select(x => new
	{
		Year = x.ReleaseYear,
		Label = x.ReleaseLabel
	})
	.OrderBy(x => x.Year)
	.ThenBy(x => x.Label)
	.Dump();

//======== ANY/ALL =============
//Returns true or false if there is any record in the collection that matches the search
//	Super useful for if statements
Albums
	.Where(x => x.ReleaseYear == 1976)
	.Any().Dump();
	
Albums
	.Any(x => x.ReleaseYear == 1976)
	.Dump();
	
//All with return true only if every element of a collection matches the search
// Example: check if all albums have a release label.
Albums
	.All(x => !(x.ReleaseLabel.Trim() == "") || x.ReleaseLabel != null)
	.Dump();

//========== Unions and Other Joins =================
int[] numbersA = [1,2,3,4];
int[] numbersB = [3,4,5,6];
//Union will combine the distinct elements
numbersA.Union(numbersB).Dump("Union");

//Intersect will only return the common elements from two collections
//Note: Super useful when you want to check if something already exists in another list.
numbersA.Intersect(numbersB).Dump("Intersect");

//Except - Return elements from the FIRST collection that are not in the SECOND collection
numbersA.Except(numbersB).Dump("Except");

//Concat - Acts like the SQL Union, returns all elements from both collection
numbersA.Concat(numbersB).Dump("Concat");



public class AlbumView
{
	public int AlbumID { get; set; } = 5;
}