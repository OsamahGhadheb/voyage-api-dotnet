﻿using System;
using System.Collections.Generic;
using System.Security.Claims;
using Autofac;
using FluentAssertions;
using Voyage.Services.User;
using Voyage.UnitTests.Common;
using Voyage.Web.AuthProviders;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security.OAuth.Messages;
using Moq;
using Ploeh.AutoFixture;
using Xunit;

namespace Voyage.UnitTests.Web.AuthProviders
{
    [Trait("Category", "OAuthProvider")]
    public class ApplicationOAuthProviderTests : BaseUnitTest
    {
        private const string PathString = "/api/v1/login";
        private readonly string _clientId;
        private readonly ApplicationOAuthProvider _provider;
        private readonly Mock<IOwinContext> _mockOwinContext;

        public ApplicationOAuthProviderTests()
        {
            _clientId = Fixture.Create<string>();
            _mockOwinContext = Mock.Create<IOwinContext>();
            _provider = new ApplicationOAuthProvider();
        }

        [Fact]
        public async void GrantResourceOwnerCredentials_Should_SetError_When_Invalid_User()
        {
            var user = "bob@bob.com";
            var password = "giraffe";
            OAuthGrantResourceOwnerCredentialsContext oAuthContext = new OAuthGrantResourceOwnerCredentialsContext(_mockOwinContext.Object, new OAuthAuthorizationServerOptions(), _clientId, user, password, new List<string>());

            // Setup the user service
            var mockUserService = Mock.Create<IUserService>();
            mockUserService.Setup(x => x.IsValidCredential(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);

            // Skip mocking out autofac, just build the container to use
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Register(c => mockUserService.Object);
            var container = containerBuilder.Build();

            _mockOwinContext.Setup(_ => _.Get<ILifetimeScope>(It.IsAny<string>()))
                .Returns(container);

            // ACT
            await _provider.GrantResourceOwnerCredentials(oAuthContext);

            // ASSERT
            Mock.VerifyAll();
            oAuthContext.HasError.Should().BeTrue();
        }

        [Fact]
        public async void GrantResourceOwnerCredentials_Should_Call_User_Service()
        {
            var user = "bob@bob.com";
            var password = "giraffe";
            OAuthGrantResourceOwnerCredentialsContext oAuthContext = new OAuthGrantResourceOwnerCredentialsContext(_mockOwinContext.Object, new OAuthAuthorizationServerOptions(), _clientId, user, password, new List<string>());

            // Identity fake
            var identity = new ClaimsIdentity();

            // Setup the user service
            var mockUserService = Mock.Create<IUserService>();

            mockUserService.Setup(_ => _.IsValidCredential(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
            mockUserService.Setup(_ => _.CreateClaimsIdentityAsync(user, OAuthDefaults.AuthenticationType))
                .ReturnsAsync(identity);
            mockUserService.Setup(_ => _.CreateClaimsIdentityAsync(user, CookieAuthenticationDefaults.AuthenticationType))
                .ReturnsAsync(identity);

            // Skip mocking out autofac, just build the container to use
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Register(c => mockUserService.Object);
            var container = containerBuilder.Build();

            // Property used for sign in
            var mockAuthenticationManager = Mock.Create<IAuthenticationManager>();
            mockAuthenticationManager.Setup(_ => _.SignIn(identity));
            _mockOwinContext.Setup(_ => _.Authentication).Returns(mockAuthenticationManager.Object);

            // Configure the context properties
            var mockOwinRequest = Mock.Create<IOwinRequest>();

            mockOwinRequest.Setup(_ => _.Context).Returns(_mockOwinContext.Object);
            _mockOwinContext.Setup(_ => _.Request).Returns(mockOwinRequest.Object);
            _mockOwinContext.Setup(_ => _.Get<ILifetimeScope>(It.IsAny<string>()))
                .Returns(container);

            // ACT
            await _provider.GrantResourceOwnerCredentials(oAuthContext);

            // ASSERT
            Mock.VerifyAll();
        }
    }
}
