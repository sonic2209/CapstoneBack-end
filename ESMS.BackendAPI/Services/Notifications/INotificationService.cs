using ESMS.BackendAPI.ViewModels.Notifications;
using FirebaseAdmin.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.Services.Notifications
{
    public interface INotificationService
    {
        Task<string> SendMessage(string topic, NotificationContent noti);
        Task<TopicManagementResponse> Subscribe(string token, string topic);
        Task<TopicManagementResponse> Unsubscribe(string token, string topic);
    }
}
