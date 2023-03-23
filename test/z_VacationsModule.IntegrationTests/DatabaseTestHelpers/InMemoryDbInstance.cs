using Microsoft.EntityFrameworkCore;
using VacationsModule.Infrastructure.Data_Access;

namespace VacationsModule.UnitTests.DatabaseTestHelpers;

public class InMemoryDbInstance : IDisposable
{
    
    private VacationRequestsDBContext _context;
    
    public VacationRequestsDBContext GetVacationRequestsDBContext()
    {

        if (_context != null)
        {
            return _context;
        }
        
        var options = new DbContextOptionsBuilder<VacationRequestsDBContext>()
            .UseInMemoryDatabase(databaseName: "VacationRequestsDB" + Guid.NewGuid())
            .Options;

        _context = new VacationRequestsDBContext(options, mediator: null);

        return _context;
    }
    
    public void Dispose()
    {
        this._context?.Database.EnsureDeleted();
        this._context?.Dispose();
    }
}