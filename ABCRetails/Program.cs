using ABCRetails.Services;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

namespace ABCRetails
{
    public class Program
    {
        public static async Task Main(string[] args)  // <-- must be async
        {
            var builder = WebApplication.CreateBuilder(args);

            // Register services
            builder.Services.AddControllersWithViews();
            builder.Services.AddSingleton<IAzureStorageService, AzureStorageService>();
            // builder.Services.AddHostedService<OrderQueueProcessor>(); // if you add this class later

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // ✅ Set South African culture (en-ZA) for currency formatting
            var supportedCultures = new[] { new CultureInfo("en-ZA") };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-ZA"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Ensure storage resources exist
            using (var scope = app.Services.CreateScope())
            {
                var storage = scope.ServiceProvider.GetRequiredService<IAzureStorageService>();
                await storage.EnsureResourcesAsync();
            }

            app.Run();
        }
    }
}
