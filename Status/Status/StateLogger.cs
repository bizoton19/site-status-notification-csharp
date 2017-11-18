using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;

namespace Status
{
   

    public abstract class StateLogger
    {
        private MODE _mode;
        public StateLogger(MODE mode)
        {
            _mode = mode;
        }
        public abstract StateLogger Save(Resource resource, string taskStatus);
        
        public virtual StateLogger Log (Resource resource, string taskStatus )
        {
            Console.Write("****\n{0} Polling:{1}-{2} ", taskStatus, resource.Name, resource.GetAbsoluteUri());
            return this;
        }
        public virtual StateLogger Log(State t, string taskStatus)
        {
          
                Console.WriteLine("|");
                Console.WriteLine("--{0} : {1} - {2} \n{3}", taskStatus, t.Url, t.Status, t.Description);
            
            return this;
        }
        public virtual StateLogger Log(IEnumerable<State> stateBatch, string recipients)
        {
            if (_mode == MODE.Background)
            {
                SendAlertNotification(stateBatch, recipients);
            }
            else
            {
                stateBatch.ToList().ForEach(x =>
                {
                    Print(x, x.Status);
                });
            }
            return this;
        }

       
        private  void Print(State t, string taskStatus)
        {
            Console.WriteLine("|");
            Console.WriteLine("--{0} : {1} - {2} \n{3}", taskStatus, t.Url, t.Status, t.Description);
        }

       
        public void SendAlertNotification(IEnumerable<State> stateBatch, string recipients)
        {
           
            stateBatch.ToList().ForEach(x=>
                {
                    SendOneAlert(x, recipients);
                }
              );

        }
        private void SendOneAlert(State state, string recipients)
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine("The following resources had or have a status change: ");
            b.AppendLine(
                $"\r--\rResource URI: {state.Url}\rStatus Code: {state.Status}\rDescription: {state.Description}\rTime: {DateTime.UtcNow.ToLocalTime().ToString()}\r--\r");

            SendMail("", recipients, "Resource Status Alerts", b.ToString());
        }
        private void SendMail(string from, string to, string subject, string body)
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
