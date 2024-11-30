namespace Hangfire.Services.Interface
{
    public interface IPdpaService
    {
        public void DldPdpaProcessManualInitialize(string dateManual);
        public void DldPdpaProcessManual(string dateManual);
        public void UpdPdpaProcess();
        public void DldPdpaProcess();
    }
}
