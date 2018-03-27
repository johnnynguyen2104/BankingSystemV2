using BankingSystem.Business.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Tests.Businesses
{
    public partial class AccountBusinessTest
    {
        static object[] NegativeAccountNumber =
        {
            new object[] { new BaseRequest() { AccountNumber = 0 } },
            new object[] { new BaseRequest() { AccountNumber = -1} },
            new object[] { new BaseRequest() { AccountNumber = -2} }
        };

        static object[] NegativeAmount =
       {
            new object[] { new BaseRequest() { AccountNumber = 1, Amount = 0 } },
            new object[] { new BaseRequest() { AccountNumber = 2, Amount = -1 } },
            new object[] { new BaseRequest() { AccountNumber = 3, Amount = -2 } }
        };

        static object[] CorrectDepositBaseRequest =
       {
            new object[] { new BaseRequest() { AccountNumber = 1, Amount = 100, Currency= "THB" }, 103.53M },
            new object[] { new BaseRequest() { AccountNumber = 2, Amount = 100, Currency = "USD"}, 3221M }
        };

        static object[] CorrectWithdrawBaseRequest =
        {
            new object[] { new BaseRequest() { AccountNumber = 1, Amount = 100, Currency= "THB" }, 97.13M },
            new object[] { new BaseRequest() { AccountNumber = 2, Amount = 100, Currency = "USD"}, 6879M }
        };

        static object[] CorrectWithdrawBaseRequest_NotEnoughBalance =
       {
            new object[] { new BaseRequest() { AccountNumber = 1, Amount = 100, Currency= "THB" }},
            new object[] { new BaseRequest() { AccountNumber = 2, Amount = 100, Currency = "USD"} }
        };
    }
}
