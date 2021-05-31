using ESMS.BackendAPI.ViewModels.Notifications;
using FirebaseAdmin.Messaging;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.Services.Notifications
{
    public class NotificationService : INotificationService
    {
        private readonly IConfiguration _config;
        public NotificationService (IConfiguration config)
        {
            _config = config;
        }
        
        IFirebaseClient client;
        //public async Task<string> SendMessage(string topic, NotificationContent noti)
        //{
        //    var message = new Message()
        //    {
        //        Notification = new Notification
        //        {
        //            Title = noti.title,
        //            Body = noti.body
        //        },
        //        Topic = topic
        //    };
        //    // Send a message to the devices subscribed to the provided topic.
        //    var result = await FirebaseMessaging.DefaultInstance.SendAsync(message);
        //    return result;
        //}

        public void SendMessage(NotificationContent noti)
        {
            IFirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = _config["Firebase:AuthSecret"],
                BasePath = _config["Firebase:BasePath"]
            };
            client = new FireSharp.FirebaseClient(config);
            PushResponse response = client.Push("fir-4d2be-default-rtdb/", noti);
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
