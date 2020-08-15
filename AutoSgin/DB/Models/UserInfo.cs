using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoSgin.DB.Domain;

namespace AutoSgin.DB.Models
{
    public class UserInfo : Entity
    {
        public int WebSiteId { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public string Cookie { get; set; }
        public string Token { get; set; }
    }
}
