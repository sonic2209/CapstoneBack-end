using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESMS.BackendAPI.Services.Notifications;
using ESMS.BackendAPI.ViewModels.Notifications;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ESMS.BackendAPI.Controllers
{
    [EnableCors("MyPolicy")]
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
        public async Task<IActionResult> SendMessage(string topic, [FromBody] NotificationContent noti)
        {
            var result = await _notificationService.SendMessage(topic, noti);         
            return Ok(result);
        }

        [HttpPost("subscription")]
        public async Task<IActionResult> Subscribe(string token, string topic)
        {
            var result = await _notificationService.Subscribe(token, topic);
            return Ok(result);
        }
        [HttpPost("unsubscription")]
        public async Task<IActionResult> Unsubscribe(string token, string topic)
        {
            var result = await _notificationService.Unsubscribe(token, topic);
            return Ok(result);
        }
    }
    }

