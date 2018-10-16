using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SDHC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Core;
using Umbraco.Core.Models;
namespace UmbracoWeb.Areas.Test.Controllers
{
  public class HomeController : Controller
  {
    // GET: Test/Home
    public ActionResult Index()
    {
      //var content = E.HomePage;
      //content.GetValueNull<string>("");
      //var c = E.Helper.TypedContent(content.Id);
      //var a = c.IContentTo<TT>();
      var a = typeof(TT);
      var value = a.GetDefaultValue();
      var test2 = new Test2();
      var convertResult = test2.MyConvertTo<Test1>();
      var convertBackT2 = convertResult.MyConvertTo<Test2>();
      var t1Again = convertBackT2.MyConvertTo<Test1>();
      return Content("");
    }
  }

  public abstract class TestBase
  {
    
    public string aa { get; set; } = "123";
    [Json]
    public virtual string p1 { get; set; }
  }
  public class Test1 : TestBase
  {
    
  }
  public class Test2 : TestBase
  {
    [Json]
    public new TT p1 { get; set; } = new TT();
  }

  public class TT : IIdentifyId
  {
    public DateTime p1 { get; set; }
    public decimal p2 { get; set; }
    public int p3 { get; set; }
    public decimal p4 { get; set; }

    public int Id { get; set; }
  }
}