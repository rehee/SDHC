using SpxUmbracoMember.Umbraco.Member.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpxUmbracoMember.Umbraco.Member
{
  public delegate void Login(SpxLogin model, out MethodResponse response);
  public delegate void Register(SpxRegister model, out MethodResponse response);
  public delegate bool PasswordStrengthCheck(string input);
  public delegate SpxMember GetMember(dynamic searchFunction);
  public delegate IEnumerable<SpxMember> GetMembers(dynamic searchFunction);
  public delegate void ChangePassword(SpxChangePassword model, out MethodResponse response);
  public delegate string ForgotPassword(string email, out MethodResponse response);
  public delegate void ActiveUser(string token, out MethodResponse response);

  public interface IUmbracoMember
  {
    Login LoginFunction { get; set; }
    Register RegisterFunction { get; set; }
    GetMember GetMemberFunctioin { get; set; }
    GetMembers GetMembersFunction { get; set; }
    PasswordStrengthCheck PasswordStrengthCheckFunction { get; set; }
    ChangePassword ChangePasswordFunction { get; set; }
    ForgotPassword ForgotPasswordFunction { get; set; }
    ActiveUser ActiveUserFunction { get; set; }
    void Logout(out MethodResponse response);
  }
}
