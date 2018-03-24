using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Common.CustomExceptions
{
    public class ApiException : Exception
    {
        public ApiException(HttpStatusCode statusCode, string errorCode, string errorDescription)
       : base($"{errorCode}::{errorDescription}")
        {
            StatusCode = statusCode;
        }

        public ApiException(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; set; }

    }
}
