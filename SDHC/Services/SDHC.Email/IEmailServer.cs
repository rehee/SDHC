using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpxUmbracoMember.Email
{
  public interface IEmailServer
  {
    void ResetSMTP(string host, string port, string user, string password, bool ssl);
    void SendEmail(string toUser, string title, string body, string fromUser);
  }
}
