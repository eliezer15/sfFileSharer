using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using ShareFile.Api.Client;
using ShareFile.Api.Client.Exceptions;
using ShareFile.Api.Client.Security.Authentication.OAuth2;
using WebhookEndpoint.Models;

namespace WebhookEndpoint.Services
{
    public class ShareFileClientFactory : IShareFileClientFactory
    {
        private ShareFileOptions _sfConfig;
        private IShareFileClient _instance;
        public ShareFileClientFactory(IOptions<ShareFileOptions> sfConfig)
        {
            _sfConfig = sfConfig.Value;
        }

        public async Task<IShareFileClient> GetClientAsync()
        {
            try
            {
                await _instance.Accounts.Get().ExecuteAsync();
            }
            catch (Exception ex) when (ex is WebAuthenticationException || ex is NullReferenceException)
            {
                _instance = await GetAuthenticatedClient();
            }

            return _instance;
        }

        private async Task<IShareFileClient> GetAuthenticatedClient()
        {
            var baseUrl = $"https://{_sfConfig.Subdomain}.sf-api.com/sf/v3/";
            var client = new ShareFileClient(baseUrl);

            var oauthService = new OAuthService(client, _sfConfig.ClientId, _sfConfig.ClientSecret);
            var token = await oauthService.PasswordGrantAsync(_sfConfig.Username, _sfConfig.Password, _sfConfig.Subdomain, "sharefile.com");

            client.AddOAuthCredentials(token);
            return client;
        }
    }
}