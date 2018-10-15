using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SpxUmbracoMember.Calture.Services
{
  public class CaltureService : ICaltureService
  {
    public int DefaultCalture { get; set; }
    public void ChangeCalture(int type)
    {
      try
      {
        HttpContext.Current.Session["calture"] = type;
      }
      catch { }
    }
    public int CurrentCalture()
    {
      try
      {
        return (int)HttpContext.Current.Session["calture"];
      }
      catch
      {

        return this.DefaultCalture;
      }
    }
    public string Text(Dictionary<int, string> input)
    {
      try
      {
        return input[this.CurrentCalture()].Text();
      }
      catch { return ""; }
    }
    public string Raw(Dictionary<int, string> input)
    {
      try
      {
        return input[this.CurrentCalture()];
      }
      catch { return ""; }
    }
  }
}
