﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace Launchpad.Models.UnitTests
{
    public class RegistrationModelTests
    {
        [Fact]
        public void Email_Is_Required()
        {
            var model = new RegistrationModel() { Password = "01234567", ConfirmPassword="01234567" };
            var result = model.RunValidations();

            result.Item1.Should().BeFalse();
            result.Item2.Should().HaveCount(1);
            result.Item2[0].MemberNames.Should().Contain("Email");
        }

        [Theory]
        [InlineData("12")]
        [InlineData("012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678911")]
        public void Password_Fails_Validation_When_Length_Is_Invalid(string password)
        {
            var model = new RegistrationModel() {
                Email = "fred@fred.com",
                Password = password,
                ConfirmPassword = password
            };

            var result = model.RunValidations();

            result.Item1.Should().BeFalse();
            result.Item2.Should().HaveCount(1);
            result.Item2[0].MemberNames.Should().Contain("Password");
        }

        [Theory]
        [InlineData("0123456")]
        [InlineData("0123456890")]
        public void Password_Passes_Validation_When_Length_Is_Valid(string password)
        {
            var model = new RegistrationModel()
            {
                Email = "fred@fred.com",
                Password = password,
                ConfirmPassword = password
            };

            var result = model.RunValidations();

            result.Item1.Should().BeTrue();
        }

        [Fact]
        public void ConfirmPassword_Fails_Validation_When_It_Does_Not_Match_Password()
        {
            var model = new RegistrationModel()
            {
                Email = "fred@fred.com",
                Password = "01234567",
                ConfirmPassword = "01234567abc"
            };

            var result = model.RunValidations();

            result.Item1.Should().BeFalse();
            result.Item2.Should().HaveCount(1);
            result.Item2[0].ErrorMessage.Should().Be("The password and confirmation password do not match.");
        }
    }
}