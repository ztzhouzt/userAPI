using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contact.API.ViewModels
{
    /// <summary>
    /// 给好友打标签
    /// </summary>
    public class TagContactInputViewModel
    {
        /// <summary>
        /// 好友ID
        /// </summary>
        public int ContactId { get; set; }

        /// <summary>
        /// 标签信息
        /// </summary>
        public List<string> Tags { get; set; }
    }
}
