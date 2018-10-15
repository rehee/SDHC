using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
  public static class ConvertJson
  {
    public static string ConvertToJson(this Object input)
    {
      try
      {
        return JsonConvert.SerializeObject(input);
      }
      catch
      {
        return "";
      }

    }
    public static T ConvertJsonToObject<T>(this string input)
    {
      try
      {
        return JsonConvert.DeserializeObject<T>(input.Text());
      }
      catch { }
      return default(T);
    }
  }
}
