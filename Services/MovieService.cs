using Microsoft.AspNetCore.Http;
using tenmovies.Models;
using tenmovies.Repositories.Interfaces;
using tenmovies.Services.Interfaces;

namespace tenmovies.Services
{
    public class MovieService : IMovieService
    {
        private readonly IRepository _repository;
        private readonly IWebHostEnvironment _appEnvironment;

        public MovieService(IRepository repository, IWebHostEnvironment appEnvironment)
        {
            _repository = repository;
            _appEnvironment = appEnvironment;
        }

        public async Task<IReadOnlyList<Movie>> GetAllAsync()
        {
            return await _repository.GetAllMoviesAsync();
        }

        public async Task<Movie?> GetByIdAsync(int id)
        {
            return await _repository.GetMovieByIdAsync(id);
        }

        public async Task CreateAsync(Movie movie, IFormFile? poster)
        {
            movie.Poster = await SavePosterAsync(poster);
            await _repository.AddMovieAsync(movie);
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

            await _repository.UpdateMovieAsync(movieInDb);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteMovieAsync(id);
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

            return fileModel;
        }
    }
}