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
        private IResourceRepository _repo;
        public StateLogger(MODE mode)
        {
            _mode = mode;
        }
        public StateLogger(MODE mode,IResourceRepository repo)
        {
            _mode = mode;
            _repo = repo;
        }
        
        
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
        /// <summary>
        /// Depending on the _mode variable, the function will either print the log status or send an alert notification via email
        /// </summary>
        /// <param name="stateBatch"></param>
        /// <param name="recipients"></param>
        /// <returns></returns>
        public virtual async Task<StateLogger> Log(ICollection<State> stateBatch, string recipients)
        {
            if (_mode == MODE.Background)
            {
                SendAlertNotification(stateBatch, recipients);
                    if (_repo != null)
                    {
                        await _repo.Save(stateBatch);
                    }
                    else
                    {
                        throw new Exception("Repository Object IStateRepository is not instantiated");
                    }
                
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
            StringBuilder msg = new StringBuilder();
            msg.AppendLine("The following resources had or have a status change: ");
            msg.Append(buildMessagingTemplate(stateBatch, msg));
            SendOneAlert(msg.ToString(), recipients);
     
        }

        public string buildMessagingTemplate(IEnumerable<State> stateBatch,StringBuilder msg)
        {
            
            stateBatch.ToList().ForEach(x =>
            {
                msg.AppendLine(
                    $"\r--\r\rResource Type: {x.Type}\r[[Resource URI: {x.Url}]---\rStatus Code: {x.Status}\rDescription: {x.Description}\rTime (UTC Local Time): {DateTime.UtcNow.ToLocalTime().ToString()}\r--\r");
            });

            return msg.ToString();
        }
        private void SendOneAlert(string currPollState, string recipients)
        {
            
            SendMail("", recipients, "Resource Status Alerts",currPollState);
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
