using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models;

namespace Umbraco.Core.Services
{
  public static partial class UmbracoContentExtend
  {
    public static void GetBreadcrumb(dynamic input, out List<dynamic> list)
    {
      var page = input;
      var listTemp = new List<dynamic>();
      do
      {
        listTemp.Add(page);
        page = page.Parent();
      } while (page != null);
      listTemp.Reverse();
      list = listTemp;
    }
    public static void GetBreadcrumb(IContent input, out List<dynamic> list)
    {
      var page = input;
      var listTemp = new List<dynamic>();
      do
      {
        listTemp.Add(page);
        page = page.Parent();
      } while (page != null);
      listTemp.Reverse();
      list = listTemp;
    }
    public static List<int> GetBreadCrumbFunction(int id, Func<int, IContent> getContentById)
    {
      var list = new List<int>();
      do
      {
        var content = getContentById(id);
        if (content == null)
          break;
        list.Add(id);
        if (content.ParentId <= 100)
          break;
        id = content.ParentId;
      } while (true);
      list.Reverse();
      return list;
    }
  }
}

