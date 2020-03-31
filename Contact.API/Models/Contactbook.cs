using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contact.API.Models
{
    /// <summary>
    /// 通讯录
    /// </summary>
    public class Contactbook
    {
        public Contactbook()
        {
            Contacts = new List<Contact>();
        }

        /// <summary>
        /// 用户id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 好友列表
        /// </summary>
        public List<Contact> Contacts { get; set; }
    }
}
