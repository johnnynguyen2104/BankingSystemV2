using BankingSystem.Common.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.ModelBinding;

namespace BankingSystem.Common.Extenstions
{
    public static class ModelErrorExtensions
    {
        public static ResponseExceptionModel ToModel(this ModelError error, string field)
        {
            var model = new ResponseExceptionModel();
            var codedMessage =
                error.ErrorMessage.Split(new[] { "::" }, StringSplitOptions.RemoveEmptyEntries);

            if (codedMessage.Length == 2)
            {
                model.Message = codedMessage[1];
            }
            else
            {
                model.Message = error.ErrorMessage;
            }

            return model;
        }

        public static IEnumerable<ResponseExceptionModel> ConvertToResponseModel(this ModelStateDictionary modelState)
        {
            var errorList = new List<ResponseExceptionModel>();

            foreach (var state in modelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    var converted = error.ToModel(state.Key);
                    errorList.Add(converted);
                }
            }

            return errorList;
        }

        public static List<ResponseExceptionModel> ToRequestErrorModel(this string message)
        {
            var model = new ResponseExceptionModel();
            var codedMessage = message.Split(new[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
            if (codedMessage.Length == 2)
            {
                model.Message = string.Join(",", codedMessage[0], codedMessage[1]);
            }
            else
            {
                model.Message = message;
            }

            return new List<ResponseExceptionModel> { model };
        }
    }
}
