﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using FluentAssertions;
using Microsoft.Owin.Security;
using Moq;
using Voyage.Api.API.v1;
using Voyage.Api.API.V1;
using Voyage.Api.UnitTests.Common;
using Voyage.Models;
using Voyage.Services.Notification;
using Xunit;

namespace Voyage.Api.UnitTests.API.V1
{
    public class NotificationsControllerTests : BaseUnitTest
    {
        private readonly NotificationsController _controller;
        private readonly Mock<INotificationService> _mockNotificationService;
        private readonly Mock<IAuthenticationManager> _mockAuthenticationManager;

        public NotificationsControllerTests()
        {
            _mockNotificationService = Mock.Create<INotificationService>();
            _mockAuthenticationManager = Mock.Create<IAuthenticationManager>();

            _controller = new NotificationsController(_mockNotificationService.Object, _mockAuthenticationManager.Object)
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };
        }

        [Fact]
        public void NotificationsController_Constructor_ParametersAreNull_ShouldThrowArgumentNullException()
        {
            Action throwAction = () => new NotificationsController(null, null);

            throwAction.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public async void NotificationsController_GetNotifications_ShouldCallServiceToGetNotifications()
        {
            // Arrange
            _mockNotificationService.Setup(_ => _.GetNotifications(It.IsAny<string>()))
                .ReturnsAsync(new List<NotificationModel>
                {
                    new NotificationModel()
                });

            var principal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "UserId")
            }));
            _mockAuthenticationManager.SetupGet(_ => _.User).Returns(principal);

            // Act
            var result = await _controller.GetNotifications();

            // Assert
            result.Should().BeOfType<OkNegotiatedContentResult<IEnumerable<NotificationModel>>>();

            var resultContent = result.As<OkNegotiatedContentResult<IEnumerable<NotificationModel>>>();
            resultContent.Content.Count().Should().Be(1);

            Mock.VerifyAll();
        }

        [Fact]
        public async void NotificationsController_MarkNotificationAsRead_ShouldCallServiceToMarkNotificationAsRead()
        {
            // Arrange
            _mockNotificationService.Setup(_ => _.MarkNotificationAsRead(It.IsAny<string>(), It.IsAny<int>())).Returns(Task.Delay(0));

            var principal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "UserId")
            }));
            _mockAuthenticationManager.SetupGet(_ => _.User).Returns(principal);

            // Act
            await _controller.MarkNotificationAsRead(1);

            // Assert
            Mock.VerifyAll();
        }

        [Fact]
        public async void NotificationsController_MarkAllNotificationsAsRead_ShouldCallServiceToMarkNotificationsAllAsRead()
        {
            // Arrange
            _mockNotificationService.Setup(_ => _.MarkAllNotificationsAsRead(It.IsAny<string>())).Returns(Task.Delay(0));

            var principal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "UserId")
            }));
            _mockAuthenticationManager.SetupGet(_ => _.User).Returns(principal);

            // Act
            await _controller.MarkAllNotificationsAsRead();

            // Assert
            Mock.VerifyAll();
        }

        [Fact]
        public async void NotificationsController_CreateNotification_ShouldCallServiceToCreateNotification()
        {
            // Arrange
            _mockNotificationService.Setup(_ => _.CreateNotification(It.IsAny<NotificationModel>()))
                .ReturnsAsync(new NotificationModel { Id = 1 });

            // Act
            var result = await _controller.CreateNotification(new NotificationModel());

            // Assert
            result.Should().BeOfType<OkNegotiatedContentResult<NotificationModel>>();

            var resultContent = result.As<OkNegotiatedContentResult<NotificationModel>>();
            resultContent.Content.Id.Should().Be(1);

            Mock.VerifyAll();
        }

        [Fact]
        public void NotificationsController_ShouldHaveV1RoutePrefixAttribute()
        {
            typeof(ApplicationInfoController).Should()
                .BeDecoratedWith<RoutePrefixAttribute>(
                _ => _.Prefix.Equals(Constants.RoutePrefixes.V1));
        }

        [Fact]
        public void NotificationsController_GetNotifications_ShouldHaveRouteAttribute()
        {
            ReflectionHelper.GetMethod<NotificationsController>(_ => _.GetNotifications())
             .Should()
             .BeDecoratedWith<RouteAttribute>(_ => _.Template.Equals("notifications"));
        }
    }
}
