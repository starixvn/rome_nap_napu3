<%@ WebHandler Language="C#" Class="ProcessLogin" %>

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Sunplus.Accounting.Business.Security;
using Sunplus.Accounting.Core.Common;
using Sunplus.Accounting.Core.Configurations;
using IBCUtils;
using Fanex.Tarus.User.Entities;
using Fanex.Tarus.User.Business.Services;

public class ProcessLogin : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        try
        {
            context.Response.ContentType = "text/html";
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            string username = context.Request.Form["TxtUsername"];
            string password = context.Request.Form["TxtPassword"];

            context.Response.Write("<html><head><script type='text/javascript'>\n");
            int custId;
            string system;
            int roleId;

            UserGroup userGroup = new UserInfoService().GetAdminGroup(username);
            string[] allowedGroupsAccess = WebSettings.AllowedGroupsAccess;
            string[] allowedUsersAccess = WebSettings.AllowedUsersAccess;
            bool allowedAccess = false;
            bool isU3SpecialAccount = false;

            for (int i = 0; i < allowedUsersAccess.Length; i++)
            {
                if (username == allowedUsersAccess[i].Trim())
                {
                    allowedAccess = true;
                    isU3SpecialAccount = true;
                    break;
                }
            }

            if (!allowedAccess)
            {
                for (int i = 0; i < allowedGroupsAccess.Length; i++)
                {
                    if (userGroup.GroupId.ToString() == allowedGroupsAccess[i].Trim())
                    {
                        allowedAccess = true;
                        break;
                    }
                }
            }

            if (!allowedAccess)
            {
                context.Session.Clear();
                FormsAuthentication.SignOut();
                context.Response.Write("parent.signIn.loginError('" + Resources.ErrorMsg.DenyLoginAccount + "');\n");
                goto end;
            }

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                context.Response.Write("parent.signIn.loginError('Username and Password must not be empty!');\n");

            if (LoginFailedHandler.IsLocked(username))
            {
                context.Response.Write(LoginFailedHandler.WriteJS(username));
                goto end;
            }

            AppSession.CurrentSystemName = "accounting";

            string browserName = context.Request.Browser.Browser + " " + context.Request.Browser.Version;
            IbcError error = Sunplus.Accounting.DataAccess.IBCDLL.UserController.AccLoginAdmin(ref username, password, context.Request.UserHostAddress, context.Request.UrlReferrer.AbsoluteUri, out custId, out roleId, out system, context.Request.Url.Host, browserName, context.Request.Browser.Platform, "");

            if (error.IsSuccess())
            {
                LoginFailedHandler.LoginSuccess(username);

                FormsAuthentication.SetAuthCookie(username, false);
                AppSession.Username = username;
                AppSession.AccUserName = username;
                AppSession.AccId = custId;
                AppSession.CurrentSystemName = system;
                AppSession.IsU3SpecialAccount = isU3SpecialAccount;
                AppSession.GroupID = userGroup.GroupId;
                AppSession.IsU3Account = userGroup.GroupId == WebSettings.U3GroupId ? true : false;
                AppSession.IsITAccount = userGroup.GroupId == WebSettings.ITGroupId ? true : false;
                
                if (string.IsNullOrEmpty(system))
                {
                    context.Session.Clear();
                    FormsAuthentication.SignOut();
                    context.Response.Write("parent.signIn.loginError('" + Resources.ErrorMsg.DenyLoginAccount + "');\n");
                    goto end;
                }

                #region checking - denied systems
                var deniedGroups = System.Configuration.ConfigurationManager.AppSettings["DeniedAccessGroup"].ToLower().Split(';');

                if (deniedGroups != null && deniedGroups.Length > 0)
                {
                    if (((IList<string>)deniedGroups).Contains(system.ToLower()))
                    {
                        context.Session.Clear();
                        FormsAuthentication.SignOut();
                        context.Response.Write("parent.signIn.loginError('" + Resources.ErrorMsg.DenyLoginAccount + "');\n");
                        goto end;
                    }
                }
                #endregion checking - denied systems

                AppSession.CurrentSystemNAME = Enumerations.ToAccountingSystem(system);
                AppSession.OnAccId = Sunplus.Accounting.DataAccess.IBCDLL.UserController.AccLoginAdminOnuser(custId, WebSettings.ServerId, context.Request.UserHostAddress);
                AppSession.RoleID = roleId;
                AppSession.AccPwd = IBCUtils.Encryption.CfsEnCode(password);

                if (!AuthManager.IsValidRequestUrl(context.Request.Url.Host, AppSession.CurrentSystemName))
                {
                    context.Session.Clear();
                    FormsAuthentication.SignOut();
                    context.Response.Write("parent.signIn.loginError('Invalid Login');\n");
                    goto end;
                }

                AuthManager.AssignRoles(custId);
                if (AppSession.RoleAccess.Count < 1) //limit user have no permission access 
                {
                    context.Session.Clear();
                    FormsAuthentication.SignOut();
                    context.Response.Write("parent.signIn.loginError('" + Resources.ErrorMsg.DenyLoginAccount + "');\n");
                    goto end;
                }

                context.Response.Write("top.location.href='" + FormsAuthentication.DefaultUrl.Replace("https", "http") + "';\n");
                //IMenuManager menuService = CommonBootStrapper.Locator.GetInstance<IMenuManager>();
                //menuService.GetUserMenu();
                AppSession.UserMenu = new List<Sunplus.Accounting.Entities.MenuBO>();

                if (Permissions.HasPermission("UserControl"))
                {
                    AppSession.UserMenu.Add(new Sunplus.Accounting.Entities.MenuBO
                    {
                        Id = 1,
                        Name = "ACCOUNT MANAGEMENT",
                        Url = "UserInfo/Index",
                        System = "accounting",
                        ParentId = 0,
                        FunctionName = "[UserControl]",
                        Target = "",
                        IsHide = false,
                        SubMenu = null
                    });
                } 
                 Uri uri = context.Request.Url;
                
                 AppSession.UserMenu.Add(new Sunplus.Accounting.Entities.MenuBO
                    {
                        Id = 0,
                        Name = "NAP",
                        Url = string.Format(System.Configuration.ConfigurationManager.AppSettings["NapHomeSSOUrl"], uri.Scheme + Uri.SchemeDelimiter + context.Request.Headers["Host"].ToString()),
                        System = "accounting",
                        ParentId = 0,
                        FunctionName = "",
                        Target = "_blank",
                        IsHide = false,
                        SubMenu = null
                    });             

                // Progressive Bonus
                Fanex.CustomerSetting.Module.Core.Web.AppSession.AdminId = custId;
                Fanex.CustomerSetting.Module.Core.Web.AppSession.AdminUsername = username;
            }
            else
            {
                if (LoginFailedHandler.IsLocked(username))
                {
                    context.Response.Write(LoginFailedHandler.WriteJS(username));
                }
                else
                {
                    LoginFailedHandler.LoginFailed(username, error.ErrorCode);
                    //-----------------------------------------------------------------------

                    if (LoginFailedHandler.IsLocked(username))
                    {
                        context.Response.Write(LoginFailedHandler.WriteJS(username));
                    }
                    else
                    {
                        context.Response.Write("parent.signIn.loginError('" + IBCUtils.Encryption.RemoveSpecialChars(error.ErrorMsg) + "');\n");
                    }
                }
            }
        end:
            context.Response.Write("\n</script></head><body></body>");
        }
        catch (Exception ex)
        {
            Sunplus.Accounting.Core.Extensions.LogExtensions.ExceptionHandler(ex);
        }
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}
