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

//Where Clauses Questions
//1. 
//HireDate is a DateOnly field with a ? 
//	The ? means the datatype is nullable
//	When looking at a nullable field, you need to look at the .Value
//	.Value gives us the underlying value stored in the field

Employees.Where(x => x.HireDate.Value >= new DateOnly(2022, 1, 1))
			.Dump();
			
//2. 
Products.Where(x => x.AvailableForSaleDate.Value >= new DateTime(2019, 7,1))
			.Dump();
			
//3.
Customers.Where(x => x.YearlyIncome.Value > 60000 && x.YearlyIncome.Value < 61000)
			.Select(x => x.EmailAddress)
			.Dump();
			
//4.
//Remember when comparing strings, unless specified otherwise transform both/all to the same casing (upper/lower)
Promotions.Where(x => x.PromotionName.ToUpper().Contains("NORTH AMERICA"))
			.Dump();

