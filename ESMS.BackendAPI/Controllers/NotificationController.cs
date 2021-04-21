using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESMS.BackendAPI.Services.Notifications;
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
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        [HttpPost]
        public IActionResult SendMessage(string topic, [FromBody] NotificationContent noti)
        {
             _notificationService.SendMessage(topic, noti);         
            return NoContent();
        }

        [HttpPost("subscription")]
        public IActionResult Subscribe(string token, string topic)
        {
            _notificationService.Subscribe(token, topic);
            return NoContent();
        }
        [HttpPost("unsubscription")]
        public IActionResult Unsubscribe(string token, string topic)
        {
            _notificationService.Unsubscribe(token, topic);       
            return NoContent();
        }
    }
    }

