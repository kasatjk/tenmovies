using System.ComponentModel.DataAnnotations;
using tenmovies.Annotations;

namespace tenmovies.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Director is required.")]
        [StringLength(100, ErrorMessage = "Director cannot exceed 100 characters.")]
        public string? Director { get; set; }

        [Required(ErrorMessage = "Genre is required.")]
        [StringLength(100, ErrorMessage = "Genre cannot exceed 100 characters.")]
        public string? Genre { get; set; }

        [Required(ErrorMessage = "Year is required.")]
        [MyYear(ErrorMessage = "Year must be between 1888 and the current year.")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Poster is required.")]
        public FileModel? Poster { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }
    }
}
