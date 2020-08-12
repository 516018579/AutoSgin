using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AutoSgin.DB;
using AutoSgin.DB.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Polly;

namespace AutoSgin.Services
{
    public class SginService : ApplicationService, ISginService
    {
        private readonly SginDbContext _dbContext;

        public SginService(SginDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SginAll()
        {
            //    await Sgin("https://crystal.alicization.org/user/checkin", "email=516018579%40qq.com;expire_in=1573529495;ip=ab19d270d1c19524ca0fa3382c19d410;key=cdc712cb08edd51215778177e465173096d3e598a5224;PHPSESSID=vt7h6f7lvnp0ohvbnu66oi26ur;uid=1209;");
            //    await Sgin("https://conair.me/user/checkin", "__cfduid=d3eeda3f61441232b7e85dac091328b321569296148;email=516018579%40qq.com;expire_in=1572942303;ip=e885c5ca56e372be61b05a23ed9aeca4;key=3822656ffb42477a83c3c46b3275efdb456d1c99cd239;uid=1521;");
            var webs = await _dbContext.WebSite.ToListAsync();
            foreach (var web in webs)
            {
                await Sgin(web);
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task Sgin(int webId)
        {
            var web = await GetWeb(webId);
            await Sgin(web);
        }


        private async Task Sgin(WebSite web)
        {


            async Task UserSgin(UserInfo user)
            {
                var handler = new HttpClientHandler();
                var httpClient = new HttpClient(handler);

                if (user.Authorization != null)
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                        "Bearer",
                        user.Authorization);

                handler.UseCookies = true;
                var cookieContainer = handler.CookieContainer = new CookieContainer();

                if (string.IsNullOrWhiteSpace(user.Cookie))
                {
                    var isLogin = false;
                    if (!(string.IsNullOrWhiteSpace(user.UserName) || string.IsNullOrWhiteSpace(user.PassWord) ||
                          string.IsNullOrWhiteSpace(web.LoginUrl)))
                    {
                        var response = await httpClient.PostAsync(web.Domain + "/" + web.LoginUrl,
                            new StringContent(
                                $"{web.UserNameFiled}={user.UserName}&{web.PassWordFiled}={user.PassWord}&remember_me=week",
                                Encoding.UTF8, "application/x-www-form-urlencoded"));

                        var loginResult = await response.Content.ReadAsStringAsync();

                        Console.WriteLine($"登录返回值:{loginResult}");

                        if (web.LoginFailResult != null && !loginResult.Contains(web.LoginFailResult) ||
                            web.LoginSuccessResult != null && loginResult.Contains(web.LoginSuccessResult))
                        {
                            isLogin = true;
                            var responseCookies = cookieContainer.GetCookies(new Uri(web.Domain));

                            user.Cookie = responseCookies.Select(cookie => cookie.Name + "=" + cookie.Value).Join(";");
                        }
                    }

                    if (!isLogin)
                    {
                        throw new Exception("网站登录失败!");
                    }
                }

                var uri = new Uri(web.Domain + "/" + web.SginUrl);

                user.Cookie.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(x =>
                {
                    var cookieKeyValue = x.Split('=');
                    cookieContainer.Add(new Cookie(cookieKeyValue[0], cookieKeyValue[1]) { Domain = uri.Host });
                });

                var result = await httpClient.PostAsync(uri, new StringContent(""));

                var text = await result.Content.ReadAsStringAsync();

                var isSuccess = text.Contains(web.SginSuccessResult);

                var msg = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {web.Name}: {user.UserName}  结果:";

                Console.WriteLine($"{msg} {(isSuccess ? "OK" : "Fail")}");

                if (!isSuccess)
                {
                    if (user.PassWord != null)
                    {
                        user.Cookie = null;
                    }

                    Console.WriteLine($"失败返回值:{text}");

                    throw new Exception("签到失败!");
                }
            }

            var users = await _dbContext.UserInfo.Where(x => x.WebSiteId == web.Id).ToListAsync();

            foreach (var user in users)
            {
                await Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync(3, (x) => TimeSpan.FromSeconds(1))
                    .ExecuteAndCaptureAsync(() => UserSgin(user));
            }
        }

        public ValueTask<WebSite> GetWeb(int id)
        {
            return _dbContext.WebSite.FindAsync(id);
        }

        public Task<List<WebSite>> GetAllWeb()
        {
            return _dbContext.WebSite.ToListAsync();
        }

        public async Task SetWeb(WebSite web)
        {
            if (web.Id > 0)
            {
                _dbContext.WebSite.Update(web);
            }
            else
                await _dbContext.WebSite.AddAsync(web);

            await _dbContext.SaveChangesAsync();
        }

        public ValueTask<UserInfo> GetUser(int id)
        {
            return _dbContext.UserInfo.FindAsync(id);
        }

        public Task<List<UserInfo>> GetAllUser()
        {
            return _dbContext.UserInfo.ToListAsync();
        }

        public async Task SetUser(UserInfo user)
        {
            if (user.Id > 0)
            {
                _dbContext.UserInfo.Update(user);
            }
            else
                await _dbContext.UserInfo.AddAsync(user);

            await _dbContext.SaveChangesAsync();
        }
    }
}


//{
//"name": "string",
//"domain": "https://conair.me",
//"userName": "516018579@qq.com",
//"userNameFiled": "email",
//"passWord": "xxxx",
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
