using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpxUmbracoMember.HttpServer
{
  public interface ISDHCHttpService
  {
    string CurrentUrl();
    string Post(string url, string date, string ContentType = "application/json");
    string Get(string url, string ContentType = "application/json");
  }
}
