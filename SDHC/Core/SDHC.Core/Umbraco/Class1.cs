using System;
using System.Collections.Generic;
using System.Text;

namespace Spxus.Core.Umbraco
{
  public interface IUmbracoContent
  {
    void CreatePage(int parentId, string pageName, string pageAlt, Dictionary<string, dynamic> model, out int pageId, out MethodResponse response);
    IEnumerable<dynamic> SearchChildContent(int parentid, Func<dynamic, bool> where, bool desc, Func<dynamic, dynamic> order, int index, int pageSize, out int total);
    dynamic GetContentById(int id);
  }
}
