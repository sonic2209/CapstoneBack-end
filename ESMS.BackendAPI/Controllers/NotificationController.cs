using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESMS.BackendAPI.ViewModels.Notifications;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ESMS.BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> SendMessage(string topic, [FromBody] NotificationContent noti)
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
            string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
            // Response is a message ID string.
            Console.WriteLine("Successfully sent message: " + response);
            return NoContent();
        }

        [HttpPost("subscription")]
        public async Task<IActionResult> Subscribe(string token, string topic)
        {
            await FirebaseMessaging.DefaultInstance.SubscribeToTopicAsync(new[] { token }, topic);
            return NoContent();
        }
        [HttpPost("unsubscription")]
        public async Task<IActionResult> Unsubscribe(string token, string topic)
        {
            await FirebaseMessaging.DefaultInstance.UnsubscribeFromTopicAsync(new[] { token }, "topic");          
            return NoContent();
        }
    }
    }

