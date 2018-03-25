using BankingSystem.Business.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Business.Interfaces
{
    public interface IAccountBusiness
    {
        ResponseAccountInfo Balance(int accountNumber);

        Task<ResponseAccountInfo> Deposit(BaseRequest req);

        Task<ResponseAccountInfo> Withdraw(BaseRequest req);
    }
}
