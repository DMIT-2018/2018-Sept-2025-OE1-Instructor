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

//Results in 2 distinct lists, but what if we want one?
Artists
	.Where(x => x.ArtistId <= 5).Dump();
	
Albums
	.Where(x => x.AlbumId <= 5).Dump();

//Anonymous Type
Artists
	.Where(x => x.ArtistId <= 5)
	.Select(x => new
	{
		ID = x.ArtistId,
		Name = x.Name,
		//Need to use navigational properties to only get the records that have a relationship with
		//	the specific record being looked at.
		Albums = x.Albums
					.OrderBy(a => a.Title)
					//Because of the navigational property, we do not need to use a WHERE
					//	this is done automatically by the navigation
					//NOTE: Using a WHERE is not Acceptable on any Assessment or Take Home
					//.Where(a => a.ArtistId == x.ArtistId)
					.Select(a => new
					{
						Album = a.Title,
						Label = a.ReleaseLabel,
						Year = a.ReleaseYear
					})
					.ToList()
	})
	.Dump();

//Strongly Typed
Artists
	.Where(x => x.ArtistId <= 5)
	.Select(x => new ArtistView
	{
		ID = x.ArtistId,
		Name = x.Name,
		Albums = x.Albums
					.OrderBy(a => a.Title)
					.Select(a => new AlbumView
					{
						Album = a.Title,
						Label = a.ReleaseLabel,
						Year = a.ReleaseYear,
						Tracks = a.Tracks
									.Select(t => new TrackView
									{
										TrackID = t.TrackId,
										Name = t.Name,
										Length = t.Milliseconds/1000, //Converting the Length to Seconds
										//We are looking at the Track record when inside the Select
										//	We can navigate up to het parent data or down to get child data
										//	as more nested records if needed
										Genre = t.Genre.Name
									})
									.ToList()
					})
					.ToList()
	})
	.ToList()
	.Dump();


//Class Definitions
public class TrackView
{
	public int TrackID {get; set;}
	public string Name {get; set;}
	public int Length {get; set;}
	public string Genre {get; set;}
}
public class AlbumView
{
	public string Album {get; set;}
	public string Label {get; set;}
	public int Year {get; set;}
	//Note: All Lists in views should be automatically populated by an empty list
	//	 = [];
	// 	[] has the same meaning as new List<TrackView>()
	//This prevents unexpected null value errors
	public List<TrackView> Tracks {get; set;} = [];
}

//Using composition to have a list of the AlbumView records
public class ArtistView
{
	public int ID {get; set;}
	public string Name {get; set;}
	public List<AlbumView> Albums {get; set;} = [];
}
	