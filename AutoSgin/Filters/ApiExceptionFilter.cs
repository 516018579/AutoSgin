using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AutoSgin.Filters
{
    public class ApiExceptionFilter : IExceptionFilter, IAsyncExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            GetInnerException(context);
        }

        public Task OnExceptionAsync(ExceptionContext context)
        {
            OnException(context);

            return Task.CompletedTask;
        }

        private void GetInnerException(ExceptionContext context)
        {
            #region 获取内部异常
            var exception = context.Exception;
            while (exception.InnerException != null)
                exception = exception.InnerException;
            context.Exception = exception;
            #endregion

            //如不是一下异常,则用UserFriendlyException包装异常, 否则不会显示异常明细
            if (!(context.Exception is AggregateException))
                context.Exception = new Exception(exception.Message, exception);
        }
    }
}
