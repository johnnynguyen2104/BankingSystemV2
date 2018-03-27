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
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
                                        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public override void OnException(System.Web.Http.Filters.HttpActionExecutedContext context)
        {
            var exception = context.Exception as ApiException;
            if (exception != null)
            {
                log.Error(exception.Message);

                context.Response
                    = context.Request.CreateResponse(
                    exception.StatusCode, exception.Message.ToRequestErrorModel());

            }
        }
    }
}