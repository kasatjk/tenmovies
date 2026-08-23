using Microsoft.EntityFrameworkCore;
namespace tenmovies.Models
{
    public class MovieContext : DbContext
    {
        public DbSet<Movie> Movies { get; set; }
        public DbSet<FileModel> Files { get; set; }
        public MovieContext(DbContextOptions<MovieContext> options)
           : base(options)
        {
            if (Database.EnsureCreated())
            {
                Movies?.Add(new Movie { Title = "Oleksandr", Director = "Chris Columbus", Genre = "Documentary", Year = 2026, Poster = new FileModel { Name = "oleksandr.png", Path = "./img/oleksandr.png", UploadDate = DateTime.Now }, Description = "A documentary about Oleksandr Zahoruiko, a software engineer and university lecturer with a background in both programming and economics. Cool teacher!" });

                Movies?.Add(new Movie { Title = "Смешарики", Director = "Denis Yuryevich Chernov", Genre = "Comedy-drama", Year = 2003, Poster = new FileModel { Name = "smeshariki.png", Path = "./img/smeshariki.png", UploadDate = DateTime.Now }, Description = "Smeshariki is an animated television series consisting of 408 episodes aimed at children of 3 to 8 years. This series uses mostly both flash animation and computer animation." });

                Movies?.Add(new Movie { Title = "The Hobbit: An Unexpected Journey", Director = "Peter Jackson", Genre = "Fantasy", Year = 2012, Poster = new FileModel { Name = "thehobbit.webp", Path = "./img/thehobbit.webp", UploadDate = DateTime.Now }, Description = "A reluctant Hobbit, Bilbo Baggins, sets out to the Lonely Mountain with a spirited group of dwarves to reclaim their mountain home and the gold within it from the dragon Smaug." });

                Movies?.Add(new Movie { Title = "Interstellar", Director = "Christopher Nolan", Genre = "Sci-Fi", Year = 2014,Poster = new FileModel { Name = "interstellar.jpg", Path = "./img/interstellar.jpg", UploadDate = DateTime.Now }, Description = "In a dystopian future where Earth has become near-uninhabitable, a team of astronauts embark on a mission to find a new home for humanity." });

                Movies?.Add(new Movie { Title = "Oppenheimer", Director = "Christopher Nolan", Genre = "History", Year = 2023, Poster = new FileModel { Name = "oppenheimer.jpg", Path = "./img/oppenheimer.jpg", UploadDate = DateTime.Now }, Description = "A dramatization of the life story of J. Robert Oppenheimer, the physicist who had a large hand in the development of the atomic bombs that brought an end to World War II." });

                Movies?.Add(new Movie { Title = "The Mandalorian", Director = "Jon Favreau", Genre = "Action Epic", Year = 2019,Poster = new FileModel { Name = "themandalorian.jpg", Path = "./img/themandalorian.jpg", UploadDate = DateTime.Now }, Description = "The travels of a lone bounty hunter in the outer reaches of the galaxy, far from the authority of the New Republic" });

                Movies?.Add(new Movie { Title = "Venom", Director = "Ruben Fleischer", Genre = "Action", Year = 2018,Poster = new FileModel { Name = "venom.jpg", Path = "./img/venom.jpg", UploadDate = DateTime.Now }, Description = "A failed reporter is bonded to an alien entity, one of many symbiotes who have invaded Earth. But the being takes a liking to Earth and decides to protect it." });

                Movies?.Add(new Movie { Title = "Star Wars: Episode V - The Empire Strikes Back", Director = "Irvin Kershner", Genre = "Sci-Fi", Year = 1980,Poster = new FileModel { Name = "starwars.jpg", Path = "./img/starwars.jpg", UploadDate = DateTime.Now }, Description = "After the Empire overpowers the Rebel Alliance, Luke Skywalker begins training with Jedi Master Yoda, while Darth Vader and bounty hunter Boba Fett pursue his friends across the galaxy." });

                Movies?.Add(new Movie { Title = "Pirates of the Caribbean: The Curse of the Black Pearl", Director = "Gore Verbinski", Genre = "Adventure", Year = 2003,Poster = new FileModel { Name = "pirates.jpg", Path = "./img/pirates.jpg", UploadDate = DateTime.Now }, Description = "Captain Jack Sparrow teams up to rescue a kidnapped maiden from cursed pirates."});

                SaveChanges();
            }
        }
    }
}
