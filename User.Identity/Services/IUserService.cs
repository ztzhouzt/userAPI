﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace User.Identity.Services
{
    public interface IUserService
    {
        /// <summary>
        /// 检查手机号码是否已注册，如果没有注册的话注册一个用户
        /// </summary>
        /// <param name="phone"></param>
        Task<int> CheckOrCreate(string phone); 
    }
}
