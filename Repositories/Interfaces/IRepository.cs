using tenmovies.Models;

namespace tenmovies.Repositories.Interfaces
{
    public interface IRepository
    {
        Task<IReadOnlyList<Movie>> GetAllMoviesAsync();
        Task<Movie?> GetMovieByIdAsync(int id);
        Task AddMovieAsync(Movie movie);
        Task UpdateMovieAsync(Movie movie);
        Task<bool> DeleteMovieAsync(int id);
    }
}