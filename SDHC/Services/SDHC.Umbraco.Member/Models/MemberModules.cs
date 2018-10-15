using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpxUmbracoMember.Umbraco.Member.Models
{
  public class SpxMember
  {
    public string UserName { get; set; }
    public dynamic MemberModel { get; set; }
  }
  public class SpxLogin
  {
    public string LoginKey { get; set; }
    public string LoginPassword { get; set; }
    public Dictionary<string, dynamic> OtherProperty { get; set; }
  }
  public class SpxRegister
  {
    public string UserName { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public string Email { get; set; }
    public Dictionary<string, dynamic> OtherProperty { get; set; }
    public bool Active { get; set; } = true;
  }
  public class SpxChangePassword
  {
    public string UserIdentify { get; set; }
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
    public bool ForceChange { get; set; } = false;
    public string Token { get; set; } = "";
  }
  public enum LoginResponseStatus
  {
    LoginKeyEmpty = 1,
    PasswordEmpty = 2,
    NamePasswordNotMatch = 3,
    PasswordNotStrong = 4
  }
  public enum RegisterResponseStatus
  {
    UserNameIsEmpty = 1,
    UserNameExist = 2,
    EmailIsEmpty = 3,
    EmailExist = 4,
    PasswordIsEmpty = 5,
    ConfirmPasswordNotSame = 6,
    PasswordNotStrong = 7,
    SystemError = 8
  }
}
