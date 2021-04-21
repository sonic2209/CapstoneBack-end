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
        public async void SendMessage(string topic, NotificationContent noti)
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
            await FirebaseMessaging.DefaultInstance.SendAsync(message);
        }

        public async void Subscribe(string token, string topic)
        {
            await FirebaseMessaging.DefaultInstance.SubscribeToTopicAsync(new[] { token }, topic);
        }

        public async void Unsubscribe(string token, string topic)
        {
            await FirebaseMessaging.DefaultInstance.UnsubscribeFromTopicAsync(new[] { token }, topic);
        }
    }
}
