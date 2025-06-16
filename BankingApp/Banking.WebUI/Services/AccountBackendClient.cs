using Refit;

namespace Banking.WebUI.Services
{
    public class AccountBackendClient : IAccountBackendClient
    {
        IHttpClientFactory _httpClientFactory;

        public AccountBackendClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<HttpResponseMessage> AccountTransfer(AccountTransfer accountTransfer)
        {
            var client = _httpClientFactory.CreateClient("Accounts");

            return await Task.Run(() =>
            {
                lock (accountTransfer)
                {
                    var result = RestService.For<IAccountBackendClient>(client).AccountTransfer(accountTransfer).Result;
                    return result;
                }

            });
        }

        public Task<List<Account>> GetAccounts()
        {
            var client = _httpClientFactory.CreateClient("Accounts");

            return RestService.For<IAccountBackendClient>(client).GetAccounts();
        }
    }
}
