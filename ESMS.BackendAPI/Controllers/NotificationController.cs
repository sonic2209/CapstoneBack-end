using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task<IActionResult> SendMessage()
        {
            var message = new Message()
            {
                Notification = new Notification
                {
                    Title = "Message Title",
                    Body = "Message Body"
                },
                Topic = "news"
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
    }
}
