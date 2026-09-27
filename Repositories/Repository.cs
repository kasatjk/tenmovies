using Microsoft.EntityFrameworkCore;
using tenmovies.Models;
using tenmovies.Repositories.Interfaces;

namespace tenmovies.Repositories
{
    public class Repository : IRepository
    {
        private readonly MovieContext _context;

        public Repository(MovieContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Movie>> GetAllMoviesAsync()
        {
            return await _context.Movies
                .Include(movie => movie.Poster)
                .ToListAsync();
        }

        public async Task<Movie?> GetMovieByIdAsync(int id)
        {
            return await _context.Movies
                .Include(movie => movie.Poster)
                .FirstOrDefaultAsync(movie => movie.Id == id);
        }

        public async Task AddMovieAsync(Movie movie)
        {
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateMovieAsync(Movie movie)
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteMovieAsync(int id)
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
    }
}