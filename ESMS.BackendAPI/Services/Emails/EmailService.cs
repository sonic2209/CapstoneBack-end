﻿using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.Services.Emails
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        public EmailService(IConfiguration config)
        {
            _config = config;
        }
        public void Send(string from, string to, string subject)
        {
            var builder = new BodyBuilder();
            using (StreamReader SourceReader = System.IO.File.OpenText(@"D:\Study\Capstone\email-inlined.html"))
            {
                builder.HtmlBody = SourceReader.ReadToEnd();
                builder.HtmlBody = builder.HtmlBody.Replace("Sometimes you just want to send a simple HTML email", "Sometimes you just want to send a simple ESMS email");
            }
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(from));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = builder.ToMessageBody();

            // send email
            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("dinhbinh599@gmail.com", "thongtinroi");
            //smtp.Connect(_config["Emails:SmtpHost"], int.Parse(_config["Emails:SmtpPort"]), SecureSocketOptions.StartTls);
            //smtp.Authenticate(_config["Emails:SmtpUser"], _config["Emails:SmtpPass"]);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
