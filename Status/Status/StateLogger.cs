using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;

namespace Status
{
   public static class StateLogger
    {
        
        public static void Print(Resource resource, string taskStatus)
        {

            Console.Write("****\n{0} Polling:{1}-{2} ", taskStatus, resource.Name, resource.GetAbsoluteUri());
        }
        public static void Print(State t, string taskStatus)
        {
            Console.WriteLine("|");
            Console.WriteLine("--{0} : {1} - {2} \n{3}", taskStatus, t.Url, t.Status, t.Description);
        }

        public static void SendAlertNotification(IEnumerable<State> stateBatch, string recipients)
       {
           StringBuilder b = new StringBuilder();
            b.AppendLine("The following resources had or have a status change: ");
            stateBatch.ToList().ForEach(
                r => b.AppendLine(String.Format("\r--\rResource URI: {0}\rStatus Code: {1}\rDescription: {2}\rTime: {3}\r--\r",
                r.Url,
                r.Status,
                r.Description,
                DateTime.UtcNow.ToLocalTime().ToString())));
           SendMail("", recipients,"Resource Status Alerts",b.ToString());
           
       }

        public static void SendMail(string from, string to, string subject, string body)
        {
        
            MailMessage message = new MailMessage();
            message.Subject = subject;
            message.Body = body;
            message.To.Add(to);
            
            SmtpClient smtp = new SmtpClient();
            smtp.SendMailAsync(message);
        }
    

    }
}
