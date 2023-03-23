using System.Net;
using System.Net.Http.Json;
using Newtonsoft.Json;
using VacationsModule.Application.DTOs;

namespace VacationsModule.IntegrationTests.EmployeesService;

public class EmployeeServiceIntegrationTest : IntegrationTest
{
    [Test]
    public async Task Given_EmployeeController_When_GetAvailableDays_Then_ReturnsCorrectAvailableDays()
    {
        // Login as seeded manager

        // await this.SeedDataAsync();
        
        //TODO: Fix 

        await this.AuthenticateAsync("manageruser01", "string123"); //works
        
        var response = await this.TestClient.GetFromJsonAsync<GetAvailableVacationDaysResponse>("api/v1.0/Employee/available-vacation-days");
        
        // 404 Not found? (endpoint not found?)
        
        var result = JsonConvert.DeserializeObject<GetAvailableVacationDaysResponse>(response.ToString());
        
        Assert.AreEqual(25, result.AvailableVacationDays);
    }
}