using VirtualNote.Database;

namespace VirtualNote.Kernel.Contracts
{
    public interface IRepositoryService
    {
        IRepository Repository { get; }
    }
}
