using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpxUmbracoMember.ApiAuthService
{
  public class ApiAuthService : Attribute, ICheckApiToken
  {
    public ApiAuthService()
    {
      this.IsTokenValue = (string input) =>
      {
        if (G.Text(input) == "")
          return false;
        return true;
      };
    }
    public CheckApiToken IsTokenValue { get; set; }
  }
}
