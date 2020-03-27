using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserAPI.Data;
using Microsoft.AspNetCore.JsonPatch;
using UserAPI.Models;
using System.Collections;

namespace UserAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        private UserContext _userContext;
        private ILogger<UserController> _logger;

        public UserController(UserContext context,ILogger<UserController> logger)
        {
            _userContext = context;
            _logger = logger;
        }

        /// <summary>
        /// 根据userid得到用户信息
        /// </summary>
        /// <returns></returns>
        [Route("")]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var user = await _userContext.Users
                .AsNoTracking()
                .Include(u => u.Type)
                .SingleOrDefaultAsync(u => u.Id == UserIdentity.UserId);

            if (user == null)
                throw new UserOperationException($"错误的用户上下文,id:{UserIdentity.UserId}");
            
            return Json(user);
        }

        /// <summary>
        /// 更新用户资料
        /// </summary>
        /// <returns></returns>
        [Route("")]
        [HttpPatch]
        public async Task<IActionResult> Patch([FromBody]JsonPatchDocument<AppUser> patch)
        {
            var user = await _userContext.Users  
                //.Include(u=>u.Type) //查用户属性列表
                .SingleOrDefaultAsync(u => u.Id == UserIdentity.UserId);

            patch.ApplyTo(user);


            foreach (var  property in user.Type)
            {
                //设置模型状态不跟踪
                _userContext.Entry(property).State = EntityState.Detached;
            }

            //===================更新用户属性(先删除数据库的属性，再添加)start=========================
            //第一步 查询现有的属性
            var originProperties = await _userContext.UserProperties.AsNoTracking().Where(u => u.AppUserId == UserIdentity.UserId)
                .ToListAsync();
            //第二步 现有属性+新添加的属性合并,并去重
            var allProperties = originProperties.Union(user.Type).Distinct();

            var removedProperties = originProperties.Except(user.Type);  //需要删除的属性
            var newProperties = allProperties.Except(originProperties);  //移除需要删除的属性，得到需要添加的新属性

            foreach (var property in removedProperties)
            {
                _userContext.Remove(property);
            }

            foreach (var property in newProperties)
            {
                _userContext.Add(property);
            }
            //===================更新用户属性(先删除数据库的属性，再添加)end=========================

            _userContext.Users.Update(user);
            _userContext.SaveChanges();
            return Json(user);
        }


        /// <summary>
        /// 检查或者创建用户(当用户手机号不存在的时候则创建用户)
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns> 
       [Route("check-or-create")]
       [HttpPost]
        public async Task<IActionResult> CheckOrCreate(string phone)
        {
            // TBD 做手机号码格式验证

            var user =  _userContext.Users.SingleOrDefault(u => u.Phone == phone);
            if (user == null)
            {
                user = new AppUser { Phone = phone };
                _userContext.Users.Add(user);
                await _userContext.SaveChangesAsync();
            }

            return Ok(user.Id);
        }


        /// <summary>
        /// 获取用户标签选项数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("tags")]
        public async Task<IActionResult> GetUserTags()
        {
            return Ok(await _userContext.UserTags.Where(u => u.AppUserId == UserIdentity.UserId).ToListAsync());
        }

        /// <summary>
        /// 根据手机号码查找用户资料
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("search")]
        public async Task<IActionResult> Search(string phone)
        {
            return Ok(await _userContext.Users.Include(u => u.Type).SingleOrDefaultAsync(u => u.Id == UserIdentity.UserId));
        }


        /// <summary>
        /// 更新用户标签数据
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("tags")]
        public async Task<IActionResult> UpdateUserTags([FromBody]List<string> tags)
        {
            var originTags = await _userContext.UserTags.Where(u => u.AppUserId == UserIdentity.UserId).ToListAsync();
            var newTags = tags.Except(originTags.Select(t => t.Tag));

            await _userContext.UserTags.AddRangeAsync(newTags.Select(t => new UserTag
            {
                CreateTime = DateTime.Now,
                AppUserId = UserIdentity.UserId,
                Tag = t
            }));

            await _userContext.SaveChangesAsync();
            return Ok();
        }

    }
}