using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using tenmovies.Models;
using tenmovies.Services.Interfaces;

namespace tenmovies.Services
{
    public class MovieService : IMovieService
    {
        private readonly MovieContext _context;
        private readonly IWebHostEnvironment _appEnvironment;

        public MovieService(MovieContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }

        public async Task<IReadOnlyList<Movie>> GetAllAsync()
        {
            return await _context.Movies
                .Include(movie => movie.Poster)
                .ToListAsync();
        }

        public async Task<Movie?> GetByIdAsync(int id)
        {
            return await _context.Movies
                .Include(movie => movie.Poster)
                .FirstOrDefaultAsync(movie => movie.Id == id);
        }

        public async Task CreateAsync(Movie movie, IFormFile? poster)
        {
            movie.Poster = await SavePosterAsync(poster);
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(int id, Movie movie, IFormFile? poster)
        {
            var movieInDb = await GetByIdAsync(id);
            if (movieInDb == null)
            {
                return false;
            }

            movieInDb.Title = movie.Title;
            movieInDb.Director = movie.Director;
            movieInDb.Genre = movie.Genre;
            movieInDb.Description = movie.Description;
            movieInDb.Year = movie.Year;

            var newPoster = await SavePosterAsync(poster);
            if (newPoster != null)
            {
                movieInDb.Poster = newPoster;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return false;
            }

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<FileModel?> SavePosterAsync(IFormFile? poster)
        {
            if (poster == null || poster.Length == 0)
            {
                return null;
            }

            if (string.IsNullOrEmpty(_appEnvironment.WebRootPath))
            {
                throw new InvalidOperationException("WebRootPath is not configured");
            }

            var uploadsFolder = Path.Combine(_appEnvironment.WebRootPath, "img");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueName = $"{Guid.NewGuid()}_{Path.GetFileName(poster.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await poster.CopyToAsync(stream);

            var fileModel = new FileModel
            {
                Name = poster.FileName,
                Path = "./img/" + uniqueName,
                UploadDate = DateTime.Now
            };

            _context.Files.Add(fileModel);
            return fileModel;
        }
    }
}