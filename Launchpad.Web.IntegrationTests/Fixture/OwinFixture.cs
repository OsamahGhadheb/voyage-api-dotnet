﻿using FluentAssertions;
using Microsoft.Owin.Hosting;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Launchpad.Web.IntegrationTests.Fixture
{
    public class OwinFixture : IDisposable
    {
        private IDisposable _webApp;
        
        public string BaseAddress => "http://localhost:9000";

        public string DefaultToken { get; }

        public HttpClient DefaultClient { get; private set; }

        /// <summary>
        /// Requests a new authorization token from the server
        /// </summary>
        /// <returns>Authorization Token</returns>
        protected async Task<string> GenerateToken()
        {
           

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, $"{BaseAddress}/api/v1/login");
            httpRequestMessage.Content = new StringContent("grant_type=password&username=admin%40admin.com&password=Hello123!", Encoding.UTF8,
                                "application/x-www-form-urlencoded");

            var response = await DefaultClient.SendAsync(httpRequestMessage);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var rawResponse = response.Content.ReadAsStringAsync().Result;
            dynamic tokenResponse = JsonConvert.DeserializeObject(rawResponse);
            string token = tokenResponse.access_token;
            token.Should().NotBeNullOrEmpty();

            return token;
        }


        public OwinFixture()
        {           
            _webApp = WebApp.Start<Startup>(url: BaseAddress);
            DefaultClient = new HttpClient();
            DefaultToken = GenerateToken().Result;
        }


        public void Dispose()
        {
            if(_webApp != null)
            {
                _webApp.Dispose();
                _webApp = null;
            }

            if (DefaultClient != null)
            {
                DefaultClient.Dispose();
            }
            
        }
    }
}