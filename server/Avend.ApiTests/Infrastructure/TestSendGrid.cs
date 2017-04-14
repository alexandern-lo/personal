using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Avend.API.Infrastructure;
using Avend.API.Model;

namespace Avend.ApiTests.Infrastructure
{
    public class TestInviteMessage
    {
        public string Message { get; set; }
        public string Subject { get; set; }
        public string InviteCode { get; set; }
        public string Email { get; set; }
    }

    public class TestSendGrid : ISendGrid
    {
        public Task<bool> Send(string message, string subject, Func<string, string> linkGenerator, IEnumerable<SubscriptionInvite> invites)
        {
            if (EmulateError) return Task.FromResult(false);

            Messages.AddRange(invites.Select(x => new TestInviteMessage
            {
                Email = x.Email,
                Message = message,
                Subject = subject,
                InviteCode = x.InviteCode
            }));

            return Task.FromResult(true);
        }

        public List<TestInviteMessage> Messages { get; } = new List<TestInviteMessage>();

        public bool EmulateError { get; set; } = false;

        public TestInviteMessage MessageFor(string email)
        {
            return Messages.First(x => x.Email == email);
        }
    }
}