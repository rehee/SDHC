using SpxUmbracoMember.Umbraco.Member;
using SpxUmbracoMember.Umbraco.Member.Models;
using SpxUmbracoMember.Umbraco.Member.Services;
using System.Collections.Generic;

namespace System
{
  public enum MyMemberType
    {
        Member = 0
    }
    public static partial class E
    {
        public static Dictionary<MyMemberType, string> MyMemberTypeMap { get; set; } = new Dictionary<MyMemberType, string>();
        public static Dictionary<MyMemberType, string> MyMemberTypeNameMapCn { get; set; } = new Dictionary<MyMemberType, string>();
        public static Dictionary<MyMemberType, string> MyMemberTypeRoleMap { get; set; } = new Dictionary<MyMemberType, string>();

        public static IUmbracoMember SpxMember { get; set; }

        public static void InitMember()
        {
            //初始化Spx Umbraco member. 
            var initMemberRoles = new List<string>() { "" };
            //需要与web.config中UmbracoMembershipProvider设置中 非数字字符数量与密码长度设置一致
            var memberPasswordNoNumberCharNumber = 1;
            var memberPasswordLength = 10;
            SpxMember = new SDHCUmbracoMember(
                MyMemberType.Member.ToString(), initMemberRoles, E.Services.MemberService, memberPasswordNoNumberCharNumber, memberPasswordLength);
        }
    }
}