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



