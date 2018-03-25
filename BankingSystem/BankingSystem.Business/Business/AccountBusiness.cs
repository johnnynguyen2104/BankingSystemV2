using BankingSystem.Business.DTOs;
using BankingSystem.Business.Interfaces;
using BankingSystem.Common;
using BankingSystem.Common.CustomExceptions;
using BankingSystem.DAL.DbContexts;
using BankingSystem.DAL.DomainModels;
using BankingSystem.DAL.Interfaces;
using BankingSystem.DAL.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Business.Business
{
    public class AccountBusiness : IAccountBusiness
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccountBusiness(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public AccountBusiness()
        {
            _unitOfWork = new UnitOfWork(new BankingSystemDb());
        }

        public ResponseAccountInfo Balance(int accountNumber)
        {
            if (accountNumber <= 0)
            {
                throw new BadRequestException(AppMessages.AccountNumberNegative);
            }

            var account = _unitOfWork.AccountRepo.Read(a => a.AccountNumber == accountNumber)
                                                .Select(a => new ResponseAccountInfo()
                                                {
                                                    AccountId = a.Id,
                                                    AccountNumber = accountNumber,
                                                    Balance = a.Amount,
                                                    Currency = a.Currency
                                                }).SingleOrDefault();
            if (account == null)
            {
                throw new BadRequestException(string.Format(AppMessages.AccountDoesntExist, accountNumber));
            }

            account.Message = string.Format(AppMessages.InquiryAction, account.AccountNumber, account.Balance);
            CreateHistory(account.AccountId, ActionCode.Inquiry, account.Balance, account.Balance, account.Message);

            _unitOfWork.CommitChanges();

            return account;
        }

        public async Task<ResponseAccountInfo> Deposit(BaseRequest req)
        {
            if (req == null)
            {
                throw new BadRequestException("Null Argument.");
            }

            CurrenciesTestAPI currenciesAPI = new CurrenciesTestAPI();
            var currenciesTask = currenciesAPI.RequestCurrenciesAsyn(req.Currency);
            var accountTask = _unitOfWork.AccountRepo.ReadOneAsyn(a => a.AccountNumber == req.AccountNumber);

            var account = await accountTask;
            var currencies = await currenciesTask;

            if (!currencies.Rates.ContainsKey(account.Currency))
            {
                throw new BadRequestException(string.Format(AppMessages.AccountCurrencyNotSupport, account.AccountNumber, account.Currency));
            }

            decimal exchangedMoney = req.Amount * Convert.ToDecimal(currencies.Rates[account.Currency]);

            if (exchangedMoney < 0)
            {
                throw new BadRequestException(string.Format(AppMessages.ExchangedNegativeAmount, account.AccountNumber, account.Currency, req.Currency, currencies.Rates[account.Currency], exchangedMoney));
            }

            var oldBalance = account.Amount;
            account.Amount += exchangedMoney;

            ResponseAccountInfo result = new ResponseAccountInfo()
            {
                AccountId = account.Id,
                AccountNumber = account.AccountNumber,
                Balance = account.Amount,
                Currency = account.Currency,
                Message = string.Format(AppMessages.DepositMessageDetails, account.AccountNumber, oldBalance, account.Amount, currencies.Rates[account.Currency], req.Currency)
            };

            CreateHistory(result.AccountId, ActionCode.Deposit, oldBalance, account.Amount, result.Message);

            if (_unitOfWork.CommitChanges() > 0)
            {
                return result;
            }

            return null;
        }

        public async Task<ResponseAccountInfo> Withdraw(BaseRequest req)
        {
            if (req == null)
            {
                throw new BadRequestException("Null Argument.");
            }

            CurrenciesTestAPI currenciesAPI = new CurrenciesTestAPI();
            var currenciesTask = currenciesAPI.RequestCurrenciesAsyn(req.Currency);
            var accountTask = _unitOfWork.AccountRepo.ReadOneAsyn(a => a.AccountNumber == req.AccountNumber);

            var account = await accountTask;
            var currencies = await currenciesTask;

            if (!currencies.Rates.ContainsKey(account.Currency))
            {
                throw new BadRequestException(string.Format(AppMessages.AccountCurrencyNotSupport, account.AccountNumber, account.Currency));
            }

            decimal exchangedMoney = req.Amount * Convert.ToDecimal(currencies.Rates[account.Currency]);

            if (exchangedMoney < 0)
            {
                throw new BadRequestException(string.Format(AppMessages.ExchangedNegativeAmount, account.AccountNumber, account.Currency, req.Currency, currencies.Rates[account.Currency], exchangedMoney));
            }

            var oldBalance = account.Amount;
            if (exchangedMoney > account.Amount)
            {
                throw new BadRequestException(AppMessages.BalanceNotEnough);
            }
            else
            {
                account.Amount -= exchangedMoney;
            }

            var result = new ResponseAccountInfo()
            {
                AccountId = account.Id,
                AccountNumber = account.AccountNumber,
                Balance = account.Amount,
                Currency = account.Currency,
                Message = string.Format(AppMessages.WithdrawMessageDetails, account.AccountNumber, oldBalance, account.Amount, currencies.Rates[account.Currency], req.Currency)
            };

            CreateHistory(result.AccountId, ActionCode.Withdraw, oldBalance, account.Amount, result.Message);

            if (_unitOfWork.CommitChanges() > 0)
            {
                return result;
            }

            return null;
        }

        #region private function
        private void CreateHistory(int accountId, ActionCode action = ActionCode.Inquiry, decimal oldBalance = 0, decimal newBalance = 0, string message = "")
        {
            _unitOfWork.TransactionHistoryRepo.Create(new TransactionHistory() { AccountId = accountId, Action = action, NewAmount = newBalance, OldAmount = oldBalance, Details = message });
        }
        #endregion
    }
}
