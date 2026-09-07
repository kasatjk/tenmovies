using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tenmovies.Models;

/// <summary>
/// Handles HTTP requests for movie resource management, including CRUD operations
/// and physical file storage for movie posters.
/// </summary>
public class MovieController : Controller
{
    private readonly MovieContext _context;
    private readonly IWebHostEnvironment _appEnvironment;

    /// <summary>
    /// Initializes a new instance of the <see cref="MovieController"/> class.
    /// </summary>
    /// <param name="context">The database context for accessing movie and file records.</param>
    /// <param name="appEnvironment">The hosting environment used to locate the web root path for file uploads.</param>
    public MovieController(MovieContext context, IWebHostEnvironment appEnvironment)
    {
        _context = context;
        _appEnvironment = appEnvironment;
    }

    /// <summary>
    /// Retrieves a list of all movies along with their associated poster metadata.
    /// </summary>
    /// <returns>A view displaying the collection of <see cref="Movie"/> entities.</returns>
    // GET: MOVIES
    public async Task<IActionResult> Index()
    {
        return View(await _context.Movies.Include(m => m.Poster).ToListAsync());
    }

    /// <summary>
    /// Displays details for a single movie specified by its unique identifier.
    /// </summary>
    /// <param name="id">The nullable primary key ID of the movie.</param>
    /// <returns>
    /// The details view containing the <see cref="Movie"/> model, or <see cref="NotFoundResult"/> 
    /// if <paramref name="id"/> is null or the entity does not exist.
    /// </returns>
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

    /// <summary>
    /// Displays the view for creating a new movie record.
    /// </summary>
    /// <returns>The creation form view.</returns>
    // GET: MOVIES/Create
    public IActionResult Create()
    {
        return View();
    }

    /// <summary>
    /// Processes creation of a new movie record and saves an uploaded poster image to disk.
    /// </summary>
    /// <param name="movie">The bound <see cref="Movie"/> model state from the request payload.</param>
    /// <param name="poster">An optional uploaded image file containing the poster visual.</param>
    /// <returns>
    /// A redirect to <see cref="Index"/> on successful persistence; otherwise, re-renders the creation view with model errors.
    /// </returns>
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

    /// <summary>
    /// Displays the view for editing an existing movie record.
    /// </summary>
    /// <param name="id">The nullable primary key ID of the movie to edit.</param>
    /// <returns>
    /// The edit view containing the requested <see cref="Movie"/> model, or <see cref="NotFoundResult"/> if invalid.
    /// </returns>
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

    /// <summary>
    /// Updates an existing movie entity in the database and optionally handles replacing its poster image.
    /// </summary>
    /// <param name="id">The route primary key ID corresponding to the target movie.</param>
    /// <param name="movie">The updated <see cref="Movie"/> model payload from the submitted form.</param>
    /// <param name="poster">An optional new poster file to replace the current image.</param>
    /// <returns>
    /// A redirect to <see cref="Index"/> on success; otherwise, returns the edit view with error messages.
    /// </returns>
    /// <exception cref="DbUpdateConcurrencyException">Thrown when a concurrency conflict occurs and the record no longer exists.</exception>
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

    /// <summary>
    /// Displays the confirmation view for deleting a specified movie.
    /// </summary>
    /// <param name="id">The nullable primary key ID of the movie to delete.</param>
    /// <returns>
    /// The deletion confirmation view containing the <see cref="Movie"/> model, or <see cref="NotFoundResult"/> if not found.
    /// </returns>
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

    /// <summary>
    /// Performs the physical deletion of the specified movie entity from the database.
    /// </summary>
    /// <param name="id">The primary key ID of the movie to delete.</param>
    /// <returns>A redirect action to <see cref="Index"/>.</returns>
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

    /// <summary>
    /// Determines whether a movie entity exists in the database.
    /// </summary>
    /// <param name="id">The nullable movie ID to query.</param>
    /// <returns><c>true</c> if a record with the given ID exists; otherwise, <c>false</c>.</returns>
    private bool MovieExists(int? id)
    {
        return _context.Movies.Any(e => e.Id == id);
    }
}