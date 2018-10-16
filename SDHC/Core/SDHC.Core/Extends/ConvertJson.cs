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
    public static object ConvertJsonToObject(this string input, Type type)
    {
      if (String.IsNullOrEmpty(input))
        return null;
      try
      {
        return JsonConvert.DeserializeObject(input.Text(), type);
      }
      catch { }
      return null;
    }
  }
}
