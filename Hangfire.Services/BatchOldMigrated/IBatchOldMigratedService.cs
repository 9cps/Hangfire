namespace Hangfire.Services.Interface
{
    public interface IBatchOldMigratedService
    {
        public void DoWorkOldBatchProcess();
        // public Task<bool> CloseBranch();
    }
}
