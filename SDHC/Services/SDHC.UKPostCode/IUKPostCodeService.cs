using SpxUmbracoMember.UKPostCode.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpxUmbracoMember.UKPostCode
{
  public interface IUKPostCodeService
  {
    PostCodeModel ConvertUKPostCode(string postCode);
  }
}
