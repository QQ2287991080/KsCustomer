using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OAuth;
using Owin;

namespace Jusoft.YiFang.Api
{
    public partial class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public static string PublicClientId { get; private set; }

        // 有关配置身份验证的详细信息，请访问 https://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            //跨域问题
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            // 配置数据库上下文、用户管理器和登录管理器，以便为每个请求使用单个实例
            //app.CreatePerOwinContext(ApplicationDbContext.Create);
            //app.CreatePerOwinContext<JuUserManager>(JuUserManager.Create);
            //app.CreatePerOwinContext<JuSignInManager>(JuSignInManager.Create);

            app.UseOAuthBearerTokens(new OAuthAuthorizationServerOptions()
            {
                TokenEndpointPath = new PathString("/token"),
                Provider = new YiFang.Api.Providers.ApplicationOAuthProvider(),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
                AllowInsecureHttp = true
            });

        }
    }
}
