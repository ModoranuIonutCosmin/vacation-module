using System.Reflection;
using System.Text;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using VacationsModule.Application.Auth;
using VacationsModule.Application.Auth.Config;
using VacationsModule.Application.Auth.ExtensionMethods;
using VacationsModule.Application.Features;
using VacationsModule.Application.Interfaces.Repositories;
using VacationsModule.Application.Interfaces.Services;
using VacationsModule.Domain.DomainServices;
using VacationsModule.Domain.DomainServices.Interfaces;
using VacationsModule.Domain.DomainServices.Validators;
using VacationsModule.Domain.Entities;
using VacationsModule.Infrastructure.Data_Access;
using VacationsModule.Infrastructure.Data_Access.v1;
using VacationsModule.Infrastructure.Seed;

public class Program
{
    public static bool testingEnvironment;
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var configuration = builder.Configuration;
        
        
        var env = builder.Environment;
        
        testingEnvironment = (env.EnvironmentName == "Testing");
        
        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();

        AddDataAccessEF(builder);
        AddIdentityJWTAuth(builder);
        AddApiVersioning(builder);

        AddCors(builder);
        AddSwagger(builder);
        AddHealthChecks(builder);


        //Add mediatr
        AddMediatR(builder);


        AddAutoMapper(builder);



        builder.Services
            .AddTransient<IEmployeeEligibleVacationDaysCountValidator, EmployeeEligibleVacationDaysCountValidator>()
            .AddTransient<IVacationDaysCalculatorService, VacationDaysCalculatorService>()
            .AddTransient<IVacationRequestValidator, VacationRequestValidator>()
            .AddTransient<IVacationDateIntervalsValidator, VacationDateIntervalsValidator>()
            .AddTransient<IVacationRequestsService, VacationRequestsService>()
            .AddTransient<INationalDaysService, NationalDaysService>()
            .AddTransient<IAuthenticationService, AuthenticationService>()
            .AddTransient<IEmployeesService, EmployeesService>()
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddScoped<IEmployeesRepository, EmployeesRepository>()
            .AddScoped<IVacationRequestsRepository, VacationRequestsRepository>()
            .AddScoped<IAuthorizationService, AuthorizationService>()
            .AddTransient<UsersSeed>()
            .AddSingleton(_ => new JwtOptionsProvider
            (
                Environment.GetEnvironmentVariable("JWT_SECRET") ?? configuration["Jwt:Secret"],
                Environment.GetEnvironmentVariable("JWT_ISSUER") ?? configuration["Jwt:Issuer"],
                Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? configuration["Jwt:Audience"]
            ))
            .AddSingleton<JwtService>();


        var app = builder.Build();


        // de sters in prod, load balancere, scale set etc
        using (var scope = app.Services.CreateScope())
        {
            scope.ServiceProvider.GetService<VacationRequestsDBContext>().Database.EnsureCreated();
        }
        //


        // Configure the HTTP request pipeline.
        //if (app.Environment.IsDevelopment())
        //{
        app.UseSwagger();
        app.UseSwaggerUI();
        //}

        app.MapHealthChecks("/hc", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
        app.MapHealthChecks("/liveness", new HealthCheckOptions
        {
            Predicate = r => r.Name.Contains("self")
        });

        app.UseHttpsRedirection();
        app.UseCors();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();


        //seed users
        // await app.Services.GetRequiredService<UsersSeed>().Seed();

        app.Run();


        static void AddDataAccessEF(WebApplicationBuilder builder)
        {
            var services = builder.Services;
            var configuration = builder.Configuration;

            var connectionString = Environment.GetEnvironmentVariable("VacationsModuleDB") ??
                                   configuration["ConnectionStrings:VacationsModuleDB"];

            services.AddDbContext<VacationRequestsDBContext>(options =>
            {
                options.UseSqlServer(connectionString,
                    opt =>
                    {
                        opt.MigrationsAssembly(typeof(VacationRequestsDBContext).Assembly.FullName);
                        opt.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(4), errorNumbersToAdd: null);
                    }
                );
                //    options.EnableSensitiveDataLogging();
            });
        }

        static void AddIdentityJWTAuth(WebApplicationBuilder builder)
        {
            var services = builder.Services;
            var configuration = builder.Configuration;


            if (!testingEnvironment)
            {
                services.AddIdentity<ApplicationUser, ApplicationRole>(opts => { opts.Stores.ProtectPersonalData = false; })
                    .AddEntityFrameworkStores<VacationRequestsDBContext>()
                    .AddDefaultTokenProviders();
            }
            
        

            services.Configure<IdentityOptions>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 3;

                options.SignIn.RequireConfirmedEmail = false;
            });


            services.AddAuthentication(options =>
            {
                //Ca sa nu returneze 404 cand nu e autorizat etc in special
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    // De schimbat cu Azure Key Vault

                    var jwtSecret = Environment.GetEnvironmentVariable("JwtSecret") ??
                                    configuration["Jwt:Secret"];

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        //Valideaza faptul ca payload-ul din Token a fost semnat cu secretul 
                        //disponibil pe server si nu a fost modificat
                        ValidateIssuerSigningKey = true,
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ValidateLifetime = true,
                        ValidIssuer = Environment.GetEnvironmentVariable("JwtIssuer") ?? configuration["Jwt:Issuer"],
                        ValidAudience = Environment.GetEnvironmentVariable("JwtAudience") ?? configuration["Jwt:Audience"],
                        IssuerSigningKey =
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                    };
                    
                });
        }


        static void AddSwagger(WebApplicationBuilder builder)
        {
            var services = builder.Services;
            services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new OpenApiInfo { Title = "VacationsModule.API", Version = "v1" });
                opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });
                opt.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                opt.IncludeXmlComments(xmlPath);
                //enum friendly names
                // opt.SchemaFilter<EnumSchemaFilter>();
            });
        }

        static void AddApiVersioning(WebApplicationBuilder builder)
        {
            var services = builder.Services;

            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
            });

        }

        void AddCors(WebApplicationBuilder builder)
        {
            var services = builder.Services;

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.AllowAnyOrigin();
                        builder.AllowAnyHeader();
                        builder.AllowAnyMethod();
                    });
            });
        }

        void AddHealthChecks(WebApplicationBuilder builder)
        {
            var configuration = builder.Configuration;

            builder.Services.AddHealthChecks()
                .AddSqlServer(configuration.GetConnectionString("SqlServer"), failureStatus: HealthStatus.Degraded);
        }

        void AddMediatR(WebApplicationBuilder builder)
        {
            builder.Services.AddMediatR((config) =>
            {
                config.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
            });
        }


        void AddAutoMapper(WebApplicationBuilder builder)
        {
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        }
    }
}