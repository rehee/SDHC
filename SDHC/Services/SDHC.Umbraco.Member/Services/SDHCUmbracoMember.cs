using SpxUmbracoMember.Umbraco.Extend.Extends;
using SpxUmbracoMember.Umbraco.Member.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Security;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace SpxUmbracoMember.Umbraco.Member.Services
{
  public class SDHCUmbracoMember : IUmbracoMember
  {
    private IMemberService service;
    private string passwordRaxString = "[0-9A-Za-z]";
    private int nonAlphanumeric { get; set; } = 0;
    private int passwordLength { get; set; } = 1;
    private int tokenExpireMins { get; set; } = 60 * 24;
    private PasswordStrengthCheck passwordCheckDefault
    {
      get
      {
        return (string input) =>
        {
          input = input.Text();
          if (input.Length < this.passwordLength)
            return false;
          var r = new Regex(this.passwordRaxString);
          var pass = r.Replace(input, "");
          if (pass.Length < this.nonAlphanumeric)
            return false;
          return true;
        };
      }
    }
    private ChangePassword ChangePasswordDefault()
    {
      return (SpxChangePassword model, out MethodResponse response) =>
      {
        response = new MethodResponse();
        try
        {
          IMember user = null;
          var token = model.Token.DecryptToken();
          if (token.IsTokenAvaliable(tokenExpireMins))
          {
            user = service.GetByKey((Guid)token.UserKey);
            model.ForceChange = true;
          }
          if (user == null)
            user = this.service.GetByUsername(model.UserIdentify);
          if (user == null)
            user = this.service.GetByEmail(model.UserIdentify);
          if (user == null)
            user = this.service.GetById(model.UserIdentify.Int32());
          if (user == null)
            user = this.service.GetByKey(new Guid(model.UserIdentify));

          if (user != null && (Membership.ValidateUser(user.Name, model.OldPassword) && model.ForceChange))
          {
            goto ChangePassword;
          }
          return;
          ChangePassword:
          if (!this.PasswordStrengthCheckFunction(model.NewPassword))
          {
            response.Error = "Password not strong";
          }
          this.service.SavePassword(user, model.NewPassword);
          response.IsOk = true;
          return;
        }
        catch (Exception ex)
        {
          response.ResponseObject = ex;
        }
      };
    }
    private string GetUserTokenByEmail(string email)
    {
      var user = service.GetByEmail(email);
      if (user == null)
        return "";
      return user.Key.GetEncryptToken();
    }
    private ForgotPassword ForgotPasswordDefault
    {
      get
      {
        return (string email, out MethodResponse response) =>
        {
          response = new MethodResponse();
          try
          {
            var str = GetUserTokenByEmail(email);
            response.IsOk = true;
            return str;
          }
          catch (Exception e)
          {
            response.Error = e.Message;
            return "";
          }
        };
      }
    }
    private ActiveUser ActiveUserDefault
    {
      get
      {
        return (string tokenString, out MethodResponse response) =>
        {
          response = new MethodResponse();
          try
          {
            var token = tokenString.DecryptToken();
            if (!token.IsTokenAvaliable(tokenExpireMins))
              return;
            var user = service.GetByKey((Guid)token.UserKey);
            if (user == null)
              return;
            user.IsApproved = true;
            service.Save(user);
            response.IsOk = true;
          }
          catch (Exception ex)
          {
            response.Message = ex.Message;
          }
        };
      }
    }

    public SDHCUmbracoMember(string memberType, List<string> roles, IMemberService service, int nonalphanumeric = 0,
        int passwordLength = 10, PasswordStrengthCheck passwordCheck = null, int tokenExpireMins = 60 * 24)
    {
      this.service = service;
      if (passwordCheck == null)
      {
        PasswordStrengthCheckFunction = passwordCheckDefault;
      }
      else
      {
        this.PasswordStrengthCheckFunction = passwordCheck;
      }
      this.passwordLength = passwordLength;
      this.nonAlphanumeric = nonalphanumeric;
      this.tokenExpireMins = tokenExpireMins;

      this.LoginFunction = this.CreateLoginFunction(this.service, this.PasswordStrengthCheckFunction);
      this.RegisterFunction = this.CreateRegisterFunction(memberType, roles, this.service, this.PasswordStrengthCheckFunction);
      this.GetMemberFunctioin = (s) => this.GetSpxMemberByUserName(G.Text(s));
      this.ChangePasswordFunction = this.ChangePasswordDefault();
      this.ForgotPasswordFunction = this.ForgotPasswordDefault;
      this.ActiveUserFunction = this.ActiveUserDefault;
    }
    public PasswordStrengthCheck PasswordStrengthCheckFunction { get; set; }
    public Login LoginFunction { get; set; }
    public Register RegisterFunction { get; set; }
    public GetMember GetMemberFunctioin { get; set; }
    public GetMembers GetMembersFunction { get; set; }
    public ChangePassword ChangePasswordFunction { get; set; }
    public ForgotPassword ForgotPasswordFunction { get; set; }
    public ActiveUser ActiveUserFunction { get; set; }
    public void Logout(out MethodResponse response)
    {
      response = new MethodResponse();
      try
      {
        FormsAuthentication.SignOut();
        response.IsOk = true;
      }
      catch (Exception ex)
      {
        response.ResponseObject = ex;
      }
    }
    public SpxMember GetSpxMemberByUserName(string name)
    {
      var member = this.service.GetByUsername(name);
      return null;
    }
    public void SpxusLogin(IMemberService service, PasswordStrengthCheck passwordStrengthCheck, SpxLogin model, out MethodResponse response)
    {
      response = new MethodResponse();
      if (model == null)
      {
        return;
      }
      if (model.LoginKey.Text() == "")
      {
        response.ResponseCode = (int)LoginResponseStatus.LoginKeyEmpty;
        return;
      }
      if (model.LoginPassword.Text() == "")
      {
        response.ResponseCode = (int)LoginResponseStatus.PasswordEmpty;
        return;
      }
      if (Membership.ValidateUser(model.LoginKey, model.LoginPassword))
      {
        response.IsOk = true;
        FormsAuthentication.SetAuthCookie(model.LoginKey, true);
        return;
      }
      var user = service.GetByEmail(model.LoginKey);
      if (user != null && Membership.ValidateUser(user.Username, model.LoginPassword))
      {
        response.IsOk = true;
        FormsAuthentication.SetAuthCookie(user.Username, true);
        return;
      }
      response.ResponseCode = (int)LoginResponseStatus.NamePasswordNotMatch;
    }
    public Login CreateLoginFunction(IMemberService service = null, PasswordStrengthCheck passwordStrengthCheck = null)
    {
      return (SpxLogin model, out MethodResponse response) =>
      {
        this.SpxusLogin(service, passwordStrengthCheck, model, out response);
      };
    }
    public void SpxusRegister(string memberType, List<string> roles, IMemberService service, PasswordStrengthCheck passwordStrengthCheck, SpxRegister model, out MethodResponse response)
    {
      response = new MethodResponse();
      if (model.UserName.Text() == "")
      {
        response.ResponseCode = (int)RegisterResponseStatus.UserNameIsEmpty;
        return;
      }
      if (model.Email.Text() == "")
      {
        response.ResponseCode = (int)RegisterResponseStatus.EmailIsEmpty;
        return;
      }
      if (model.Password.Text() == "")
      {
        response.ResponseCode = (int)RegisterResponseStatus.PasswordIsEmpty;
        return;
      }
      if (model.Password.Text() != model.ConfirmPassword.Text())
      {
        response.ResponseCode = (int)RegisterResponseStatus.ConfirmPasswordNotSame;
        return;
      }
      var member = service.GetByUsername(model.UserName.Text());
      if (member != null)
      {
        response.ResponseCode = (int)RegisterResponseStatus.UserNameExist;
        return;
      }
      member = service.GetByEmail(model.Email.Text());
      if (member != null)
      {
        response.ResponseCode = (int)RegisterResponseStatus.EmailExist;
        return;
      }

      if (!passwordStrengthCheck(model.Password))
      {
        response.ResponseCode = (int)RegisterResponseStatus.PasswordNotStrong;
        return;
      }
      try
      {
        member = service.CreateMember(model.UserName, model.Email, model.UserName, memberType);
        member.IsApproved = model.Active;
        service.Save(member);
        service.SavePassword(member, model.Password);
        roles.ForEach(
            b =>
            {
              try
              {
                service.AssignRole(member.Id, b);
              }
              catch { }
            });
        response.IsOk = true;
        if (model.OtherProperty == null)
        {
          return;
        }
        foreach (var item in model.OtherProperty)
        {
          try
          {
            member.SetValue(item.Key, item.Value);
          }
          catch { }
        }
        try
        {
          service.Save(member);
        }
        catch { }
      }
      catch (Exception ex)
      {
        response.ResponseCode = (int)RegisterResponseStatus.SystemError;
        response.Message = ex.Message;
        return;
      }
    }
    public Register CreateRegisterFunction(string memberType, List<string> roles, IMemberService service = null, PasswordStrengthCheck passwordStrengthCheck = null)
    {
      return (SpxRegister model, out MethodResponse response) =>
      {
        this.SpxusRegister(memberType, roles, service, passwordStrengthCheck, model, out response);
      };
    }
  }

}
