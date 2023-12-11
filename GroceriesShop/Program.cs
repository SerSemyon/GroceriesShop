using GroceriesShop.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GroceriesShop
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            //builder.Services.AddAuthentication("Bearer").AddCookie();
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

            builder.Services.AddAuthorization();            // ���������� �������� �����������

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
                // html-����� ��� ����� ������/������
                string loginForm = @"<!DOCTYPE html>
                    <html>
                    <head>
                        <meta charset='utf-8' />
                        <title>�����������</title>
                    </head>
                    <body>
                        <h2>�����������</h2>
                        <form method='post'>
                            <p>
                                <label>����� ��������</label><br />
                                <input name='phone_number' />
                            </p>
                            <p>
                                <label>������</label><br />
                                <input type='password' name='password' />
                            </p>
                            <input type='submit' value='���������' />
                        </form>
                    </body>
                    </html>";
                await context.Response.WriteAsync(loginForm);
            });

            app.MapPost("/login", async (string? returnUrl, HttpContext context) =>
            {
                GroceriesContext db = new GroceriesContext();
                // �������� �� ����� phone_number � ������
                var form = context.Request.Form;
                // ���� phone_number �/��� ������ �� �����������, �������� ��������� ��� ������ 400
                if (!form.ContainsKey("phone_number") || !form.ContainsKey("password"))
                    return Results.BadRequest("����� �������� �/��� ������ �� �����������");
                string phone_number = form["phone_number"];
                string password = form["password"];

                // ������� ������������ 
                Account? person = db.Accounts.FirstOrDefault(p => (p.PhoneNumber == phone_number) && (p.Hashpassword == password));
                // ���� ������������ �� ������, ���������� ��������� ��� 401
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

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.UseSession();

            app.Run();


            //var builder = WebApplication.CreateBuilder(args);

            //// Add services to the container.
            //builder.Services.AddControllersWithViews();
            //builder.Services.AddDbContext<GroceriesContext>();
            //builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //    .AddJwtBearer(options =>
            //    {
            //        options.TokenValidationParameters = new TokenValidationParameters
            //        {
            //            // ���������, ����� �� �������������� �������� ��� ��������� ������
            //            ValidateIssuer = true,
            //            // ������, �������������� ��������
            //            ValidIssuer = AuthOptions.ISSUER,
            //            // ����� �� �������������� ����������� ������
            //            ValidateAudience = true,
            //            // ��������� ����������� ������
            //            ValidAudience = AuthOptions.AUDIENCE,
            //            // ����� �� �������������� ����� �������������
            //            ValidateLifetime = true,
            //            // ��������� ����� ������������
            //            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
            //            // ��������� ����� ������������
            //            ValidateIssuerSigningKey = true,
            //        };
            //    });

            //var app = builder.Build();

            //// Configure the HTTP request pipeline.
            //if (!app.Environment.IsDevelopment())
            //{
            //    app.UseExceptionHandler("/Home/Error");
            //    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //    app.UseHsts();
            //}

            //app.UseHttpsRedirection();
            ////app.UseDefaultFiles();
            //app.UseStaticFiles();

            //app.UseRouting();
            //app.UseAuthentication();
            //app.UseAuthorization();

            //app.MapPost("/login", (Account loginData) =>
            //{
            //    // ������� ������������ 
            //    GroceriesContext groceriesContext = new GroceriesContext();
            //    Account? person = groceriesContext.Accounts.FirstOrDefault(p => p.PhoneNumber == loginData.PhoneNumber && p.Hashpassword == loginData.Hashpassword);
            //    // ���� ������������ �� ������, ���������� ��������� ��� 401
            //    if (person is null) return Results.Unauthorized();

            //    var claims = new List<Claim>
            //    {
            //        new Claim(ClaimTypes.Name, person.PhoneNumber),
            //        new Claim(ClaimsIdentity.DefaultRoleClaimType, person.RoleId.ToString()),
            //    };
            //    // ������� JWT-�����
            //    var jwt = new JwtSecurityToken(
            //            issuer: AuthOptions.ISSUER,
            //            audience: AuthOptions.AUDIENCE,
            //            claims: claims,
            //            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
            //            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            //    var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            //    // ��������� �����
            //    var response = new
            //    {
            //        access_token = encodedJwt,
            //        username = person.Email
            //    };

            //    return Results.Json(response);
            //});

            //app.Map("/data", [Authorize] () => new { message = "Hello World!" });

            //app.MapControllerRoute(
            //    name: "default",
            //    pattern: "{controller=Home}/{action=Index}/{id?}");

            //app.Run();
        }
    }
    public class AuthOptions
    {
        public const string ISSUER = "MyAuthServer"; // �������� ������
        public const string AUDIENCE = "MyAuthClient"; // ����������� ������
        const string KEY = "mysupersecret_secretkey!123qwertyblablacarLOOOOOOOOL";   // ���� ��� ��������
        public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
    }
}
