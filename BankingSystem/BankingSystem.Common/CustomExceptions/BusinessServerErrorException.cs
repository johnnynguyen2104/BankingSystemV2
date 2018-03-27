using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Common.CustomExceptions
{
    public class BusinessServerErrorException : ApiException
    {
        public BusinessServerErrorException()
         : base(HttpStatusCode.InternalServerError)
        {
        }

        public BusinessServerErrorException(string message)
            : base(HttpStatusCode.InternalServerError, HttpStatusCode.InternalServerError.ToString(), message)
        {
        }
    }
}
