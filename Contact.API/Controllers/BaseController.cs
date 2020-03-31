using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contact.API.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Contact.API.Controllers
{
   
    public class BaseController : Controller
    {

        protected UserIdentity UserIdentity => new UserIdentity { UserId = 1};

        //protected UserIdentity UserIdentity
        //{
        //    get
        //    {
        //        var userIdentity = new UserIdentity();
        //        userIdentity.UserId = Convert.ToInt16(User.Claims.FirstOrDefault(b => b.Type == "sub").Value);
        //        userIdentity.Title = User.Claims.FirstOrDefault(b => b.Type == "title").Value;
        //        userIdentity.Company = User.Claims.FirstOrDefault(b => b.Type == "company").Value;
        //        userIdentity.Avatar = User.Claims.FirstOrDefault(b => b.Type == "avatar").Value;
        //        userIdentity.Name = User.Claims.FirstOrDefault(b => b.Type == "name").Value;
        //        return userIdentity;
        //    }
        //}
    }
}