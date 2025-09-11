<Query Kind="Statements">
  <Connection>
    <ID>ff425501-9209-49aa-8e02-55fa9d6c112c</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Server>MOMSDESKTOP\SQLEXPRESS</Server>
    <AllowDateOnlyTimeOnly>true</AllowDateOnlyTimeOnly>
    <DeferDatabasePopulation>true</DeferDatabasePopulation>
    <Database>Contoso</Database>
    <DriverData>
      <LegacyMFA>false</LegacyMFA>
    </DriverData>
  </Connection>
</Query>

// Anonymous Data Sets & Navigational Properties In Class

//1. 

Products
	.Where(x => x.UnitPrice < 10)
	.Select(x => new
	{
		ProductLabel = x.ProductLabel,
		ProductName = x.ProductName,
		UnitPrice = x.UnitPrice
	})
	.OrderBy(x => x.UnitPrice)
	.ThenBy(x => x.ProductName)
	.Dump();
	
//2.
Customers
	.Where(x => x.Geography.StateProvinceName == "British Columbia"
				&& x.Geography.RegionCountryName == "Canada")
	.Select(x => new
	{
		FirstName = x.FirstName,
		LastName = x.LastName,
		CityName = x.Geography.CityName
	})
	.OrderBy(x => x.CityName)
	.ThenBy(x => x.LastName)
	.Dump();

//3.
//When finding data from multiple tables it is easiest to find the lowest child table to start at
//	Example: We don't want to start at Category or SubCategory because we will only get 
//		collections for the child records.
//	We can navigate to a parents parent by chaining the navigation

Products
	.Where(x => x.ProductSubcategory.ProductCategory.ProductCategoryName == "Audio"
			&& x.ColorName == "Pink"
			&& (x.ProductSubcategory.ProductSubcategoryName == "Bluetooth Headphones"
				|| x.ProductSubcategory.ProductSubcategoryName == "Recording Pen"))
	.Select(x => new
	{
		CategoryName = x.ProductSubcategory.ProductCategory.ProductCategoryName,
		SubcategoryName = x.ProductSubcategory.ProductSubcategoryName,
		ProductName = x.ProductName
	})
	.OrderBy(x => x.SubcategoryName)
	.ThenBy(x => x.ProductName)
	.Dump();


