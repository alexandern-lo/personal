using System;
using System.Collections.Generic;

namespace Avend.ApiTests.DataSamples
{
    public class TestUser
    {
        public static readonly List<TestUser> All;

        public Guid Uid { get; set; }
        public string Token { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public Guid SubscriptionUid { get; set; }

        static TestUser()
        {
            All = new List<TestUser>();

            AliceTester = new TestUser
            {
                Uid = new Guid("6f47b5b0-7633-4ce5-834b-d9870504a47c"),
                Token = "Alice Tester",
                FirstName = "Alice",
                LastName = "Tester",
                Email = "apps@studiomobile.ru"
            };

            NoSubscription = new TestUser
            {
                Uid = new Guid("0b25f007-0318-492c-bc63-63b9feec0939"),
                Token = "No Subscription",
                FirstName = "No",
                LastName = "Subscription",
                Email = "nosubscription@avenddev.onmicrosoft.com"
            };

            BobTester = new TestUser
            {
                Uid = new Guid("d25e1ba1-48ae-4a3d-a1a2-d61f84d2e895"),
                Token = "Bob Tester",
                FirstName = "Bob",
                LastName = "Tester",
                Email = "bob@avenddev.onmicrosoft.com"
            };

            CecileTester = new TestUser
            {
                Uid = new Guid("f0e3d3cd-2c78-4732-824b-8b065cd2f2c2"),
                Token = "Cecile Tester",
                FirstName = "Cecile",
                LastName = "Tester",
                Email = "cecile@avenddev.onmicrosoft.com"
            };

            MikeTester = new TestUser
            {
                Uid = new Guid("d0ce017b-e746-418a-86d7-8118a44f59b2"),
                Token = "Mike Tester",
                FirstName = "Mike",
                LastName = "Tester",
                Email = "mike@avenddev.onmicrosoft.com"
            };

            JohnTester = new TestUser
            {
                Uid = new Guid("8365d134-5aa6-43af-ae20-2fecfff9b658"),
                Token = "John Tester",
                FirstName = "John",
                LastName = "Tester",
                Email = "john@avenddev.onmicrosoft.com"
            };

            AlexTester = new TestUser
            {
                Uid = new Guid("c569b970-4139-4a07-973f-0d7f748a3db9"),
                Token = "Alex Tester",
                FirstName = "Alex",
                LastName = "Tester",
                Email = "alex@avenddev.onmicrosoft.com"
            };

            MarcTester = new TestUser
            {
                Uid = new Guid("ff58faec-35f0-44e3-8f56-b177eaab348d"),
                Token = "Marc Tester",
                FirstName = "Marc",
                LastName = "Tester",
                Email = "marc@avenddev.onmicrosoft.com"
            };
        }

        private static int testUserNr = 1;

        public static TestUser MakeSample(Action<TestUser> postProcessor = null)
        {
            var user = new TestUser()
            {
                Uid = Guid.NewGuid(),
                Token = "Tester " + testUserNr,
                FirstName = "Tester " + testUserNr,
                LastName = "TT " + testUserNr,
                Email = $"tester{testUserNr}@avenddev.onmicrosoft.com"
            };
            postProcessor?.Invoke(user);
            testUserNr++;
            return user;
        }

        public static readonly TestUser NoSubscription;
        public static readonly TestUser AliceTester;

        //Alex is super admin
        public static readonly TestUser AlexTester;

        //Recurly account - d25e1ba1-48ae-4a3d-a1a2-d61f84d2e895
        //Subscription id (corporate 1000) - 3b20fdd8e32cc0409e287c4864aa6954
        //Bob is admin, owns subscription
        public static readonly TestUser BobTester;
        public static readonly TestUser CecileTester;
        public static readonly TestUser JohnTester;
        public static readonly TestUser MikeTester;

        //Recurly account - ff58faec-35f0-44e3-8f56-b177eaab348d
        //Subscription id (individual) - 3b6d12a93f0c3cad8b897140fb89e18c
        //Marc is admin, owns subscription (separate from Bob)
        public static readonly TestUser MarcTester;

        private static TestUser AddNewUser(TestUser user)
        {
            All.Add(user);
            return user;
        }
    }
}