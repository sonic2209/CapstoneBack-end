using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.Services.Emails
{
    public interface IEmailService
    {
        public void Send(string from, string to, string password, string name);
    }
}
