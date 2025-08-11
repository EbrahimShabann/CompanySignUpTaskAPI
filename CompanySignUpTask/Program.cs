
using CompanySignUpTask.Data_Layer;
using CompanySignUpTask.Repository_Layer.IRepository;
using CompanySignUpTask.Repository_Layer.Repository;
using CompanySignUpTask.Service_Layer.CompanyServices;
using CompanySignUpTask.Service_Layer.EmailService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace CompanySignUpTask
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var txt = "";
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            //register Email Service
            builder.Services.AddScoped<IEmailService, EmailService>();
            // Configure Entity Framework Core with PostegreSQL
            builder.Services.AddDbContext<AppDbContext>(options =>
                            options
                            .UseLazyLoadingProxies()
                            .UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
            );

            //configure swagger
            builder.Services.AddSwaggerGen(options =>
            {
                // Add JWT Bearer support to Swagger UI
                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOi...\""
                });
                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

          

            //enable CORS
            builder.Services.AddCors(options =>
            {

                // Add CORS policy for Angular frontend
                options.AddPolicy("AllowAngular",
                    builder => builder
                        .WithOrigins("http://localhost:4200") 
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials());


            });
            // Configure JWT Authentication
            //var jwtKey = builder.Configuration["Jwt:Key"];
            //var jwtIssuer = builder.Configuration["Jwt:Issuer"];
            //builder.Services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //})
            //.AddJwtBearer(options =>
            //{
            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuer = true,
            //        ValidateAudience = false,
            //        ValidateLifetime = true,
            //        ValidateIssuerSigningKey = true,
            //        ValidIssuer = jwtIssuer,
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            //        RoleClaimType = ClaimTypes.Role,

            //    };
            //    options.MapInboundClaims = false; // Disable automatic mapping of claims coz it's mapping userName to name identifier
            //});
            builder.Services.AddAuthorization();

            //register services and repositories
            builder.Services.AddScoped<ICompanyRepo,CompanyRepo>();   
            builder.Services.AddScoped<ICredintailsService,CredintailsService>();   

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/openapi/v1.json", "v1");
                    //options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");

                });
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowAngular");
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
