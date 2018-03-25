using BankingSystem.Common.CustomExceptions;
using BankingSystem.Common.Extenstions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;



namespace BankingSystem.Filters
{
    public class ApiExceptionFilterAttribute : System.Web.Http.Filters.ExceptionFilterAttribute
    {
        public override void OnException(System.Web.Http.Filters.HttpActionExecutedContext context)
        {
            var exception = context.Exception as ApiException;
            if (exception != null)
            {
                context.Response
                    = context.Request.CreateResponse(
                    exception.StatusCode, exception.Message.ToRequestErrorModel());

            }
        }
    }
}