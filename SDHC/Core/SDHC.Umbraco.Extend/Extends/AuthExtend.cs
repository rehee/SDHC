using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace SpxUmbracoMember.Umbraco.Extend.Extends
{
  public static class ApiAuth
  {
    public static string EncryptKey { get; set; } = "1234";
    public static string EncryptSecondKey { get; set; } = "4321";
    public static string Encrypt(this string str, string encryptKey)
    {
      DESCryptoServiceProvider descsp = new DESCryptoServiceProvider();
      byte[] key = Encoding.Unicode.GetBytes(encryptKey);
      byte[] data = Encoding.Unicode.GetBytes(str);
      MemoryStream MStream = new MemoryStream();
      CryptoStream CStream = new CryptoStream(MStream, descsp.CreateEncryptor(key, key), CryptoStreamMode.Write);
      CStream.Write(data, 0, data.Length);
      CStream.FlushFinalBlock();
      return Convert.ToBase64String(MStream.ToArray());
    }
    public static string Decrypt(this string str, string encryptKey)
    {
      DESCryptoServiceProvider descsp = new DESCryptoServiceProvider();
      byte[] key = Encoding.Unicode.GetBytes(encryptKey);
      byte[] data = Convert.FromBase64String(str);
      MemoryStream MStream = new MemoryStream();
      CryptoStream CStram = new CryptoStream(MStream, descsp.CreateDecryptor(key, key), CryptoStreamMode.Write);
      CStram.Write(data, 0, data.Length);
      CStram.FlushFinalBlock();
      return Encoding.Unicode.GetString(MStream.ToArray());
    }
    public static string GetEncryptToken(this Guid userKey)
    {
      var json = new ApiAuthToken(userKey, DateTime.UtcNow).ConvertToJson();
      var str1 = json.Encrypt(EncryptKey);
      var str2 = str1.Encrypt(EncryptSecondKey);
      return str2;
    }
    public static ApiAuthToken DecryptToken(this string str)
    {
      try
      {
        var str1 = str.Decrypt(EncryptSecondKey);
        var json = str1.Decrypt(EncryptKey);
        return json.ConvertJsonToObject<ApiAuthToken>();
      }
      catch
      {
        return null;
      }

    }
    public static void Register(HttpConfiguration config)
    {
      config.Routes.MapHttpRoute(
          name: "ApiAuth",
          routeTemplate: "auth",
          defaults: new { controller = "auth", }
      );
    }
    public static bool IsTokenAvaliable(this ApiAuthToken token, int tokenExpireMins)
    {
      return token != null && token.UserKey != null && token.TimeStamp != null && (DateTime.UtcNow - (DateTime)token.TimeStamp).TotalMinutes < tokenExpireMins;
    }
  }

  public class SpxusAuthorizeLogin
  {
    public string Name { get; set; }
    public string Password { get; set; }
  }
  public class ApiAuthToken
  {
    public Guid? UserKey { get; set; } = null;
    public DateTime? TimeStamp { get; set; } = null;
    public ApiAuthToken(Guid? userKey, DateTime? timeStamp)
    {
      this.UserKey = userKey;
      this.TimeStamp = timeStamp;
    }
  }
}
