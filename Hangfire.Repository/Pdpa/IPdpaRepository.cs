using Hangfire.Models.Entities;

namespace Hangfire.Repositories.Interface
{
    public interface IPdpaRepository
    {
        Task Add(TempDwhPdpa obj);
        Task Add(TdrPdpaDelLog obj);
        Task<List<TempDwhPdpa>> Get(DateTime asOfDate);
        TempDwhPdpa? Get(TempDwhPdpa obj);
        string? findCustName(TempDwhPdpa obj);
        Task Delete(string accountSuffix);
        Task DeleteAll(List<TempDwhPdpa> list);
        Task<List<TdrPdpaDelLog>> GetLogPdpa(DateTime asOfDate);
    }
}
