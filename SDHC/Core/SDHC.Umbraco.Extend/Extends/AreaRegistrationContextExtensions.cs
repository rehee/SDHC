using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Routing;

namespace System.Web.Mvc
{
  public static class AreaRegistrationContextExtensions
  {
    public static Route MapHttpRoute(this AreaRegistrationContext context, string name, string routeTemplate)
    {
      return context.MapHttpRoute(name, routeTemplate, null, null);
    }

    public static Route MapHttpRoute(this AreaRegistrationContext context, string name, string routeTemplate, object defaults)
    {
      return context.MapHttpRoute(name, routeTemplate, defaults, null);
    }

    public static Route MapHttpRoute(this AreaRegistrationContext context, string name, string routeTemplate, object defaults, object constraints)
    {
      var route = context.Routes.MapHttpRoute(name, routeTemplate, defaults, constraints);
      if (route.DataTokens == null)
      {
        route.DataTokens = new RouteValueDictionary();
      }
      route.DataTokens.Add("area", context.AreaName);
      return route;
    }
  }

}
