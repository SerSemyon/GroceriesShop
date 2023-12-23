using GroceriesShop.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace GroceriesShop
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<GroceriesContext>();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/login";
                    options.AccessDeniedPath = "/accessdenied";
                });

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.Cookie.Name = ".MyApp.Session";
                options.IdleTimeout = TimeSpan.FromMinutes(60);
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddAuthorization();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapGet("/accessdenied", async (HttpContext context) =>
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Access Denied");
            });

            app.MapGet("/login", async (HttpContext context) =>
            {
                context.Response.ContentType = "text/html; charset=utf-8";
                string loginForm = @"<!DOCTYPE html>
                    <html>
                    <head>
                        <meta charset='utf-8' />
                        <title>Авторизация</title>
                    </head>
                    <body>
                        <h2>Авторизация</h2>
                        <form method='post'>
                            <p>
                                <label>Номер телефона</label><br />
                                <input name='phone_number' />
                            </p>
                            <p>
                                <label>Пароль</label><br />
                                <input type='password' name='password' />
                            </p>
                            <input type='submit' value='Отправить' />
                        </form>
                    </body>
                    </html>";
                await context.Response.WriteAsync(loginForm);
            });

            app.MapPost("/login", async (string? returnUrl, HttpContext context) =>
            {
                GroceriesContext db = new GroceriesContext();
                var form = context.Request.Form;
                if (!form.ContainsKey("phone_number") || !form.ContainsKey("password"))
                    return Results.BadRequest("Номер иелефона и/или пароль не установлены");
                string phone_number = form["phone_number"];
                string password = form["password"];

                Account? person = db.Accounts.FirstOrDefault(p => (p.PhoneNumber == phone_number) && (p.Hashpassword == password));
                if (person is null) return Results.Redirect("/login");
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, person.PhoneNumber),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, person.RoleId.ToString()),
                };
                var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                await context.SignInAsync(claimsPrincipal);
                return Results.Redirect(returnUrl ?? "/");
            });

            app.MapGet("/logout", async (HttpContext context) =>
            {
                await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Results.Redirect("/login");
            });

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.UseSession();

            app.Run();
        }
    }
    public class AuthOptions
    {
        public const string ISSUER = "MyAuthServer";
        public const string AUDIENCE = "MyAuthClient";
        const string KEY = "mysupersecret_secretkey!123qwertyblablacarLOOOOOOOOL";
        public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
    }
}
