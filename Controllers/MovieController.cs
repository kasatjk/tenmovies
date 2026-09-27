using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tenmovies.Models;
using tenmovies.Services.Interfaces;

public class MovieController : Controller
{
    private readonly IMovieService _movieService;

    public MovieController(IMovieService movieService)
    {
        _movieService = movieService;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _movieService.GetAllAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var movie = await _movieService.GetByIdAsync(id.Value);
        return movie == null ? NotFound() : View(movie);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Movie movie, IFormFile? poster)
    {
        if (poster == null || poster.Length == 0)
        {
            ModelState.AddModelError("poster", "Please choose a poster file.");
        }

        if (ModelState.IsValid)
        {
            try
            {
                await _movieService.CreateAsync(movie, poster);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Poster", $"Error uploading file: {ex.Message}");
            }
        }

        return View(movie);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var movie = await _movieService.GetByIdAsync(id.Value);
        return movie == null ? NotFound() : View(movie);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Movie movie, IFormFile? poster)
    {
        if (id != movie.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                if (!await _movieService.UpdateAsync(id, movie, poster))
                {
                    return NotFound();
                }

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Poster", $"Error uploading file: {ex.Message}");
            }
        }

        return View(movie);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var movie = await _movieService.GetByIdAsync(id.Value);
        return movie == null ? NotFound() : View(movie);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        if (id == null || !await _movieService.DeleteAsync(id.Value))
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }
}