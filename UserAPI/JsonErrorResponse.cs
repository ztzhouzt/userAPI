using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace UserAPI
{
    public class JsonErrorResponse
    {
        public string Message { get; set; }

        public object DeveloperMessage { get; set; }
    }
}
