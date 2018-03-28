using BankingSystem.Business;
using BankingSystem.Business.Business;
using BankingSystem.Business.DTOs;
using BankingSystem.Business.Interfaces;
using BankingSystem.Common;
using BankingSystem.Common.CustomExceptions;
using BankingSystem.DAL.DomainModels;
using BankingSystem.DAL.Interfaces;
using BankingSystem.DAL.UnitOfWork;
using BankingSystem.Tests.TestModels;
using FizzWare.NBuilder;
using Moq;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankingSystem.Tests.Businesses
{
    [TestFixture]
    public partial class AccountBusinessTest
    {
        private Mock<IDbContext> dbcontextMock;
        private Mock<ICurrenciesTestAPI> testApiMock;

        private IUnitOfWork unitOfWork;
        private IAccountBusiness accountBusiness;

        [SetUp]
        public void Setup()
        {
            dbcontextMock = new Mock<IDbContext>();
            testApiMock = new Mock<ICurrenciesTestAPI>();
            unitOfWork = new UnitOfWork(dbcontextMock.Object);
            accountBusiness = new AccountBusiness(unitOfWork, testApiMock.Object);
        }

        #region Balance
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-2)]
        [TestCase(-3)]
        public void Balance_GivenZeroOrNegativeNumber_Should_BusinessServerErrorException(int accountNumber)
        {
            //Arr
            //Act
            var result = Assert.Throws<BusinessServerErrorException>(() => accountBusiness.Balance(accountNumber));

            //Assert
            result.ShouldNotBeNull();
            result.Message.ShouldContain(AppMessages.AccountNumberNegative);
        }

        [TestCase(101)]
        [TestCase(100)]
        [TestCase(200)]
        [TestCase(300)]
        public void Balance_GivenPositiveAccountNumberButNotExisted_ShouldBusinessServerErrorException(int accountNumber)
        {
            //Arr
            dbcontextMock.Setup(a => a.Set<BankAccount>())
                .Returns(new FakeDbSet<BankAccount>(Builder<BankAccount>.CreateListOfSize(30).Build()));
            //Act
            var result = Assert.Throws<BusinessServerErrorException>(() => accountBusiness.Balance(accountNumber));

            //Assert
            result.ShouldNotBeNull();
            result.Message.ShouldContain(string.Format(AppMessages.AccountDoesntExistOrInactive, accountNumber));

            dbcontextMock.Verify(a => a.Set<BankAccount>(), Times.Once);
            dbcontextMock.Verify(a => a.Set<TransactionHistory>(), Times.Never);
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        public void Balance_GivenExistedPositiveAccountNumber_ShouldReturnGoodResponseData(int accountNumber)
        {
            //Arr
            dbcontextMock.Setup(a => a.Set<BankAccount>())
                .Returns(new FakeDbSet<BankAccount>(Builder<BankAccount>.CreateListOfSize(5).All().With(a => a.IsActive = true).Build()));
            dbcontextMock.Setup(a => a.Set<TransactionHistory>())
              .Returns(new FakeDbSet<TransactionHistory>(Builder<TransactionHistory>.CreateListOfSize(2).Build()));
            //Act
            var result = accountBusiness.Balance(accountNumber);

            //Assert
            result.ShouldNotBeNull();
            result.AccountNumber.ShouldBe(accountNumber);
            result.Message.ShouldNotBeNullOrEmpty();

            dbcontextMock.Verify(a => a.Set<BankAccount>(), Times.Once);
            dbcontextMock.Verify(a => a.Set<TransactionHistory>(), Times.Once);
            dbcontextMock.Verify(a => a.CommitChanges(), Times.Once);
        }
        #endregion

        #region Deposit
        [Test]
        public void Deposit_GivenNullArgurment_ShouldThrowBusinessErrorException()
        {
            var result = Assert.ThrowsAsync<BusinessServerErrorException>(() => accountBusiness.Deposit(null));

            result.ShouldNotBeNull();
            result.Message.Contains(AppMessages.NullArgument);

            dbcontextMock.Verify(a => a.Set<TransactionHistory>(), Times.Never);
            dbcontextMock.Verify(a => a.CommitChanges(), Times.Never);
        }

        [TestCaseSource("NegativeAccountNumber")]
        public void Deposit_GivenBaseRequestWithNegativeAccountNumber_ShouldThrowBusinessErrorException(BaseRequest req)
        {
            var result = Assert.ThrowsAsync<BusinessServerErrorException>(() => accountBusiness.Deposit(req));

            result.ShouldNotBeNull();
            result.Message.Contains(AppMessages.NullArgument);

            dbcontextMock.Verify(a => a.Set<TransactionHistory>(), Times.Never);
            dbcontextMock.Verify(a => a.CommitChanges(), Times.Never);
        }

        [TestCaseSource("NegativeAmount")]
        public void Deposit_GivenBaseRequestWithNegativeAmount_ShouldThrowBusinessErrorException(BaseRequest req)
        {
            var result = Assert.ThrowsAsync<BusinessServerErrorException>(() => accountBusiness.Deposit(req));

            result.ShouldNotBeNull();
            result.Message.Contains(AppMessages.NegativeAmount);

            dbcontextMock.Verify(a => a.Set<TransactionHistory>(), Times.Never);
            dbcontextMock.Verify(a => a.CommitChanges(), Times.Never);
        }

        [Test]
        public void Deposit_GivenUnSupportCurrency_ShouldThrowBusinessErrorException()
        {
            BaseRequest req = new BaseRequest()
            {
                AccountNumber = 1,
                Amount = 123,
                Currency = "VND"
            };

            Dictionary<string, double> rates = new Dictionary<string, double>();
            rates.Add("THB", 2);
            rates.Add("USD", 3);

            dbcontextMock.Setup(a => a.Set<BankAccount>())
                .Returns(new FakeDbSet<BankAccount>(Builder<BankAccount>.CreateListOfSize(2).All().With(a => a.IsActive = true).Build()));
            testApiMock.Setup(a => a.RequestCurrenciesAsyn(req.Currency))
                .Returns(Task.FromResult(new CurrenciesResponse()
                {
                    BaseCurrency = req.Currency,
                    Date = DateTime.Now,
                    Rates = rates
                }));

            var result = Assert.ThrowsAsync<BusinessServerErrorException>(() => accountBusiness.Deposit(req));

            result.ShouldNotBeNull();
            result.Message.Contains(string.Format(AppMessages.AccountCurrencyNotSupport, req.AccountNumber, req.Currency));

            testApiMock.Verify(a => a.RequestCurrenciesAsyn(req.Currency), Times.Once);
            dbcontextMock.Verify(a => a.Set<TransactionHistory>(), Times.Never);
            dbcontextMock.Verify(a => a.Set<BankAccount>(), Times.Once);
            dbcontextMock.Verify(a => a.CommitChanges(), Times.Never);
        }


        [Test]
        public void Deposit_GivenNegativeExchangedAmount_ShouldThrowBusinessErrorException()
        {
            BaseRequest req = new BaseRequest()
            {
                AccountNumber = 1,
                Amount = 123,
                Currency = "USD"
            };

            Dictionary<string, double> rates = new Dictionary<string, double>();
            rates.Add("THB", -2);
            rates.Add("USD", -3);
            var accounts = Builder<BankAccount>.CreateListOfSize(2).Build();
            accounts[0].Currency = "THB";
            accounts[0].IsActive = true;

            dbcontextMock.Setup(a => a.Set<BankAccount>())
                .Returns(new FakeDbSet<BankAccount>(accounts));
            testApiMock.Setup(a => a.RequestCurrenciesAsyn(req.Currency))
                .Returns(Task.FromResult(new CurrenciesResponse()
                {
                    BaseCurrency = req.Currency,
                    Date = DateTime.Now,
                    Rates = rates
                }));

            var result = Assert.ThrowsAsync<BusinessServerErrorException>(() => accountBusiness.Deposit(req));

            result.ShouldNotBeNull();

            testApiMock.Verify(a => a.RequestCurrenciesAsyn(req.Currency), Times.Once);
            dbcontextMock.Verify(a => a.Set<TransactionHistory>(), Times.Never);
            dbcontextMock.Verify(a => a.Set<BankAccount>(), Times.Once);
            dbcontextMock.Verify(a => a.CommitChanges(), Times.Never);
        }

        [Test]
        public async Task Deposit_GivenCorrectRequest_WithSameCurrency_ShouldDepositSuccessfulWithCorrectAmount()
        {
            BaseRequest req = new BaseRequest()
            {
                AccountNumber = 1,
                Amount = 123,
                Currency = "USD"
            };

            dbcontextMock.Setup(a => a.Set<BankAccount>())
                .Returns(new FakeDbSet<BankAccount>(new List<BankAccount>() { new BankAccount() { AccountNumber = 1, Amount = 100, Currency = "USD", IsActive = true } }));
            dbcontextMock.Setup(a => a.Set<TransactionHistory>())
                .Returns(new FakeDbSet<TransactionHistory>(Builder<TransactionHistory>.CreateListOfSize(1).Build()));
            dbcontextMock.Setup(a => a.CommitChanges()).Returns(1);

            var result = await accountBusiness.Deposit(req);

            result.ShouldNotBeNull();
            result.AccountNumber.ShouldBe(req.AccountNumber);
            result.Balance.ShouldBe(223);

            testApiMock.Verify(a => a.RequestCurrenciesAsyn(req.Currency), Times.Never);
            dbcontextMock.Verify(a => a.Set<TransactionHistory>(), Times.Once);
            dbcontextMock.Verify(a => a.Set<BankAccount>(), Times.Once);
            dbcontextMock.Verify(a => a.CommitChanges(), Times.Once);
        }

        [TestCaseSource("CorrectDepositBaseRequest")]
        public async Task Deposit_GivenCorrectRequest_WithDifferenntCurrency_ShouldDepositSuccessfulWithCorrectAmount(BaseRequest req, decimal expectedAmount)
        {
            Dictionary<string, double> rates = new Dictionary<string, double>();
            rates.Add("THB", 31.21);
            rates.Add("USD", 0.032);

            dbcontextMock.Setup(a => a.Set<BankAccount>())
                .Returns(new FakeDbSet<BankAccount>(new List<BankAccount>() { new BankAccount() { AccountNumber = 1, Amount = 100.33M, Currency = "USD", IsActive = true },
                new BankAccount() { AccountNumber = 2, Amount = 100, Currency = "THB", IsActive = true }}));
            dbcontextMock.Setup(a => a.Set<TransactionHistory>())
                .Returns(new FakeDbSet<TransactionHistory>(Builder<TransactionHistory>.CreateListOfSize(1).Build()));
            dbcontextMock.Setup(a => a.CommitChanges()).Returns(1);
            testApiMock.Setup(a => a.RequestCurrenciesAsyn(req.Currency))
                .Returns(Task.FromResult(new CurrenciesResponse()
                {
                    BaseCurrency = req.Currency,
                    Date = DateTime.Now,
                    Rates = rates
                }));

            var result = await accountBusiness.Deposit(req);

            result.ShouldNotBeNull();
            result.AccountNumber.ShouldBe(req.AccountNumber);
            result.Balance.ShouldBe(expectedAmount);

            testApiMock.Verify(a => a.RequestCurrenciesAsyn(req.Currency), Times.Once);
            dbcontextMock.Verify(a => a.Set<TransactionHistory>(), Times.Once);
            dbcontextMock.Verify(a => a.Set<BankAccount>(), Times.Once);
            dbcontextMock.Verify(a => a.CommitChanges(), Times.Once);
        }

        #endregion

        #region Withdraw
        [Test]
        public void Withdraw_GivenNullArgurment_ShouldThrowBusinessErrorException()
        {
            var result = Assert.ThrowsAsync<BusinessServerErrorException>(() => accountBusiness.Withdraw(null));

            result.ShouldNotBeNull();
            result.Message.Contains(AppMessages.NullArgument);

            dbcontextMock.Verify(a => a.Set<TransactionHistory>(), Times.Never);
            dbcontextMock.Verify(a => a.CommitChanges(), Times.Never);
        }

        [TestCaseSource("NegativeAccountNumber")]
        public void Withdraw_GivenBaseRequestWithNegativeAccountNumber_ShouldThrowBusinessErrorException(BaseRequest req)
        {
            var result = Assert.ThrowsAsync<BusinessServerErrorException>(() => accountBusiness.Withdraw(req));

            result.ShouldNotBeNull();
            result.Message.Contains(AppMessages.NullArgument);

            dbcontextMock.Verify(a => a.Set<TransactionHistory>(), Times.Never);
            dbcontextMock.Verify(a => a.CommitChanges(), Times.Never);
        }

        [TestCaseSource("NegativeAmount")]
        public void Withdraw_GivenBaseRequestWithNegativeAmount_ShouldThrowBusinessErrorException(BaseRequest req)
        {
            var result = Assert.ThrowsAsync<BusinessServerErrorException>(() => accountBusiness.Withdraw(req));

            result.ShouldNotBeNull();
            result.Message.Contains(AppMessages.NegativeAmount);

            dbcontextMock.Verify(a => a.Set<TransactionHistory>(), Times.Never);
            dbcontextMock.Verify(a => a.CommitChanges(), Times.Never);
        }

        [Test]
        public void Withdraw_GivenUnSupportCurrency_ShouldThrowBusinessErrorException()
        {
            BaseRequest req = new BaseRequest()
            {
                AccountNumber = 1,
                Amount = 123,
                Currency = "VND"
            };

            Dictionary<string, double> rates = new Dictionary<string, double>();
            rates.Add("THB", 2);
            rates.Add("USD", 3);

            dbcontextMock.Setup(a => a.Set<BankAccount>())
                .Returns(new FakeDbSet<BankAccount>(Builder<BankAccount>.CreateListOfSize(2).All().With(a => a.IsActive = true).Build()));
            testApiMock.Setup(a => a.RequestCurrenciesAsyn(req.Currency))
                .Returns(Task.FromResult(new CurrenciesResponse()
                {
                    BaseCurrency = req.Currency,
                    Date = DateTime.Now,
                    Rates = rates
                }));

            var result = Assert.ThrowsAsync<BusinessServerErrorException>(() => accountBusiness.Withdraw(req));

            result.ShouldNotBeNull();
            result.Message.Contains(string.Format(AppMessages.AccountCurrencyNotSupport, req.AccountNumber, req.Currency));

            testApiMock.Verify(a => a.RequestCurrenciesAsyn(req.Currency), Times.Once);
            dbcontextMock.Verify(a => a.Set<TransactionHistory>(), Times.Never);
            dbcontextMock.Verify(a => a.Set<BankAccount>(), Times.Once);
            dbcontextMock.Verify(a => a.CommitChanges(), Times.Never);
        }


        [Test]
        public void Withdraw_GivenNegativeExchangedAmount_ShouldThrowBusinessErrorException()
        {
            BaseRequest req = new BaseRequest()
            {
                AccountNumber = 1,
                Amount = 123,
                Currency = "USD"
            };

            Dictionary<string, double> rates = new Dictionary<string, double>();
            rates.Add("THB", -2);
            rates.Add("USD", -3);
            var accounts = Builder<BankAccount>.CreateListOfSize(2).Build();
            accounts[0].Currency = "THB";
            accounts[0].IsActive = true;

            dbcontextMock.Setup(a => a.Set<BankAccount>())
                .Returns(new FakeDbSet<BankAccount>(accounts));
            testApiMock.Setup(a => a.RequestCurrenciesAsyn(req.Currency))
                .Returns(Task.FromResult(new CurrenciesResponse()
                {
                    BaseCurrency = req.Currency,
                    Date = DateTime.Now,
                    Rates = rates
                }));

            var result = Assert.ThrowsAsync<BusinessServerErrorException>(() => accountBusiness.Withdraw(req));

            result.ShouldNotBeNull();

            testApiMock.Verify(a => a.RequestCurrenciesAsyn(req.Currency), Times.Once);
            dbcontextMock.Verify(a => a.Set<TransactionHistory>(), Times.Never);
            dbcontextMock.Verify(a => a.Set<BankAccount>(), Times.Once);
            dbcontextMock.Verify(a => a.CommitChanges(), Times.Never);
        }

        [Test]
        public void Withdraw_GivenCorrectRequest_WithSameCurrency_NotEnoughBalance_ShouldThrowBusinessException()
        {
            BaseRequest req = new BaseRequest()
            {
                AccountNumber = 1,
                Amount = 123,
                Currency = "USD"
            };

            var account = new BankAccount() { AccountNumber = 1, Amount = 100, Currency = "USD", IsActive = true };
            dbcontextMock.Setup(a => a.Set<BankAccount>())
                .Returns(new FakeDbSet<BankAccount>(new List<BankAccount>() { account }));
            dbcontextMock.Setup(a => a.Set<TransactionHistory>())
                .Returns(new FakeDbSet<TransactionHistory>(Builder<TransactionHistory>.CreateListOfSize(1).Build()));

            var result = Assert.ThrowsAsync<BusinessServerErrorException>(() => accountBusiness.Withdraw(req));

            result.ShouldNotBeNull();
            result.Message.Contains(AppMessages.BalanceNotEnough);

            testApiMock.Verify(a => a.RequestCurrenciesAsyn(req.Currency), Times.Never);
            dbcontextMock.Verify(a => a.Set<TransactionHistory>(), Times.Never);
            dbcontextMock.Verify(a => a.Set<BankAccount>(), Times.Once);
            dbcontextMock.Verify(a => a.CommitChanges(), Times.Never);
        }

        [TestCaseSource("CorrectWithdrawBaseRequest")]
        public async Task Withdraw_GivenCorrectRequest_WithDifferenntCurrency_ShouldWithdrawSuccessfulWithCorrectAmount(BaseRequest req, decimal expectedAmount)
        {
            Dictionary<string, double> rates = new Dictionary<string, double>();
            rates.Add("THB", 31.21);
            rates.Add("USD", 0.032);

            dbcontextMock.Setup(a => a.Set<BankAccount>())
                .Returns(new FakeDbSet<BankAccount>(new List<BankAccount>() { new BankAccount() { AccountNumber = 1, Amount = 100.33M, Currency = "USD", IsActive = true },
                new BankAccount() { AccountNumber = 2, Amount = 10000, Currency = "THB", IsActive = true }}));
            dbcontextMock.Setup(a => a.Set<TransactionHistory>())
                .Returns(new FakeDbSet<TransactionHistory>(Builder<TransactionHistory>.CreateListOfSize(1).Build()));
            dbcontextMock.Setup(a => a.CommitChanges()).Returns(1);
            testApiMock.Setup(a => a.RequestCurrenciesAsyn(req.Currency))
                .Returns(Task.FromResult(new CurrenciesResponse()
                {
                    BaseCurrency = req.Currency,
                    Date = DateTime.Now,
                    Rates = rates
                }));

            var result = await accountBusiness.Withdraw(req);

            result.ShouldNotBeNull();
            result.AccountNumber.ShouldBe(req.AccountNumber);
            result.Balance.ShouldBe(expectedAmount);

            testApiMock.Verify(a => a.RequestCurrenciesAsyn(req.Currency), Times.Once);
            dbcontextMock.Verify(a => a.Set<TransactionHistory>(), Times.Once);
            dbcontextMock.Verify(a => a.Set<BankAccount>(), Times.Once);
            dbcontextMock.Verify(a => a.CommitChanges(), Times.Once);
        }

        [Test]
        public async Task Withdraw_GivenCorrectRequest_WithSameCurrency_ShouldWithdrawSuccessfulWithCorrectAmount()
        {
            BaseRequest req = new BaseRequest()
            {
                AccountNumber = 1,
                Amount = 123,
                Currency = "USD"
            };
            var account = new BankAccount() { AccountNumber = 1, Amount = 200, Currency = "USD", IsActive = true };
            dbcontextMock.Setup(a => a.Set<BankAccount>())
                .Returns(new FakeDbSet<BankAccount>(new List<BankAccount>() { account }));
            dbcontextMock.Setup(a => a.Set<TransactionHistory>())
                .Returns(new FakeDbSet<TransactionHistory>(Builder<TransactionHistory>.CreateListOfSize(1).Build()));
            dbcontextMock.Setup(a => a.CommitChanges()).Returns(1);

            var result = await accountBusiness.Withdraw(req);

            result.ShouldNotBeNull();
            result.AccountNumber.ShouldBe(req.AccountNumber);
            result.Balance.ShouldBe(200 - req.Amount);

            testApiMock.Verify(a => a.RequestCurrenciesAsyn(req.Currency), Times.Never);
            dbcontextMock.Verify(a => a.Set<TransactionHistory>(), Times.Once);
            dbcontextMock.Verify(a => a.Set<BankAccount>(), Times.Once);
            dbcontextMock.Verify(a => a.CommitChanges(), Times.Once);
        }

        [TestCaseSource("CorrectWithdrawBaseRequest_NotEnoughBalance")]
        public void Withdraw_GivenCorrectRequest_WithDifferenntCurrency_NotEnoughBalance_ShouldThrowBusinessException(BaseRequest req)
        {
            Dictionary<string, double> rates = new Dictionary<string, double>();
            rates.Add("THB", 31.21);
            rates.Add("USD", 0.032);

            dbcontextMock.Setup(a => a.Set<BankAccount>())
                .Returns(new FakeDbSet<BankAccount>(new List<BankAccount>() { new BankAccount() { AccountNumber = 1, Amount = 1M, Currency = "USD", IsActive = true },
                new BankAccount() { AccountNumber = 2, Amount = 3120, Currency = "THB", IsActive = true }}));
            dbcontextMock.Setup(a => a.Set<TransactionHistory>())
                .Returns(new FakeDbSet<TransactionHistory>(Builder<TransactionHistory>.CreateListOfSize(1).Build()));
            dbcontextMock.Setup(a => a.CommitChanges()).Returns(1);
            testApiMock.Setup(a => a.RequestCurrenciesAsyn(req.Currency))
                .Returns(Task.FromResult(new CurrenciesResponse()
                {
                    BaseCurrency = req.Currency,
                    Date = DateTime.Now,
                    Rates = rates
                }));

            var result = Assert.ThrowsAsync<BusinessServerErrorException>(() => accountBusiness.Withdraw(req));

            result.ShouldNotBeNull();
            result.Message.Contains(AppMessages.BalanceNotEnough);

            testApiMock.Verify(a => a.RequestCurrenciesAsyn(req.Currency), Times.Once);
            dbcontextMock.Verify(a => a.Set<TransactionHistory>(), Times.Never);
            dbcontextMock.Verify(a => a.Set<BankAccount>(), Times.Once);
            dbcontextMock.Verify(a => a.CommitChanges(), Times.Never);
        }
        #endregion

    }
}
