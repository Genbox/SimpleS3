using System;
using Genbox.SimpleS3.Core.Builders;
using Genbox.SimpleS3.Core.Enums;
using Xunit;

namespace Genbox.SimpleS3.Core.Tests.GenericTests.Builders
{
    public class AclBuilderTests
    {
        private string _testUserId = "deadbeefcafe0000000000000000000000000000000000000000000000000000";
        private string _testUserEmail = "email1@example.com";

        [Fact]
        public void GenericTest()
        {
            AclBuilder acl = new AclBuilder();
            Assert.Null(acl.Build());

            acl.AddEmail(_testUserEmail);
            acl.AddEmail("other@email.com");
            Assert.Equal($"emailAddress=\"{_testUserEmail}\",emailAddress=\"other@email.com\"", acl.Build());

            acl.AddUserId(_testUserId);
            Assert.Equal($"emailAddress=\"{_testUserEmail}\",emailAddress=\"other@email.com\",id=\"{_testUserId}\"", acl.Build());

            acl.AddGroup(PredefinedGroup.AllUsers);
            Assert.Equal($"emailAddress=\"{_testUserEmail}\",emailAddress=\"other@email.com\",id=\"{_testUserId}\",uri=\"http://acs.amazonaws.com/groups/global/AllUsers\"", acl.Build());
        }

        [Fact]
        public void TestDuplicate()
        {
            AclBuilder acl = new AclBuilder();
            acl.AddEmail(_testUserEmail);
            acl.AddUserId(_testUserId);
            acl.AddGroup(PredefinedGroup.AuthenticatedUsers);

            Assert.Throws<ArgumentException>(() => acl.AddEmail(_testUserEmail));
            Assert.Throws<ArgumentException>(() => acl.AddUserId(_testUserId));
            Assert.Throws<ArgumentException>(() => acl.AddGroup(PredefinedGroup.AuthenticatedUsers));
        }
    }
}