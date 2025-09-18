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

//Order By Questions
//1. 
Employees.Where(x => x.HireDate.Value >= new DateOnly(2022, 1, 1))
			.OrderBy(x => x.LastName)
			.Dump();

//2. 
Products.Where(x => x.AvailableForSaleDate.Value >= new DateTime(2019, 7, 1))
			.OrderByDescending(x => x.ProductLabel)
			.Dump();

//3.
//Order By should always follow a where clause (not before!)
//	This reduce the processing because we filter the number of records being ordered
//	When selecting a single field it is no longer a property or field it is just the fields datatype
//	Note: Order By should be before the select
Customers.Where(x => x.YearlyIncome.Value > 60000 && x.YearlyIncome.Value < 61000)
			.OrderBy(x => x.EmailAddress)
			.Select(x => x.EmailAddress)
			.Dump();

//What if we wanted to order by something not selected (last name)
//As long as you order by before the select you can order by anything in the Customer record
Customers.Where(x => x.YearlyIncome.Value > 60000 && x.YearlyIncome.Value < 61000)
			.OrderBy(x => x.LastName)
			.Select(x => x.EmailAddress)
			.Dump();

//4.
//Remember when comparing strings, unless specified otherwise transform both/all to the same casing (upper/lower)
Promotions.Where(x => x.PromotionName.ToUpper().Contains("NORTH AMERICA"))
			.OrderBy(x => x.PromotionName)
			.ThenBy(x => x.StartDate)
			.Dump();

