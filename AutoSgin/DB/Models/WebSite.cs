using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoSgin.DB.Domain;

namespace AutoSgin.DB.Models
{
    public class WebSite : Entity
    {
        public string Name { get; set; }
        public string Domain { get; set; }
        public string UserName { get; set; }
        public string UserNameFiled { get; set; }
        public string PassWord { get; set; }
        public string PassWordFiled { get; set; }
        public string SginUrl { get; set; }
        public string SginSuccessResult { get; set; }
        public string SginFailResult { get; set; }
        public string LoginUrl { get; set; }
        public string LoginParam { get; set; }
        public string LoginContentType { get; set; }
        public string LoginType { get; set; }
        public string LoginSuccessResult { get; set; }
        public string LoginFailResult { get; set; }
        public string Cookie { get; set; }
    }
}
