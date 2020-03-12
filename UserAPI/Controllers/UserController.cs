using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserAPI.Data;

namespace UserAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {

        private UserContext _userContext;

        public UserController(UserContext context)
        {
            _userContext = context;
        }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var user = _userContext.Users
                .AsNoTracking()
                .Include(u=>u.Type)
                .SingleOrDefault(u => u.Id == UserIdentity.UserId);

            if (user == null)
                return NotFound();
            return  Json(user);
        }

    }
}