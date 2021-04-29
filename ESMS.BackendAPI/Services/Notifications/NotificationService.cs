using ESMS.BackendAPI.ViewModels.Notifications;
using FirebaseAdmin.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.Services.Notifications
{
    public class NotificationService : INotificationService
    {
        public async Task<string> SendMessage(string topic, NotificationContent noti)
        {
            var message = new Message()
            {
                Notification = new Notification
                {
                    Title = noti.Title,
                    Body = noti.Body
                },
                Topic = topic
            };
            // Send a message to the devices subscribed to the provided topic.
            var result = await FirebaseMessaging.DefaultInstance.SendAsync(message);
            return result;
        }

        public async Task<TopicManagementResponse> Subscribe(string token, string topic)
        {
            var result = await FirebaseMessaging.DefaultInstance.SubscribeToTopicAsync(new[] { token }, topic);
            return result;
        }

        public async Task<TopicManagementResponse> Unsubscribe(string token, string topic)
        {
            var result = await FirebaseMessaging.DefaultInstance.UnsubscribeFromTopicAsync(new[] { token }, topic);
            return result;
        }
    }
}
