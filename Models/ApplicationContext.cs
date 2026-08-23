using Microsoft.EntityFrameworkCore;

namespace tenmovies.Models
{
    // контекст даних для роботи з базою через Entity Framework Core
    public class ApplicationContext : DbContext
    {
        public DbSet<FileModel> Files { get; set; } // набір сутностей, що відповідає таблиці файлів

        // конструктор приймає параметри підключення через DI
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated(); // автоматично створює базу даних, якщо її ще немає
        }
    }
}