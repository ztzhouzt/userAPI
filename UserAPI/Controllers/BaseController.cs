using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserAPI.Dtos;

namespace UserAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : Controller
    {

        protected UserIdentity UserIdentity => new UserIdentity { UserId = 1, Name = "zhoutao" };

    }
}