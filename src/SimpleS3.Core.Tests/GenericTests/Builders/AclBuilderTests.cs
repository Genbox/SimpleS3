using System;
using Genbox.SimpleS3.Core.Builders;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.TestBase;
using Xunit;

namespace Genbox.SimpleS3.Core.Tests.GenericTests.Builders
{
    public class AclBuilderTests
    {
        [Fact]
        public void GenericTest()
        {
            AclBuilder acl = new AclBuilder();
            Assert.Null(acl.Build());

            acl.AddEmail("email1@example.com");
            acl.AddEmail("other@email.com");
            Assert.Equal("emailAddress=\"email1@example.com\",emailAddress=\"other@email.com\"", acl.Build());

            acl.AddUserId(TestConstants.TestUserId);
            Assert.Equal($"emailAddress=\"email1@example.com\",emailAddress=\"other@email.com\",id=\"{TestConstants.TestUserId}\"", acl.Build());

            acl.AddGroup(PredefinedGroup.AllUsers);
            Assert.Equal($"emailAddress=\"email1@example.com\",emailAddress=\"other@email.com\",id=\"{TestConstants.TestUserId}\",uri=\"http://acs.amazonaws.com/groups/global/AllUsers\"", acl.Build());
        }

        [Fact]
        public void TestDuplicate()
        {
            AclBuilder acl = new AclBuilder();
            acl.AddEmail("email1@example.com");
            acl.AddUserId(TestConstants.TestUserId);
            acl.AddGroup(PredefinedGroup.AuthenticatedUsers);

            Assert.Throws<ArgumentException>(() => acl.AddEmail("email1@example.com"));
            Assert.Throws<ArgumentException>(() => acl.AddUserId(TestConstants.TestUserId));
            Assert.Throws<ArgumentException>(() => acl.AddGroup(PredefinedGroup.AuthenticatedUsers));
        }
    }
}