using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bilomax.Generic.Infrastructure.Email;
using Bilomax.Generic.Infrastructure.Logging;

namespace Status
{
   public static class StateLogger
    {
       public static void Print(string state){
           Console.WriteLine(state);
       }

       public static async Task SendAlertNotification(List<State> stateBatch)
       {
           StringBuilder b = new StringBuilder();
           stateBatch.ForEach(
               r=>b.Append(
                   string.Concat(
                   r.Url,r.Status)));
           IEmailService service = EmailServiceFactory.GetEmailService();
           service.SendMail("alexs@hdwih.com","bizoton19@gmail.com","site status alerts",b.ToString() ,"");
           
       }
    }
}
