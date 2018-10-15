using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SpxUmbracoMember.Email.Services
{
  public class EmailServices : IEmailServer
  {
    private string host { get; set; }
    private int ports { get; set; }
    private string user { get; set; }
    private string password { get; set; }
    private bool ssl { get; set; }


    public EmailServices()
    {
    }
    public EmailServices(string host, int ports, string user, string password, bool ssl)
    {
      this.host = host;
      this.ports = ports;
      this.user = user;
      this.password = password;
      this.ssl = ssl;
    }
    public void SendEmail(string toUser, string title, string body, string fromUser)
    {
      var mailToArray = new List<string>() { toUser.Text() };
      var mailSubject = title.Text();
      var mailBody = body.Text();
      var isbodyHtml = true;
      MailAddress maddr = new MailAddress(user, fromUser);
      MailMessage myMail = new MailMessage();
      if (mailToArray != null)
      {
        for (int i = 0; i < mailToArray.Count; i++)
        {
          myMail.To.Add(mailToArray[i].ToString());
        }
      }
      myMail.Subject = mailSubject;
      myMail.From = maddr;
      myMail.Body = mailBody;
      myMail.BodyEncoding = Encoding.Default;
      myMail.Priority = MailPriority.High;
      myMail.IsBodyHtml = isbodyHtml;
      myMail.SubjectEncoding = System.Text.Encoding.UTF8;
      myMail.BodyEncoding = System.Text.Encoding.UTF8;
      SmtpClient smtp = this.GetSMTP();

      try
      {
        smtp.Send(myMail);
      }
      catch
      {
        try
        {
          smtp.Dispose();
        }
        catch { }
      }

    }
    public void ResetSMTP(string host, string port, string user, string password, bool ssl)
    {
      this.host = host.Text();
      this.ports = port.Int32(0);
      this.user = user.Text();
      this.password = password.Text();
      this.ssl = ssl;
    }
    private SmtpClient GetSMTP()
    {
      var smtp = new SmtpClient();
      smtp.EnableSsl = true;
      smtp.Host = host;
      smtp.Port = ports;
      smtp.UseDefaultCredentials = false;
      smtp.Credentials = new System.Net.NetworkCredential(user, password);
      smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
      smtp.EnableSsl = this.ssl;
      smtp.SendCompleted += (s, e) => { smtp.Dispose(); };
      return smtp;
    }


  }

}
