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
        private readonly ICurrenciesTestAPI _currencyApi;

        public AccountBusiness(IUnitOfWork unitOfWork, ICurrenciesTestAPI api)
        {
            _unitOfWork = unitOfWork;
            _currencyApi = api;
        }

        public AccountBusiness()
        {
            _unitOfWork = new UnitOfWork(new BankingSystemDb());
            _currencyApi = new CurrenciesTestAPI();
        }

        public ResponseAccountInfo Balance(int accountNumber)
        {
            if (accountNumber <= 0)
            {
                throw new BusinessServerErrorException(AppMessages.AccountNumberNegative);
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
                throw new BusinessServerErrorException(string.Format(AppMessages.AccountDoesntExistOrInactive, accountNumber));
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
                throw new BusinessServerErrorException(AppMessages.NullArgument);
            }

            if (req.AccountNumber <= 0)
            {
                throw new BusinessServerErrorException(AppMessages.AccountNumberNegative);
            }

            if (req.Amount <= 0)
            {
                throw new BusinessServerErrorException(AppMessages.NegativeAmount);
            }

            decimal exchangedMoney = req.Amount;
            var account = _unitOfWork.AccountRepo.ReadOne(a => a.AccountNumber == req.AccountNumber);

            if (account == null)
            {
                throw new BusinessServerErrorException(string.Format(AppMessages.AccountDoesntExistOrInactive, req.AccountNumber));
            }

            if (req.Currency != account.Currency)
            {
                var currencies = await _currencyApi.RequestCurrenciesAsyn(req.Currency);

                if (!currencies.Rates.ContainsKey(req.Currency))
                {
                    throw new BusinessServerErrorException(string.Format(AppMessages.AccountCurrencyNotSupport, account.AccountNumber, account.Currency));
                }

                exchangedMoney = req.Amount * Convert.ToDecimal(currencies.Rates[account.Currency]);

            }

            if (exchangedMoney < 0)
            {
                throw new BusinessServerErrorException(string.Format(AppMessages.ExchangedNegativeAmount, account.AccountNumber, account.Currency, exchangedMoney));
            }

            var oldBalance = account.Amount;
            account.Amount += exchangedMoney;

            ResponseAccountInfo result = new ResponseAccountInfo()
            {
                AccountId = account.Id,
                AccountNumber = account.AccountNumber,
                Balance = account.Amount,
                Currency = account.Currency,
                Message = string.Format(AppMessages.DepositMessageDetails, account.AccountNumber, oldBalance, account.Amount, req.Currency)
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
                throw new BusinessServerErrorException(AppMessages.NullArgument);
            }

            if (req.AccountNumber <= 0)
            {
                throw new BusinessServerErrorException(AppMessages.AccountNumberNegative);
            }

            if (req.Amount <= 0)
            {
                throw new BusinessServerErrorException(AppMessages.NegativeAmount);
            }

           
            var account = _unitOfWork.AccountRepo.ReadOne(a => a.AccountNumber == req.AccountNumber);

            if (account == null)
            {
                throw new BusinessServerErrorException(string.Format(AppMessages.AccountDoesntExistOrInactive, req.AccountNumber));
            }

            decimal exchangedMoney = req.Amount;
            if (account.Currency != req.Currency)
            {
                var currencies = await _currencyApi.RequestCurrenciesAsyn(req.Currency);

                if (!currencies.Rates.ContainsKey(req.Currency))
                {
                    throw new BusinessServerErrorException(string.Format(AppMessages.AccountCurrencyNotSupport, account.AccountNumber, account.Currency));
                }

                exchangedMoney = req.Amount * Convert.ToDecimal(currencies.Rates[account.Currency]);
            }

            if (exchangedMoney < 0)
            {
                throw new BusinessServerErrorException(string.Format(AppMessages.ExchangedNegativeAmount, account.AccountNumber, account.Currency, exchangedMoney));
            }

            var oldBalance = account.Amount;
            if (exchangedMoney > account.Amount)
            {
                throw new BusinessServerErrorException(AppMessages.BalanceNotEnough);
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
                Message = string.Format(AppMessages.WithdrawMessageDetails, account.AccountNumber, oldBalance, account.Amount, req.Currency)
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
