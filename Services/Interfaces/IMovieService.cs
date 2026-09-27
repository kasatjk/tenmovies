using Microsoft.AspNetCore.Http;
using tenmovies.Models;

namespace tenmovies.Services.Interfaces
{
    public interface IMovieService
    {
        Task<IReadOnlyList<Movie>> GetAllAsync();
        Task<Movie?> GetByIdAsync(int id);
        Task CreateAsync(Movie movie, IFormFile? poster);
        Task<bool> UpdateAsync(int id, Movie movie, IFormFile? poster);
        Task<bool> DeleteAsync(int id);
    }
}