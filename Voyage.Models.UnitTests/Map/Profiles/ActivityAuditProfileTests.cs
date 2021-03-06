﻿using FluentAssertions;
using Ploeh.AutoFixture;
using Voyage.Models.Entities;
using Voyage.Models.UnitTests.Common;
using Voyage.Models.UnitTests.Common.AutoMapperFixture;
using Xunit;

namespace Voyage.Models.UnitTests.Map.Profiles
{
    [Collection(AutoMapperCollection.CollectionName)]
    public class ActivityAuditProfileTests : BaseUnitTest
    {
        private readonly AutoMapperFixture _mappingFixture;

        public ActivityAuditProfileTests(AutoMapperFixture mappingFixture)
        {
            _mappingFixture = mappingFixture;
        }

        [Fact]
        public void ActivityAuditModel_Should_Map_To_ActivityAudit()
        {
            var model = Fixture.Create<ActivityAuditModel>();

            var result = _mappingFixture.MapperInstance.Map<ActivityAudit>(model);

            result.Date.Should().Be(model.Date);
            result.Error.Should().Be(model.Error);
            result.IpAddress.Should().Be(model.IpAddress);
            result.Method.Should().Be(model.Method);
            result.Path.Should().Be(model.Path);
            result.RequestId.Should().Be(model.RequestId);
            result.StatusCode.Should().Be(model.StatusCode);
            result.UserName.Should().Be(model.UserName);
            result.Id.Should().Be(default(int));
        }
    }
}
