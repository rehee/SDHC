using SpxUmbracoMember.UKPostCode.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
  public static class SDHCUKPostExtend
  {
    public static string GetLat(this PostCodeModel input)
    {
      if (input == null)
        return "0";
      try
      {
        return input.result.latitude.Text("0");
      }
      catch
      {
        return "0";
      }
    }
    public static string GetLng(this PostCodeModel input)
    {
      if (input == null)
        return "0";
      try
      {
        return input.result.longitude.Text("0");
      }
      catch
      {
        return "0";
      }
    }
  }
}
