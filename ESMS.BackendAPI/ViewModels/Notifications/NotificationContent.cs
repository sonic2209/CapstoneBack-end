using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Notifications
{
    public class NotificationContent
    {
        public string title { get; set; }
        public string body { get; set; }
        public bool status { get; set; } = true;
        public string topic { get; set; }
        public DateTime dateCreate { get; set; }
    }
}
