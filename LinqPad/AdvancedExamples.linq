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
						Bob = "bob"
					});
	//We can access the individual piece of data, but not always
	//	You can only access them within the scope or method where they are declared!
	foreach (var song in anonScopeData)
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
		.Select(x => new SongView
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
	foreach (var song in strongNotScopeData)
	{
		song.AlbumTitle.Dump();
	}

	//The inheritence lets us define any fields in the base class (Person) or the derived class (Student)
	var student1 = new Student {
		FirstName = "Bob",
		LastName = "Ross",
		GPA = 4.0m
	};
	
	
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
public List<SongView> SongsByPartialName(string partialSongName)
{
	return Tracks
			.Where(x => x.Name.ToLower().Contains(partialSongName.ToLower()))
			.Select(x => new SongView
			{
				AlbumTitle = x.Album.Title,
				SongTitle = x.Name,
				Artist = x.Album.Artist.Name
			})
			//Must have this ToList to return a List
			.ToList();
}

//Class Definition
public class SongView
{
	public string AlbumTitle {get; set;}
	public string SongTitle {get; set;}
	public string Artist {get; set;}
}

public class Person 
{
	public string FirstName { get; set; } 
	public string LastName { get; set; } 
}

//Inheritence - the class is derived from another class (base class)
//	Has access to all the base class information that is public
public class Student: Person 
{
	public decimal GPA { get; set; }
}

//Composition - Contains other class (instances of another class)
public class Classroom
{
	public string ClassName { get; set; }
	public List<Student> Students { get; set; } = [];
}
