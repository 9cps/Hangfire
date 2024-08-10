using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hangfire.Services.Interface
{
    public interface ITestJobs
    {
        public Task<string> SendWelcomeEmail(string email, string name);
    }
}
