using Microsoft.EntityFrameworkCore;
using tenmovies.Models;
namespace tenmovies
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // отримуємо рядок підключення з конфігураційного файлу appsettings.json (ідемо туди і дивимося)
            // звісно, має бути піднятий SQL Server, перевірити дані для підключення можна в SQL Server Management Studio
            string? connection = builder.Configuration.GetConnectionString("DefaultConnection");

            // додаємо контекст бази даних (дивимось Student.cs та StudentContext.cs)
            builder.Services.AddDbContext<MovieContext>(options => options.UseSqlServer(connection));
            // на основі рядка вище, інфраструктура ASP.NET Core створить об'єкт StudentContext, і передасть його в контролер StudentController через механізм впровадження залежностей (Dependency Injection)
            // до речі, в старому ASP.NET (не Core) такого не було, там треба було самому створювати об'єкти контексту даних, а тут все робиться автоматично (в Spring Boot для Java теж так само)
            // !!! два рядки коду вище будуть потрібні завжди, коли треба підключитися до бази даних через Entity Framework !!!

            // додаємо сервіси MVC, інакше не працюватимуть контролери і не підтягнуться вью
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // обслуговуємо статичні файли з wwwroot
            app.UseStaticFiles();

            // стандартний маршрут, при заході на корінь сайту відкривається метод Index контролера Student
            // при бажанні, можна буде вказати айді студента в адресі, наприклад: /Student/Index/2
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Movie}/{action=Index}/{id?}"); // тут Student - це назва саме контролера, а не моделі
            // насправді, клас контролера називається StudentController, але в маршруті вказується лише Student, цього достатньо тому що фреймворк сам додасть слово Controller, є таке правило
            // можна легко замінити контролер на будь-який інший, наприклад Home, тоді відкриватиметься HomeController.cs
            // роутів може бути багато, вони перевірятимуться зверху вниз

            // дивимось клас контролера StudentController.cs та метод Index там

            app.Run(); // запускаємо веб-додаток
        }
    }
}
