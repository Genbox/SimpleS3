using System;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Builders;
using Xunit;

namespace Genbox.SimpleS3.Core.Tests.GenericTests.S3Builders
{
    public class AclBuilderTests
    {
        [Fact]
        public void GenericTest()
        {
            AclBuilder acl = new AclBuilder();
            Assert.Null(acl.Build());

            acl.AddEmail(TestConstants.TestEmail);
            acl.AddEmail("other@email.com");
            Assert.Equal($"emailAddress=\"{TestConstants.TestEmail}\",emailAddress=\"other@email.com\"", acl.Build());

            acl.AddUserId(TestConstants.TestUserId);
            Assert.Equal($"emailAddress=\"{TestConstants.TestEmail}\",emailAddress=\"other@email.com\",id=\"{TestConstants.TestUserId}\"", acl.Build());

            acl.AddGroup(PredefinedGroup.AllUsers);
            Assert.Equal($"emailAddress=\"{TestConstants.TestEmail}\",emailAddress=\"other@email.com\",id=\"{TestConstants.TestUserId}\",uri=\"http://acs.amazonaws.com/groups/global/AllUsers\"", acl.Build());
        }

        [Fact]
        public void TestDuplicate()
        {
            AclBuilder acl = new AclBuilder();
            acl.AddEmail(TestConstants.TestEmail);
            acl.AddUserId(TestConstants.TestUserId);
            acl.AddGroup(PredefinedGroup.AuthenticatedUsers);

            Assert.Throws<ArgumentException>(() => acl.AddEmail(TestConstants.TestEmail));
            Assert.Throws<ArgumentException>(() => acl.AddUserId(TestConstants.TestUserId));
            Assert.Throws<ArgumentException>(() => acl.AddGroup(PredefinedGroup.AuthenticatedUsers));
        }
    }
}