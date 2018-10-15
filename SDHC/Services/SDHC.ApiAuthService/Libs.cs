using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpxUmbracoMember.ApiAuthService
{
  public delegate bool CheckApiToken(string token);
  public static class A
  {
    public static ICheckApiToken checkToken { get; set; }
  }
  public class APiAuthModel
  {
    public string UserId { get; set; }
    public string Token { get; set; }
    public object Input { get; set; }
  }
}
