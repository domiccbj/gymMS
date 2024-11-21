using GymMembershipManagement.DataAccess;
using GymMembershipManagement.DataAccess.Services;

namespace GymMembershipManagement
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Registriraj DatabaseConnection
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddSingleton(new DatabaseConnection(connectionString));


            // Registracija servisa
            builder.Services.AddTransient<DashboardService>();
            builder.Services.AddTransient<UserRepository>();
            builder.Services.AddTransient<MemberService>();
            builder.Services.AddSingleton<EmailService>();
            builder.Services.AddTransient<ActivityLogService>();
            builder.Services.AddTransient<ReportService>();

            builder.Services.AddHostedService<MembershipNotificationService>();
            
            builder.Configuration.AddUserSecrets<Program>();






            // Dodaj session
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Trajanje session-a
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");

            app.Run();
        }
    }
}
