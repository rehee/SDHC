using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Core.Models;
namespace UmbracoWeb.Areas.Test.Controllers
{
  public class HomeController : Controller
  {
    // GET: Test/Home
    public ActionResult Index()
    {
      var content = E.HomePage;
      content.GetValueNull<string>("");
      var c = E.Helper.TypedContent(content.Id);
      var a = c.IContentTo<TT>();
      
      return Content("");
    }
  }

  public class TT:IIdentifyId
  {
    public DateTime p1 { get; set; }
    public int Id { get; set; }
  }
}