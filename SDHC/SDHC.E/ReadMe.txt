﻿注意: 在 umbraco 初始安装后. 需要将
<add key="umbracoPath" value="~/umbraco" /> 改为
<add key="umbracoPath" value="~/admin" />
1. 将Global.asax改为 <%@ Application Inherits="Umbraco.Web.CustomUmbracoApplication" Language="C#" %>
2. 如果部署在godaddy上 需要改full trust. 只能用这种写法 在web.config 中 <trust level="Full" />
3. 在web.config 中 UmbracoMembershipProvider 修改 allowManuallyChangingPassword="true" 
4. 定制逻辑放入area中
5. 可能需要下列的nuget packag
Microsoft.CodeAnalysis
Microsoft.CodeAnalysis.Analyzers




6. 此示例中需在web.config 的 appsetting 中  加入key
homeID
<add key="homeID" value="1063"/>
<add key="caltureId" value="0"/>

<add key="umbracoVersionCheckPeriod" value="0"/>

在UmbracoMembershipProvider和UsersMembershipProvider 中
加入 maxInvalidPasswordAttempts="2147483647" 

apiController需要在area的RegisterArea中加入
context.MapHttpRoute(
                name: "//name",
                routeTemplate: "//api route Template"
            );

验证在Filters中

