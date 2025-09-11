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


//========= WHERE EXAMPLES ===============
int[] numbers = [1, 2, 3, 4, 5, 6, 7, 8, 9];

//Evaluates each item in the numbers collection to test if % 2 == 0 is true
var evenNumbers = numbers.Where(n => n % 2 == 0);

numbers.Dump();
evenNumbers.Dump();

//Get all Album titles that have a release year = 1999
Albums.Where(a => a.ReleaseYear == 1999)
		.Select(a => a.Title)
		.Dump();
		
//======== SORTING EXAMPLES ===========

// && AND, || OR
Albums.Where(x => x.ReleaseYear >= 1990 && x.ReleaseYear < 2000)
	.OrderBy(x => x.ReleaseYear)
	.Dump();

Albums.Where(x => x.ReleaseYear >= 1990 && x.ReleaseYear < 2000)
	.OrderByDescending(x => x.ReleaseYear)
	.Dump();

//Order By both year (ascending) and label (descending)
Albums.Where(x => x.ReleaseYear >= 1990 && x.ReleaseYear < 2000)
	.OrderBy(x => x.ReleaseYear)
	.ThenByDescending(x => x.ReleaseLabel)
	.Dump();

//================== Navigational Properties ========================
//Navigating to a Parent, we get 1 record only
//Navigate by saying record.Parent
//	We can then see all fields/properties of the parent record
//	Example: record.Parent.parentField
Albums.Where(album => album.Artist.Name == "Deep Purple").Dump();

//Navigating to a Child Record, we get a collection only (collection of 0 or more records
//We cannot see the individual fields/properties of child records
//	because the child records are returned as a collection
//	For example: Albums.Where(album => album.Tracks.Composer == "Coverdale").Dump
//	The example will never work, because album.Tracks is not a single record

//==================== Anonymous Datasets ===========================
//Basic data from our entity collections is returned with a Class <Artist> and <Album>
Artists.Where(x => x.ArtistId < 6).Dump();
Albums.Where(x => x.AlbumId < 6).Dump();

//If we only want certain fields we can return an anonymous collection of data
//	We use the new keyword to say we want to define (shape) the data to a new type
//	Below we are not shaping to a class or defined type, it is anonymous <> (empty)
Albums.Where(x => x.AlbumId < 6)
		.Select(x => new
		{
			Title = x.Title,
			Year = x.ReleaseYear,
			Label = x.ReleaseLabel
		}).Dump();

//We can order our results, remember to use the names that you define
//	ReleaseYear no longer exists in our NEW Type, we must use Year
Albums.Where(x => x.AlbumId < 6)
		.Select(x => new
		{
			Title = x.Title,
			Year = x.ReleaseYear,
			Label = x.ReleaseLabel
		})
		.OrderByDescending(x => x.Year)
		.Dump();

//We can also use our navigational properties to shape data from multiple tables!
//	Navigate to the Artist (parent) record to get the name of the artist from the single parent record

Albums.Where(x => x.AlbumId < 6)
		.Select(x => new
		 {
			 Title = x.Title,
			 Year = x.ReleaseYear,
			 Label = x.ReleaseLabel,
			 Artist = x.Artist.Name
		 }).Dump();

//Navigation in any type (anonymous or otherwise) to a child record will return a collection
//	We cannot select a single field/property straight from the child record collection
Albums.Where(x => x.AlbumId < 6)
		.Select(x => new
		{
			Title = x.Title,
			Year = x.ReleaseYear,
			Label = x.ReleaseLabel,
			Artist = x.Artist.Name,
			Tracks = x.Tracks
		}).Dump();
		
//============ Terinary Operators ===============================

//Must have a true and a false path that contains 1 statement each.
//	Remember a statement is ended with a semi-colon
//Function like a if/else with single statements
//	conditional test ? true statement : false statement

var test = true;
//statements all the way, you can nest as many terinary operators as you want.
var test2 = test ? true : test ? true : false;

Random rand = new Random();
var num = rand.Next(1, 5);
var numResult = num == 1 ? "The number is 1" : 
				num == 2 ? "The number is 2" : 
				num == 3 ? "The number is 3" : "The number is to high!"; 
num.Dump();
numResult.Dump();

//This could be rewritten as if/else or as a switch, but with a terinary operator it remains as one simple statement.

//Example: We can replace null or white space Fax numbers for customers with the word "Unknown"
Customers.OrderByDescending(x => x.Fax)
			.Select(x => new
			{
				Name = x.FirstName + " " + x.LastName,
				Country = x.Country,
				Fax = x.Fax == null || x.Fax.Trim() == "" ? "Unknown" : x.Fax
			}).Dump();
			
//You can use multiple conditions with && and || or
//You can call methods, use data from navigational properties, etc.
//	As long as the condition of the terinary operator return true or false, it will work.

Albums.Select(x => new 
	{
		Title = x.Title,
		Artist = x.Artist.Name,
		Year = x.ReleaseYear,
		Decade = x.ReleaseYear < 1970 ? "Oldies" : 
					x.ReleaseYear < 1980 ? "70s" :
					x.ReleaseYear < 1990 ? "80s" :
					x.ReleaseYear < 2000 ? "90s" : "Modern"
	})
	.OrderBy(x => x.Year)
	.Dump();
	
	
	
	