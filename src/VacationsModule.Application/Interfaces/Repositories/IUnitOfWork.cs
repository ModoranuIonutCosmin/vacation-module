using VacationsModule.Application.Interfaces.Repositories.Base;

namespace VacationsModule.Application.Interfaces.Repositories;

public interface IUnitOfWork
{
    void Register(IRepository repository);

    Task CommitTransaction();
}