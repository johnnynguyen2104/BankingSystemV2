using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Common
{
    public class AppMessages
    {
        public static readonly string UnsupportedCurrency = "Unsupport currency {0}";

        public static readonly string NullArgument = "Null Argument.";

        public static readonly string DepositMessageDetails = "You just deposited money to Account Number= {0}, you balance before: {1}, balance after: {2}, currency: {3}";

        public static readonly string WithdrawMessageDetails = "You just withdrawed money to Account Number= {0}, you balance before: {1}, balance after: {2}, currency: {3}";

        public static readonly string NotEnoughBalance = "Account Number {0} doesn't have enough balance to withdraw.";

        public static readonly string AccountDoesntExistOrInactive = "The account number {0} doesn't exist or inactive.";

        public static readonly string AccountNumberNegative = "Account Number can't be negative number or Zero.";

        public static readonly string AccountCurrencyNotSupport = "Not supported currency type for account number {0}, currency = {1}";

        public static readonly string BalanceNotEnough = "Balance not enough to withdraw.";

        public static readonly string InquiryAction = "Account Number: {0}, Balance: {1}";

        public static readonly string NegativeAmount = "Amount cant be negative number.";

        public static readonly string ExchangedNegativeAmount = "Negative amount after exchange accountNumber= {0}, AccountCurrency= {1}, amount after exchange: {2}";
    }
}
