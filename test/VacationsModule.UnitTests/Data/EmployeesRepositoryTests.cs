using VacationsModule.Infrastructure.Data_Access;
using VacationsModule.Infrastructure.Data_Access.v1;
using VacationsModule.UnitTests.DatabaseTestHelpers;

namespace VacationsModule.UnitTests.Data;

public class EmployeesRepositoryTests
{
    private InMemoryDbInstance inMemoryDbInstance;

    [SetUp]
    public void Setup()
    {
        this.inMemoryDbInstance = new InMemoryDbInstance();
        
        DatabaseInitializer.Initialize(inMemoryDbInstance.GetVacationRequestsDBContext());
    }
    
    //GetEmployeeByUserIdEagerAsync with id 95f8e50e-32e6-4f3e-b229-86c89089963c should return a not null employee
    [Test]
    public async Task Given_EmployeesRepository_When_GetEmployeeByUserIdEagerAsyncIsCalledWithIdOfExistingUser_ShouldReturnANotNullEmployee()
    {
        var dbContext = this.inMemoryDbInstance.GetVacationRequestsDBContext();
        var employeesRepository = new EmployeesRepository(dbContext, logger: null,
            unitOfWork: new UnitOfWork(logger: null));
        
        var result = await employeesRepository.GetEmployeeByUserIdEagerAsync(Guid.Parse("95f8e50e-32e6-4f3e-b229-86c89089963c"));
        
        Assert.NotNull(result);
    }
    
    //GetEmployeeByUserIdEagerAsync with inexistent id should return default
    [Test]
    public async Task Given_EmployeesRepository_When_GetEmployeeByUserIdEagerAsyncIsCalledWithIdOfNonExistingUser_ShouldReturnANullEmployee()
    {
        var dbContext = this.inMemoryDbInstance.GetVacationRequestsDBContext();
        var employeesRepository = new EmployeesRepository(dbContext, logger: null,
            unitOfWork: new UnitOfWork(logger: null));
        
        var result = await employeesRepository.GetEmployeeByUserIdEagerAsync(Guid.Parse("ffffe50e-32e6-4f3e-b229-86c8ffff963c"));
        
        Assert.Null(result);
    }
    

}