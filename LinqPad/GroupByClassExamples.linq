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

//============= Group By ====================
//Group By is used only when absolutely needed, otherwise use other methods as Group By is resource intensive.
//	Group By can slow down queries quite a lot

//Group By is needed when we are trying to organize data or find information about data and we are trying to find it based
//	on data in same table.

//If I wanted to show Album data by the Artist, Group By isn't the best solution
//But if I want to show Album data by the ReleaseYear, Group By is useful
Albums
	.GroupBy(x => x.ReleaseYear)
	.Dump();
	
Albums
	.GroupBy(x => x.ReleaseYear)
	.Select(x => new
	{
		//Key refers the the field that is used in the Group By
		Year = x.Key,
		Albums = x.ToList()
	})
	.Dump();
	
//We can also group by more than one field
//	To do this we make what we are grouping by an anonymous dataset
//	use the new keyword
Albums
	.GroupBy(x => new { x.ReleaseYear, x.ReleaseLabel })
	.Select(x => new
	{
		//Reference each key in the Group By
		Year = x.Key.ReleaseYear,
		Label = x.Key.ReleaseLabel,
		Count = x.Count(),
		//The data that is grouped now you can select information from
		Albums = x.Select(a => new
		{
			Title = a.Title,
			Artist = a.Artist.Name
		}).ToList()
	})
	.OrderBy(x => x.Year)
	.ThenBy(x => x.Label)
	.Dump();
	
//Example when not to use Group By
// Show the invoice Total amounts for each customer
//	In this case you can use the navigational properties to avoid grouping
//	We can start at the customer table

Customers
	.Select(x => new
	{
		Name = x.FirstName + " " + x.LastName,
		TotalInvoiceAmount = x.Invoices.Sum(i => i.Total)
	})
	.OrderBy(x => x.Name)
	.ToList()
	.Dump();

//Same results using a Group By
//	This is slower and with thousands or millions of records
//	this will slow down more
Invoices
	.GroupBy(x => new
	{
		FullName = x.Customer.FirstName + " " + x.Customer.LastName
	})
	.Select(x => new
	{
		Name = x.Key.FullName,
		TotalInvoiceAmount = x.Sum(i => i.Total)
	})
	.ToList()
	.Dump();
