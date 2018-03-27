using BankingSystem.Business.Business;
using BankingSystem.Business.DTOs;
using BankingSystem.Business.Interfaces;
using BankingSystem.Filters;
using System.Threading.Tasks;
using System.Web.Http;


namespace BankingSystem.Controllers
{
    [ApiExceptionFilter]
    public class AccountController : BaseController
    {
        private readonly IAccountBusiness _accountBusiness;
        public AccountController(IAccountBusiness accountBusiness)
        {
            _accountBusiness = accountBusiness;
        }

        public AccountController()
        {
            _accountBusiness = new AccountBusiness();
        }

        [HttpGet]
        [Route("api/account/{accountNumber:int}")]
       public IHttpActionResult Balance(int? accountNumber)
       {
            var result = _accountBusiness.Balance(accountNumber ?? 0);

            return Json(result);
       }

        [HttpPost]
        [Route("api/account/{accountNumber:int}/Withdraw")]
        public async Task<IHttpActionResult> Withdraw(BaseRequest req)
        {
            var result = await _accountBusiness.Withdraw(req);

            return Json(result);
        }

        [HttpPost]
        [Route("api/account/{accountNumber:int}/Deposit")]
        public async Task<IHttpActionResult> Deposit(BaseRequest req)
        {
            var result = await _accountBusiness.Deposit(req);

            return Json(result);
        }
    }
}