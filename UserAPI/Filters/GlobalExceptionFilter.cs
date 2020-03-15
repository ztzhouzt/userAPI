using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace UserAPI.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly IHostingEnvironment _env;
        private readonly ILogger<GlobalExceptionFilter> _logger;

        public GlobalExceptionFilter(IHostingEnvironment env,ILogger<GlobalExceptionFilter> logger)
        {
            _env = env;
            _logger = logger;
        }

        /// <summary>
        /// 异常捕获
        /// </summary>
        /// <param name="context"></param>
        public void OnException(ExceptionContext context)
        {
            var json=new JsonErrorResponse();
            //已定义的异常
            if (context.Exception.GetType() == typeof(UserOperationException))
            {
                json.Message = context.Exception.Message;
                context.Result = new BadRequestObjectResult(json);
            }
            else //未定义的的异常
            {
                json.Message = "发生了未知的错误信息";
                if (_env.IsDevelopment()) //开发环境下，返回具体错误信息
                {
                    json.DeveloperMessage = context.Exception.StackTrace;
                }
                context.Result = new InternalServerErrorObjectResult(json);
            }
            //错误记录到log
            _logger.LogError(context.Exception,context.Exception.Message);
            context.ExceptionHandled = true;
        }

        public class InternalServerErrorObjectResult:ObjectResult
        {
            public InternalServerErrorObjectResult(object error) : base(error)
            {
                StatusCode = StatusCodes.Status500InternalServerError;
            }   
        }
    }
}
