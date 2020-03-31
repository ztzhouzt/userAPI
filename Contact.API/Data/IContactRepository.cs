﻿using Contact.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Contact.API.Data
{
    public interface IContactRepository
    {
        /// <summary>
        /// 更新联系人信息
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>

        Task<bool> UpdateContactInfoAsync(BaseUserInfo userInfo, CancellationToken cancellationToken);

        /// <summary>
        /// 添加联系人信息
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> AddContactAsync(int userId, BaseUserInfo contact, CancellationToken cancellationToken);

        /// <summary>
        /// 获取联系人列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>

        Task<List<Models.Contact>> GetContactsAsync(int userId, CancellationToken cancellationToken);

        /// <summary>
        /// 更新好友标签
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        Task<bool> TagContactAsync(int userId, int contactId,List<string> tags, CancellationToken cancellationToken);
    }
}
