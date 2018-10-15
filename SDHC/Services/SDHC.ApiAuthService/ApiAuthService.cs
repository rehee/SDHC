using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpxUmbracoMember.ApiAuthService
{
  public interface ICheckApiToken
  {
    CheckApiToken IsTokenValue { get; set; }
  }
}
