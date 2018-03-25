using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;

namespace BankingSystem.Controllers
{
    public class BaseController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
                                        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET: Base
        protected override ExceptionResult InternalServerError(Exception exception)
        {
            log.Error(exception.Message);
            log.Error(exception.InnerException);
            return base.InternalServerError(exception);
        }

        //public ActionResult ErrorPage()
        //{
        //    return
        //}
    }
}