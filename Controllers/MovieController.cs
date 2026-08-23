
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tenmovies.Models;

public class MovieController : Controller
{
    private readonly MovieContext _context;
    private readonly IWebHostEnvironment _appEnvironment;

    public MovieController(MovieContext context, IWebHostEnvironment appEnvironment)
    {
        _context = context;
        _appEnvironment = appEnvironment;
    }

    // GET: MOVIES
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Movies.Include(m => m.Poster).ToListAsync());
    }

    // GET: MOVIES/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var movie = await _context.Movies
            .Include(m => m.Poster)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (movie == null)
        {
            return NotFound();
        }

        return View(movie);
    }

    // GET: MOVIES/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: MOVIES/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Movie movie, IFormFile? poster)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (poster != null && poster.Length > 0)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(_appEnvironment.WebRootPath))
                        {
                            ModelState.AddModelError("Poster", "WebRootPath is not configured");
                            return View(movie);
                        }

                        var uploadsFolder = Path.Combine(_appEnvironment.WebRootPath, "img");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var uniqueName = Guid.NewGuid() + "_" + Path.GetFileName(poster.FileName);
                        var filePath = Path.Combine(uploadsFolder, uniqueName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await poster.CopyToAsync(stream);
                        }

                        var fileModel = new FileModel
                        {
                            Name = poster.FileName,
                            Path = "./img/" + uniqueName,
                            UploadDate = DateTime.Now
                        };

                        _context.Files.Add(fileModel);
                        movie.Poster = fileModel;
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("Poster", $"Error uploading file: {ex.Message}");
                        return View(movie);
                    }
                }

                _context.Movies.Add(movie);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"An error occurred: {ex.Message}");
        }

        return View(movie);
    }

    // GET: MOVIES/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var movie = await _context.Movies
            .Include(m => m.Poster)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (movie == null)
        {
            return NotFound();
        }
        return View(movie);
    }

    // POST: MOVIES/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Movie movie, IFormFile? poster)
    {
        if (id != movie.Id)
        {
            return NotFound();
        }

        var movieInDb = await _context.Movies
            .Include(m => m.Poster)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (movieInDb == null)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            movieInDb.Title = movie.Title;
            movieInDb.Director = movie.Director;
            movieInDb.Genre = movie.Genre;
            movieInDb.Description = movie.Description;
            movieInDb.Year = movie.Year;

            if (poster != null && poster.Length > 0)
            {
                try
                {
                    if (string.IsNullOrEmpty(_appEnvironment.WebRootPath))
                    {
                        ModelState.AddModelError("Poster", "WebRootPath is not configured");
                        return View(movieInDb);
                    }

                    var uploadsFolder = Path.Combine(_appEnvironment.WebRootPath, "img");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueName = Guid.NewGuid() + "_" + Path.GetFileName(poster.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await poster.CopyToAsync(stream);
                    }

                    var newPoster = new FileModel
                    {
                        Name = poster.FileName,
                        Path = "./img/" + uniqueName,
                        UploadDate = DateTime.Now
                    };

                    _context.Files.Add(newPoster);
                    movieInDb.Poster = newPoster;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Poster", $"Error uploading file: {ex.Message}");
                    return View(movieInDb);
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieExists(movie.Id))
                    return NotFound();
                else
                    throw;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred while saving: {ex.Message}");
                return View(movieInDb);
            }

            return RedirectToAction(nameof(Index));
        }

        return View(movie);
    }

    // GET: MOVIES/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var movie = await _context.Movies
            .Include(m => m.Poster)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (movie == null)
        {
            return NotFound();
        }

        return View(movie);
    }

    // POST: MOVIES/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var movie = await _context.Movies.FindAsync(id);
        if (movie != null)
        {
            _context.Movies.Remove(movie);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool MovieExists(int? id)
    {
        return _context.Movies.Any(e => e.Id == id);
    }
}
