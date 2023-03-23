using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VacationsModule.Application.DTOs.Auth;
using VacationsModule.Domain.Entities;
using VacationsModule.Infrastructure.Data_Access;
using VacationsModule.IntegrationTests.DatabaseTestHelpers;

namespace VacationsModule.IntegrationTests
{
    public class IntegrationTest
    {
        protected readonly HttpClient TestClient;
        protected readonly WebApplicationFactory<Program> Factory;

        protected IntegrationTest()
        {
            var appFactory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.UseEnvironment("Testing");
                    
                    builder.ConfigureServices(services =>
                    {
                        // unregister la baza de date uzuala

                        var context = services.FirstOrDefault(descriptor =>
                            descriptor.ServiceType == typeof(VacationRequestsDBContext));
                        if (context != null)
                        {
                            services.Remove(context);
                            var options = services.Where(r => (r.ServiceType == typeof(DbContextOptions))
                                                              || (r.ServiceType.IsGenericType &&
                                                                  r.ServiceType.GetGenericTypeDefinition() ==
                                                                  typeof(DbContextOptions<>))).ToArray();
                            foreach (var option in options)
                            {
                                services.Remove(option);
                            }
                        }


                        var dbId = Guid.NewGuid();
                        // adaugam context cu bd in memory
                        services.AddDbContext<VacationRequestsDBContext>(
                            opt => opt.UseInMemoryDatabase($@"TestDB{dbId}"));
                        
                        
                        services.AddIdentity<ApplicationUser, ApplicationRole>(opts => { opts.Stores.ProtectPersonalData = false; })
                            .AddEntityFrameworkStores<VacationRequestsDBContext>()
                            .AddDefaultTokenProviders();

                        var serviceProvider = services.BuildServiceProvider();
                        using var scope = serviceProvider.CreateScope();

                        var dbContext = scope.ServiceProvider.GetRequiredService<VacationRequestsDBContext>();
                        // Ensure the database is created.
                        dbContext.Database.EnsureCreated();
                        // Seed the database with test data.
                        SeedDataAsync(dbContext).Wait();
                        
                        Debugger.Break();
                    });
                });


            Factory = appFactory;
            TestClient = appFactory.CreateClient();
        }

        protected async Task AuthenticateAsync(string usernameOrEmail, string password)
        {
            TestClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", await GetJwtAsync(usernameOrEmail, password));
        }

        //protected async Task<PostResponse> CreatePostAsync(CreatePostRequest request)
        //{
        //    var response = await TestClient.PostAsJsonAsync(ApiRoutes.Posts.Create, request);
        //    return await response.Content.ReadAsAsync<PostResponse>();
        //}

        private async Task<string> GetJwtAsync(string usernameOrEmail, string password)
        {
            var loginData = new LoginUserDataModel()
            {
                UserNameOrEmail = usernameOrEmail,
                Password = password
            };
            var response = await TestClient.PostAsJsonAsync("api/v1.0/Account/login", loginData,
                new JsonSerializerOptions(JsonSerializerDefaults.General)
                );

            var registrationResponse = await response.Content.ReadFromJsonAsync<UserProfileDetailsApiModel>();

            return registrationResponse.Token;
        }

        protected async Task SeedDataAsync(VacationRequestsDBContext context)
        {
            DatabaseInitializer.Initialize(context);
        }
    }
}