using VacationsModule.Domain.Datamodels;
using VacationsModule.Infrastructure.Data_Access;
using VacationsModule.Infrastructure.Data_Access.v1;
using VacationsModule.UnitTests.DatabaseTestHelpers;

namespace VacationsModule.UnitTests.Data;

public class VacationRequestsRepositoryTests
{
    private InMemoryDbInstance inMemoryDbInstance;

    [SetUp]
    public void Setup()
    {
        this.inMemoryDbInstance = new InMemoryDbInstance();
        DatabaseInitializer.Initialize(inMemoryDbInstance.GetVacationRequestsDBContext());
    }
    
    [Test]
    public async Task Given_VacationRequestsRepository_When_GetVacationRequestsIsCalled_ShouldReturnTheExistingVacationRequest()
    {
        var dbContext = this.inMemoryDbInstance.GetVacationRequestsDBContext();


        var requestId = Guid.Parse("3b5ff972-894d-4785-8181-8a76264aa11f");
        var employeeId = Guid.Parse("95f8e50e-32e6-4f3e-b229-86c89089963c");
        var vacationRequestsRepository = new VacationRequestsRepository(dbContext, logger: null,
            unitOfWork: new UnitOfWork(logger: null));
        
        var result = await vacationRequestsRepository.GetVacationRequestByEmployeeIdAndVacationRequestId(requestId, employeeId: employeeId);
        
        Assert.AreEqual(requestId, result.Id);
    }
    
    
    //GetVacationRequestsByStatusAndEmployeeIdPaginated with status VacationRequestStatus.Approved should return 0 vacations requests for the seed data
    [Test]
    public async Task Given_VacationRequestsRepository_When_GetVacationRequestsByStatusAndEmployeeIdPaginatedIsCalledWithStatusApproved_ShouldReturn0VacationRequests()
    {
        var dbContext = this.inMemoryDbInstance.GetVacationRequestsDBContext();

        var vacationRequestsRepository = new VacationRequestsRepository(dbContext, logger: null,
            unitOfWork: new UnitOfWork(logger: null));
        
        var result = await vacationRequestsRepository.GetVacationRequestsByStatusAndEmployeeIdPaginated(VacationRequestStatus.Approved, null);
        
        Assert.AreEqual(0, result.Count);
    }
    
    //GetVacationRequestsByStatusAndEmployeeIdPaginated with status VacationRequestStatus.Pending should return 1 vacations requests for the seed data set
    
    [Test]
    public async Task Given_VacationRequestsRepository_When_GetVacationRequestsByStatusAndEmployeeIdPaginatedIsCalledWithStatusPending_ShouldReturn1VacationRequests()
    {
        var dbContext = this.inMemoryDbInstance.GetVacationRequestsDBContext();

        var vacationRequestsRepository = new VacationRequestsRepository(dbContext, logger: null,
            unitOfWork: new UnitOfWork(logger: null));
        
        var result = await vacationRequestsRepository.GetVacationRequestsByStatusAndEmployeeIdPaginated(VacationRequestStatus.Pending, null);
        
        Assert.AreEqual(1, result.Count);
    }

}