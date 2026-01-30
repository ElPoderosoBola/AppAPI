using AplicaciónWebTest1.Data;
using AplicaciónWebTest1.Models;
using AplicaciónWebTest1.Services;

namespace AplicaciónWebTest1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();
            builder.Services.AddSingleton<ConfigService>();
            builder.Services.AddSingleton<AppConfig>(sp => sp.GetRequiredService<ConfigService>().Settings);
            builder.Services.AddTransient<ProductoDAO>();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Productos/Index");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapControllers();
            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Productos}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
