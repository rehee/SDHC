using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
  public static class O
  {
    public static Dictionary<string, object> ConvertToDictionary(this Object model)
    {
      Dictionary<string, object> exp = new Dictionary<string, object>();
      try
      {
        foreach (PropertyDescriptor p in TypeDescriptor.GetProperties(model.GetType()))
        {
          exp.Add(p.Name, p.GetValue(model));
        }
      }
      catch { }
      return exp;
    }
    public static Dictionary<string, object> GetDynamicDictionary(this object model)
    {
      return model.ConvertToDictionary();
    }
  }
}
