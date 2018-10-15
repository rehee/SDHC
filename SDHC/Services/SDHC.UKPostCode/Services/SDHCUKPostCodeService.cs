using SpxUmbracoMember.HttpServer;
using SpxUmbracoMember.UKPostCode.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpxUmbracoMember.UKPostCode.Services
{
  public class SDHCUKPostCodeService : IUKPostCodeService
  {
    private ISDHCHttpService httpService;
    public SDHCUKPostCodeService(ISDHCHttpService httpService)
    {
      this.httpService = httpService;
      if (UKPostCodeModel.PostCodeList == null)
      {
        UKPostCodeModel.PostCodeList = new Dictionary<string, PostCodeModel>();
      }
    }
    public PostCodeModel ConvertUKPostCode(string postCode)
    {
      postCode = postCode.Text().Replace(" ", "");
      try
      {
        return UKPostCodeModel.PostCodeList[postCode];
      }
      catch
      {
        return this.GetUKPost(postCode);
      }
    }
    private PostCodeModel GetUKPost(string postCode)
    {
      try
      {
        var url = $"http://api.postcodes.io/postcodes/{postCode}";
        var result = httpService.Get(url);
        var list = result.ConvertJsonToObject<PostCodeModel>();
        if (list.status == 200)
        {
          if (UKPostCodeModel.PostCodeList.ContainsKey(postCode))
          {
            UKPostCodeModel.PostCodeList[postCode] = list;
          }
          else
          {
            UKPostCodeModel.PostCodeList.Add(postCode, list);
          }
        }
        return list;
      }
      catch (Exception ex)
      {
        var c = ex.Message;
      }
      return null;

    }
  }
}
