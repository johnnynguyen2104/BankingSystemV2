using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Common.CustomExceptions
{
    public class BadRequestException : ApiException
    {
        public BadRequestException()
         : base(HttpStatusCode.BadRequest)
        {
        }

        public BadRequestException(string message)
            : base(HttpStatusCode.BadRequest, HttpStatusCode.BadRequest.ToString(), message)
        {
        }
    }
}
