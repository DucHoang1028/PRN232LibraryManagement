using System.Text;
using System.Text.Json.Serialization;
using DataAccessObjects;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Repositories;
using Services;

namespace LibraryManagementAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //Add Authenticcation with Jwt Bearer
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                var jwtSettings = builder.Configuration.GetSection("Jwt");
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
                };
            });


            // Add services to the container.
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetSection
                    ("ConnectionStrings:LibraryManagementDB").Value);
            });

            builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;

                options.Password.RequiredLength = 8; // Minimum length
                options.Password.RequireDigit = true; // Must contain a number
                options.Password.RequireUppercase = true; // Must contain an uppercase letter
                options.Password.RequireLowercase = true; // Must contain a lowercase letter
                options.Password.RequireNonAlphanumeric = true; // Must contain special characters (e.g. @, #)
            }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            // Add CORS policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowWebClient", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            // Register repositories
            builder.Services.AddScoped<IBookRepository, BookRepository>();
            builder.Services.AddScoped<IMemberRepository, MemberRepository>();
            builder.Services.AddScoped<ILoanRepository, LoanRepository>();
            builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
            builder.Services.AddScoped<IBlogRepository, BlogRepository>();

            // Register services
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IBookService, BookService>();
            builder.Services.AddScoped<IMemberService, MemberService>();
            builder.Services.AddScoped<ILoanService, LoanService>();
            builder.Services.AddScoped<IPublisherService, PublisherService>();
            builder.Services.AddScoped<IFineService, FineService>();
            builder.Services.AddScoped<IReviewService, ReviewService>();
            builder.Services.AddScoped<IBlogService, BlogService>();

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
            });            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthentication();
            // Use CORS before other middleware
            app.UseCors("AllowWebClient");

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
