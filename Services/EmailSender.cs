using Newtonsoft.Json.Linq;
using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Model;
using System.Diagnostics;

namespace final_project_depi.Services
{
    public class EmailSender
    {

        public static void SendEmail(string senderName , string senderEmail, string toName, string toEmail, string textContent, string subject)
        {
            var apiInstance = new TransactionalEmailsApi();
           
            SendSmtpEmailSender Email = new SendSmtpEmailSender(senderName, senderEmail);
            SendSmtpEmailTo smtpEmailTo = new SendSmtpEmailTo(toEmail, toName);
            List<SendSmtpEmailTo> To = new List<SendSmtpEmailTo>();
            To.Add(smtpEmailTo);
           
            
            try
            {
                var sendSmtpEmail = new SendSmtpEmail(Email, To, null, null, null, textContent, subject);
                CreateSmtpEmail result = apiInstance.SendTransacEmail(sendSmtpEmail);
                Debug.WriteLine("Email Sender pk: \n" + result.ToJson());
                
            }
            catch (Exception e)
            {
                Console.WriteLine("Email Sender Failed: \n" + e.Message);
            }
        }
    }
}
