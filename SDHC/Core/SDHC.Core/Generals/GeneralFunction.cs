using PCLAppConfig;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web;

namespace System
{
  public static partial class G
  {
    //经常需要把dynamic或者其他乱七八糟的转成字符,trim 或者如果是空取默认值或者第几个字母开始大写等等
    #region
    public static string Text(this object input, string defaultValue = "", int captalString = -1, bool trim = true)
    {
      var result = "";
      if (input == null)
      {
        result = defaultValue;
      }
      else
      {
        result = Convert.ToString(input);
      }
      return result.Text(captalString: captalString, trim: trim);
    }
    public static string Text(this string input, string defaultValue = "", int captalString = -1, bool trim = true)
    {
      var result = input;
      if (String.IsNullOrEmpty(input) || String.IsNullOrWhiteSpace(input))
      {
        result = defaultValue;
      }
      if (trim)
        result = result.Trim();
      return result.CaptalString(captalString);
    }
    public static string CaptalString(this string input, int captalString = -1)
    {
      if (input == null)
      {
        input = "";
      }
      if (captalString < 0)
        return input;
      if (captalString == 0)
        return input.ToLower();
      if (captalString >= input.Length)
        return input.ToUpper();
      return $"{input.Substring(0, captalString).ToUpper()}{input.Substring(captalString - 1, input.Length - captalString).ToLower()}";
    }
    #endregion

    public static int Int32(this string input, int defaultValue = 0)
    {
      try
      {
        return Convert.ToInt32(input);
      }
      catch { return defaultValue; }
    }
    public static int Int32(this bool input)
    {
      if (input)
        return 1;
      return 0;
    }
    public static bool ToBool(this object input)
    {
      try { return Convert.ToBoolean(input); }
      catch
      {
        if (input.Text() == "1")
        {
          return true;
        }
        return false;
      }
    }


    //不想用链表又不想用集合字典,又想添加唯一值
    public static void AddToStringList(this string input, List<string> target)
    {
      var result = input.Text();
      if (result != "")
        target.Add(result);
    }
    //不想用三目怎么办呢
    public static string Compare(this string input, string check, string trueResult = "", string errorResult = "")
    {
      if (input.Text() == check.Text())
      {
        return trueResult.Text();
      }
      return errorResult.Text();
    }
    public static string Compare(this bool input, bool check, string trueResult = "", string errorResult = "")
    {
      if (input == check)
        return trueResult.Text();
      return errorResult.Text();
    }
    public static string Compare(this int input, int check, IntCompareType type = IntCompareType.equal, string trueResult = "", string errorResult = "")
    {
      bool checkResult = false;
      int sub = input - check;
      switch (type)
      {
        case IntCompareType.equal:
          if (sub == 0)
            checkResult = true;
          break;
        case IntCompareType.less:
          if (sub < 0)
            checkResult = true;
          break;
        case IntCompareType.more:
          if (sub > 0)
            checkResult = true;
          break;
        default:
          break;
      }
      if (checkResult)
        return trueResult.Text();
      return errorResult.Text();
    }
    //想要变成现金又不想写Format.
    public static string DefaultMoneyFormat { get; set; } = "#,##0.00";
    #region
    public static string MoneyString(this object input, string moneyString = "")
    {
      decimal result = 0;
      try
      {
        result = Convert.ToDecimal(input);
      }
      catch { }
      return result.ToString(moneyString.Text(DefaultMoneyFormat));
    }
    public static string MoneyString(this Int16 input, string moneyString = "")
    {
      return input.ToString(moneyString.Text(DefaultMoneyFormat));
    }
    public static string MoneyString(this Int32 input, string moneyString = "")
    {
      return input.ToString(moneyString.Text(DefaultMoneyFormat));
    }
    public static string MoneyString(this Int64 input, string moneyString = "")
    {
      return input.ToString(moneyString.Text(DefaultMoneyFormat));
    }
    public static string MoneyString(this double input, string moneyString = "")
    {
      return input.ToString(moneyString.Text(DefaultMoneyFormat));
    }
    public static string MoneyString(this decimal input, string moneyString = "")
    {
      return input.ToString(moneyString.Text(DefaultMoneyFormat));
    }
    #endregion
    //左右pad,还想补齐
    public static string CPadLeft(this object input, int padNumber, char step = '\0')
    {
      var result = Convert.ToString(input);
      if (result.Length > padNumber)
      {
        result = result.Substring(result.Length - padNumber, padNumber);
      }
      else
      {
        result = result.PadLeft(padNumber, step);
      }
      return result;
    }
    public static string CPadRight(this object input, int padNumber, char step = '\0')
    {
      var result = Convert.ToString(input);
      if (result.Length > padNumber)
      {
        result = result.Substring(0, padNumber);
      }
      else
      {
        result = result.PadRight(padNumber, step);
      }
      return result;
    }
    //纯蛋疼没卵用
    #region
    public static string GetMonth(this int month)
    {
      switch (month)
      {
        case 1:
          return "January";
        case 2:
          return "February";
        case 3:
          return "March";
        case 4:
          return "April";
        case 5:
          return "May";
        case 6:
          return "June";
        case 7:
          return "July";
        case 8:
          return "August";
        case 9:
          return "September";
        case 10:
          return "October";
        case 11:
          return "November";
        case 12:
          return "December";
        default:
          return "January";
      }
    }
    public static string GetDay(this int day)
    {
      return day.ToString().PadLeft(2, '0');
    }
    
    #endregion

    //蛋疼用的Portable 类. 然后悲剧了...不能直接读web.config的key
    public static void SetSettingStream(Stream config)
    {
      ConfigurationManager.Initialise(config);
    }
    public static string AppSettings(string key)
    {
      try
      {
        return ConfigurationManager.AppSettings[key];
      }
      catch
      {
        return key;
      }
    }

    public static string RenderTemplate(string temp, IDictionary<string, string> model, string separatFormat = "[{0}]")
    {
      foreach (var item in model)
      {
        temp = temp.Replace(String.Format(separatFormat, item.Key), item.Value);
      }
      return temp;
    }

    public static string ToLowerNth(this string input, int index = 1)
    {
      if (index <= 0)
        return input.ToLower();
      var chars = Char.ToLowerInvariant(input[index - 1]) + input.Substring(index);
      return chars;
    }

  }

  public enum IntCompareType
  {
    equal = 0,
    less = 1,
    more = 2
  }
}
