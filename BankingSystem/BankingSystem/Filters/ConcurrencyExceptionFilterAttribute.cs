using BankingSystem.Common;
using BankingSystem.Common.CustomExceptions;
using BankingSystem.Common.Extenstions;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace BankingSystem.Filters
{
    public class ConcurrencyExceptionFilterAttribute : System.Web.Http.Filters.ExceptionFilterAttribute
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
                                      (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public override void OnException(System.Web.Http.Filters.HttpActionExecutedContext context)
        {
            var exception = context.Exception as DbUpdateConcurrencyException;
            if (exception != null)
            {
                log.Error(exception.Message);
                context.Response
                    = context.Request.CreateResponse(exception.ToRequestErrorModel(AppMessages.ConcuurrencyError));

            }
        }
    }
}