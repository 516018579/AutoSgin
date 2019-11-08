using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoSgin.DB.Models;
using Microsoft.EntityFrameworkCore.Internal;

namespace AutoSgin.Services
{
    public class SginService : ApplicationService, ISginService
    {
        public async Task SginAll()
        {
            await Sgin("https://crystal.alicization.org/user/checkin", "email=516018579%40qq.com;expire_in=1573529495;ip=ab19d270d1c19524ca0fa3382c19d410;key=cdc712cb08edd51215778177e465173096d3e598a5224;PHPSESSID=vt7h6f7lvnp0ohvbnu66oi26ur;uid=1209;");
            await Sgin("https://conair.me/user/checkin", "__cfduid=d3eeda3f61441232b7e85dac091328b321569296148;email=516018579%40qq.com;expire_in=1572942303;ip=e885c5ca56e372be61b05a23ed9aeca4;key=3822656ffb42477a83c3c46b3275efdb456d1c99cd239;uid=1521;");
        }

        public async Task Sgin(string sginUrl, string cookieValue)
        {
            var handler = new HttpClientHandler();
            var httpClient = new HttpClient(handler);

#if DEBUG
            handler.Proxy = new WebProxy("http://127.0.0.1:1080/");
            handler.UseProxy = true;
#endif

            handler.UseCookies = true;
            var cookieContainer = handler.CookieContainer = new CookieContainer();

            var uri = new Uri(sginUrl);

            cookieValue.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(x =>
            {
                var cookieKeyValue = x.Split('=');
                cookieContainer.Add(new Cookie(cookieKeyValue[0], cookieKeyValue[1]) { Domain = uri.Host });
            });

            var result = await httpClient.PostAsync(uri, new StringContent(""));

            var text = await result.Content.ReadAsStringAsync();

            var isSuccess = text.Contains("MB");

            Console.WriteLine(isSuccess ? "OK" : "Fail");
        }

        public async Task Sgins(WebSite web)
        {
            var handler = new HttpClientHandler();
            var httpClient = new HttpClient(handler);

#if DEBUG
            handler.Proxy = new WebProxy("http://127.0.0.1:1080/");
            handler.UseProxy = true;
#endif

            handler.UseCookies = true;
            var cookieContainer = handler.CookieContainer = new CookieContainer();

            if (string.IsNullOrWhiteSpace(web.Cookie))
            {
                var isLogin = false;
                if (!(string.IsNullOrWhiteSpace(web.UserName) || string.IsNullOrWhiteSpace(web.PassWord) || string.IsNullOrWhiteSpace(web.LoginUrl)))
                {

                    var response = await httpClient.PostAsync(web.Domain + "/" + web.LoginUrl, new StringContent($"{web.UserNameFiled}={web.UserName}&{web.PassWordFiled}={web.PassWord}&remember_me=week", Encoding.UTF8, "application/x-www-form-urlencoded"));

                    var loginResult = await response.Content.ReadAsStringAsync();

                    Console.WriteLine($"登录返回值:{loginResult}");

                    if (web.LoginFailResult != null && !loginResult.Contains(web.LoginFailResult) || web.LoginSuccessResult != null && loginResult.Contains(web.LoginSuccessResult))
                    {
                        isLogin = true;
                        var responseCookies = cookieContainer.GetCookies(new Uri(web.Domain));

                        web.Cookie = responseCookies.Select(cookie => cookie.Name + "=" + cookie.Value).Join(";");
                    }
                }

                if (!isLogin)
                {
                    throw new Exception("网站登录失败!");
                }
            }

            var uri = new Uri(web.Domain + "/" + web.SginUrl);

            web.Cookie.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(x =>
            {
                var cookieKeyValue = x.Split('=');
                cookieContainer.Add(new Cookie(cookieKeyValue[0], cookieKeyValue[1]) { Domain = uri.Host });
            });

            var result = await httpClient.PostAsync(uri, new StringContent(""));

            var text = await result.Content.ReadAsStringAsync();

            var isSuccess = text.Contains(web.SginSuccessResult);

            Console.WriteLine(isSuccess ? "OK" : "Fail");
        }
    }
}


//{
//"name": "string",
//"domain": "https://conair.me",
//"userName": "516018579@qq.com",
//"userNameFiled": "email",
//"passWord": "15107210156",
//"passWordFiled": "passwd",
//"sginUrl": "/user/checkin",
//"sginSuccessResult": "MB",
//"sginFailResult": "string",
//"loginUrl": "/auth/login",
//"loginParam": "string",
//"loginContentType": "string",
//"loginType": "string",
//"loginSuccessResult": "",
//"loginFailResult": "邮箱或者密码错误",
//"cookie": "",
//"id": 0
//}
