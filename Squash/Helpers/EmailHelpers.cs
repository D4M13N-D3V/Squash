using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Configuration;

namespace Squash.Helpers
{
    public class EmailInformation
    {
        public List<string> Reciepents = new List<string>();
        public string Title = "";
        public string Body = "";
    }
        public static class EmailHelpers
        {
        public static bool SendEmail(EmailInformation info)
        {
            using (var msg = new MailMessage())
            {
                foreach (var email in info.Reciepents)
                {
                    msg.To.Add(new MailAddress(email));
                }
                msg.From = new MailAddress(ConfigurationManager.AppSettings.Get("EmailUsername"));
                msg.Subject = info.Title;
                msg.Body = info.Body.ToString();
                msg.IsBodyHtml = true;

                var client = new SmtpClient
                {
                    Host = ConfigurationManager.AppSettings.Get("SMTPServer"),
                    Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings.Get("EmailUsername"), ConfigurationManager.AppSettings.Get("EmailPassword")),
                    Port = Int32.Parse(ConfigurationManager.AppSettings.Get("SMTPPort")),
                    EnableSsl = true
                };

                // You can use Port 25 if 587 is blocked 
                try
                {
                    client.Send(msg);
                    return true;
                }
                catch (SmtpException smtpEx)
                {
                    return false;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
    }
}