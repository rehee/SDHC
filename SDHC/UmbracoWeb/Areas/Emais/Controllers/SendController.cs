using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UmbracoWeb.Areas.Emais.Controllers
{
  public class SendController : Controller
  {
    // GET: Emais/Send
    public ActionResult Index()
    {
      var c = E.Services.ContentService.GetById(1);
      
      E.EmailService.SendEmail("x3iii131@gmail.com", "test", "test", "lalala");
      return Content("");
    }
  }
}