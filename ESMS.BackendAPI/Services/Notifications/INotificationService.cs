using ESMS.BackendAPI.ViewModels.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.Services.Notifications
{
    public interface INotificationService
    {
        public void SendMessage(string topic, NotificationContent noti);
        public void Subscribe(string token, string topic);
        public void Unsubscribe(string token, string topic);
    }
}
