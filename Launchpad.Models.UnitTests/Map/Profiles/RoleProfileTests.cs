﻿using Launchpad.Models.UnitTests.Fixture;
using Launchpad.UnitTests.Common;
using Xunit;
using FluentAssertions;
using Launchpad.Models.EntityFramework;

namespace Launchpad.Models.UnitTests.Map.Profiles
{
    [Collection(AutoMapperCollection.CollectionName)]
    public class RoleProfileTests : BaseUnitTest
    {
        private AutoMapperFixture _mappingFixture;

        public RoleProfileTests(AutoMapperFixture mappingFixture)
        {
            _mappingFixture = mappingFixture;
        }

        [Fact]
        public void ApplicationRole_Should_Map_To_RoleModel()
        {
            var appRole = new ApplicationRole();
            appRole.Name = "role1";
            appRole.Id = "123";

            var role = _mappingFixture.MapperInstance.Map<RoleModel>(appRole);

            role.Should().NotBeNull();
            role.Name.Should().Be(appRole.Name);
            role.Id.Should().Be(appRole.Id);
        }

    }
}