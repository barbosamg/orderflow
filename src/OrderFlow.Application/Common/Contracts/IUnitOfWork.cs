namespace OrderFlow.Application.Common.Contracts;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken);
}