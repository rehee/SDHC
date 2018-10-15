using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models;
using Umbraco.Core.Strings;

namespace Umbraco.Core.Services
{
  public static partial class UmbracoContentExtend
  {
    public static DefaultUrlSegmentProvider urlSegmentProvider { get; set; } = new DefaultUrlSegmentProvider();
    public static string RelativeUrl(this IContent content)
    {
      var pathParts = new List<string>();
      var n = content;
      while (n != null)
      {
        pathParts.Add(n.UrlName());
        n = n.Parent();
      }
      pathParts.RemoveAt(pathParts.Count() - 1); //remove root node
      pathParts.Reverse();
      var path = "/" + string.Join("/", pathParts);
      return path;
    }
    public static string UrlName(this IContent content)
    {
      return urlSegmentProvider.GetUrlSegment(content).ToLower();
    }
    public static string GetStringPropertyIncludeParent(this IContent page, string propertyAltName)
    {
      var result = page.GetValueNull<string>(propertyAltName);
      if (String.IsNullOrEmpty(result))
      {
        if (page.Parent() == null)
        {
          return "";
        }
        return page.Parent().GetStringPropertyIncludeParent(propertyAltName);
      }
      return result;
    }
  }
}
