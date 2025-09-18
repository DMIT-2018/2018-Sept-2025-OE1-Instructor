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
Customers
	.Where(x => x.TotalChildren > 0)
	.Count()
	.Dump();
	
//Q2
Employees
	.Where(x => x.BaseRate > 70)
	.Count()
	.Dump();
	
//Q3
Products
	.Select(x => new
	{
		Name = x.ProductName,
		//In Entity Framework, when there are no child records a null is returned
		//	So we sometimes have to check if there are actually records
		//	We count up the inventory records, if it is 0 (no records) then
		//		we set the value to 0, otherwise we return the sum.
		//	This is needed because SUM will through a null error sometimes.
		TotalOnHand = x.Inventories.Count() == 0 ? 0 : x.Inventories.Sum(i => i.OnHandQuantity)
	})
	.OrderBy(x => x.Name)
	.Dump();
	
//Q4
Promotions
	.Select(x => new 
	{
		PromotionID = x.PromotionID,
		PromotionName = x.PromotionName,
		//Since the PromotionID (FK) field in the InvoiceLines record is nullable
		//	SUM does not throw a Null error like in the above question
		//LINQ only throws null errors when the FK field is not nullable
		TotalDiscountGiven = x.InvoiceLines.Sum(il => il.DiscountAmount)
	})
	.OrderBy(x => x.PromotionName)
	.Dump();
	
//Q5
//Min Cost and Price - done
//OrderBy Category Name - done
//Where Cost and Price are available - done
//Since we need to use aggregate functions on field in the Product Table
//	The product table MUST remain a collection when we query
//	which means we cannot start from the Product Table

ProductSubcategories
	//When possible always keep you WHERE above your select or ordering
	.Where(x => x.Products.Min(p => p.UnitCost) != null && x.Products.Min(p => p.UnitPrice) != null)
	.Select(x => new
	{
		Category = x.ProductCategory.ProductCategoryName,
		SubCategory = x.ProductSubcategoryName,
		LowestCost = x.Products.Min(p => p.UnitCost),
		LowestPrice = x.Products.Min(p => p.UnitPrice)
	})
	//This would not be best practice
	//.Where(x => x.LowestCost != null && x.LowestPrice != null)
	.OrderBy(x => x.Category)
	.ToList()
	.Dump();
	
//Q6
Stores
	.Select(x => new
	{
		StoreID = x.StoreID,
		Name = x.StoreName,
		OldestInvoice = x.Invoices.Min(i => i.DateKey) == null ? "N/A" : x.Invoices.Min(i => i.DateKey).ToShortDateString()
	})
	.OrderBy(x => x.Name)
	.ToList()
	.Dump();
	
	
	
	
	