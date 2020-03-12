﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace User.Identity.Services
{
    public interface IAuthCodeService
    {

        /// <summary>
        /// 根据手机号码验证验证码
        /// </summary>
        /// <param name="phone">手机号码</param>
        /// <param name="authCode">验证码</param>
        /// <returns></returns>
        bool ValiDate(string phone, string authCode);

    }
}
