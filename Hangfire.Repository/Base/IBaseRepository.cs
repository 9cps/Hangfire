using Hangfire.Models.Dto;

namespace Hangfire.Repositories.Interface
{
    public interface IBaseRepository
    {
        public List<T> CallStoredProcedure<T>(string spName, List<ParameterOfStoredProcedure>? listParams = null) where T : class, new();
        public void CallStoredProcedure(string spName, List<ParameterOfStoredProcedure>? listParams = null);
        Task BatchExceptionLog(string stepJob, string remark, string? status = null);
    }
}
