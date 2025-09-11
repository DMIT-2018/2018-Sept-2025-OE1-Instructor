<Query Kind="Program">
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

void Main()
{
	//============= Strongly Typed Datasets ===============
	
	//This will return an anonymous type because we only use the new keyword in the select
	var anonScopeData = Tracks
					.Where(x => x.Name.ToLower().Contains("dance"))
					.Select(x => new
					{
						AlbumTitle = x.Album.Title,
						SongTitle = x.Name,
						Artist = x.Album.Artist.Name,
					});
	//We can access the individual piece of data, but not always
	//	You can only access them within the scope or method where they are declared!
	foreach(var song in anonScopeData)
	{
		song.AlbumTitle.Dump();
	}
	
	//Cannot cast (change) Anonymous Datasets to Strongly Typed
	//	Even if the data definition is in the same scope, it still cannot be cast
	//var changeTest = (Song)anonScopeData;
	//changeTest.Dump();
	
	//Getting Anonymous Data outside the Scope
	var anonNotScopedData = SongsByPartialNameAnonymous("dance");
	//anonNotScopedData.Dump();
	
	//Now that the anonymous dataset was created outside the scope of the Main() Method
	//	We cannot access the individual fields created, we can only access the whole record.
	//foreach(var song in anonNotScopedData)
	//{
	//	song.Dump();
	//	song.AlbumTitle.Dump();
	//}
	
	
	//Changing the above to Strongly Typed, we just need to use the Song Class
	Tracks
		.Where(x => x.Name.ToLower().Contains("dance"))
		//by specifying new Song we are telling it what we want the class to be for all results
		.Select(x => new Song
		{
			//Can only include fields that are defined in the Class Definition (Song)
			AlbumTitle = x.Album.Title,
			SongTitle = x.Name,
			Artist = x.Album.Artist.Name
		})
		.Dump();
	var strongNotScopeData = SongsByPartialName("dance");
	//With strongly typed, you can still access the individual fields of the data
	//	Even when it is a dataset created outside the scope
	foreach(var song in strongNotScopeData)
	{
		song.AlbumTitle.Dump();
	}
}

//Making a method to return the anonymous set of data
public IEnumerable SongsByPartialNameAnonymous(string partialSongName)
{
	return Tracks
			.Where(x => x.Name.ToLower().Contains(partialSongName.ToLower()))
			.Select(x => new
			{
				AlbumTitle = x.Album.Title,
				SongTitle = x.Name,
				Artist = x.Album.Artist.Name,
				Bob = "Bob"
			});
}

//When returning a strongly type dataset you want to say the type of data to return
//	Put the type between the <>
//Once the data is no longer on the server and we want to store it in memory
//	we want to use a more robust or functional collection type
//	That is why we are using Lists
//	Lists have more options (functionality) then other more primitive collection types
public List<Song> SongsByPartialName(string partialSongName)
{
	return Tracks
			.Where(x => x.Name.ToLower().Contains(partialSongName.ToLower()))
			.Select(x => new Song
			{
				AlbumTitle = x.Album.Title,
				SongTitle = x.Name,
				Artist = x.Album.Artist.Name
			})
			//Must have this ToList to return a List
			.ToList();
}

//Class Definition
public class Song
{
	public string AlbumTitle {get; set;}
	public string SongTitle {get; set;}
	public string Artist {get; set;}
}
