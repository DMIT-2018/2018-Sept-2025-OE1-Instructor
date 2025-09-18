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

//Q1

Employees
	//In this case the LastName is not individually in our select
	//	So we have to OrderBy it before we select
	.OrderBy(x => x.LastName)
	.Select(x => new
	{
		FullName = x.FirstName + " " + x.LastName,
		Department = x.DepartmentName,
		//Use a Ternary to display different string values based on database results
		IncomeCategory = x.BaseRate < 30 ? "Required Review" : "No Review Required"
	})
	.Dump();
	
//Q2
Products
	.Where(x => x.ProductSubcategory.ProductCategory.ProductCategoryName == "Music, Movies and Audio Books")
	//Since StyleName is not in the Selected Data, the order by has to be before the select
	.OrderBy(x => x.StyleName)
	.Select(x => new
	{
		ProductName = x.ProductName, 
		Colour = x.ColorName,
		ColourProcessNeeded = (x.ColorName == "Black" || x.ColorName == "White") ? "No" : "Yes"
	})
	.Dump();
	
//Other Example
//Get the 