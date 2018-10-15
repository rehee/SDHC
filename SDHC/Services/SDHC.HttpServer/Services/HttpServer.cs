using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SpxUmbracoMember.HttpServer.Services
{
  public class HttpServer : ISDHCHttpService
  {
    public string CurrentUrl()
    {
      return "";
    }
    public string Get(string url, string ContentType = "application/json")
    {
      HttpClient request = new HttpClient();
      try
      {
        HttpResponseMessage result = request.GetAsync(url).Result;
        return result.Content.ReadAsStringAsync().Result.ToString();
      }
      catch (Exception ex)
      {
        return ex.Message;
      }
    }
    public string Post(string url, string date, string ContentType = "application/json")
    {
      HttpClient request = new HttpClient();
      try
      {
        HttpResponseMessage result = request.PostAsync(url, new StringContent(date, Encoding.UTF8, ContentType)).Result;
        return result.Content.ReadAsStringAsync().Result.ToString();
      }
      catch (Exception ex)
      {
        return ex.Message;
      }
    }
  }
}
