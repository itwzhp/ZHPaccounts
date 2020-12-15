using Divergic.Logging.Xunit;
using FluentAssertions;
using NSubstitute;
using NSubstitute.Core;
using System;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Zhp.Office.AccountManagement.Domain.Ports;
using Zhp.Office.AccountManagement.Domain.Services;
using Zhp.Office.AccountManagement.Model;

namespace Zhp.Office.AccountManagement.Tests.Domain.Services
{
    public class AccountsCreatingServiceTests
    {
        private readonly IAccountManager accountManager = Substitute.For<IAccountManager>();
        private readonly ITicketRepository ticketRepository = Substitute.For<ITicketRepository>();
        private readonly IPasswordGenerator passwordGenerator = Substitute.For<IPasswordGenerator>();
        private readonly IMailAddressGenerator mailAddressGenerator = Substitute.For<IMailAddressGenerator>();
        private readonly ICacheLogger<AccountsCreatingService> logger;

        private readonly AccountsCreatingService subject;

        private readonly ActivationRequest[] testRequests =
        {
            new ActivationRequest
            {
                Id = "123",
                FirstName = "Tomasz",
                LastName = "Nowak",
                FirstLevelUnit = "Hufiec Sopot",
                SecondLevelUnit = "Chorągiew Gdańska",
                MembershipNumber = "AL123456789"
            },
            new ActivationRequest
            {
                Id = "321",
                FirstName = "Jan",
                LastName = "Malinowski",
                FirstLevelUnit = "Hufiec Tczew",
                SecondLevelUnit = "Chorągiew Gdańska",
                MembershipNumber = "AL987654321"
            }
        };

        public AccountsCreatingServiceTests(ITestOutputHelper outputHelper)
        {
            logger = outputHelper.BuildLoggerFor<AccountsCreatingService>();
            passwordGenerator.GeneratePassword().Returns("czuwaj!.8");
            mailAddressGenerator.GetPossibleAddressesForUser(null!, null!)
                .ReturnsForAnyArgs(new[]
                {
                    new MailAddress("test@example.com"),
                    new MailAddress("test2@example.com"),
                    new MailAddress("test3@example.com"),
                    new MailAddress("test4@example.com"),
                });
            accountManager.TryAddUser(null!, null!, null!, default).ReturnsForAnyArgs(true);

            subject = new AccountsCreatingService(
                accountManager,
                ticketRepository,
                logger,
                passwordGenerator,
                mailAddressGenerator);
        }

        [Fact]
        public async Task SimpleCase_AllAccountsCreated_AllTicketsClosed()
        {
            ticketRepository.GetApprovedActivationRequests(default)
                .ReturnsForAnyArgs(testRequests);

            await subject.CreateAccounts(CancellationToken.None);

            foreach(var ticket in testRequests)
            {
                await accountManager.Received().TryAddUser(Arg.Is<ActivationRequest>(r => r.Id == ticket.Id), new MailAddress("test@example.com"), "czuwaj!.8", CancellationToken.None);
                await ticketRepository.Received().MarkAsDone(ticket.Id, Arg.Any<string?>(), Arg.Any<CancellationToken>());
            }
        }

        [Fact]
        public async Task FirstMailAlreadyTaken_AccountsCreated_AllTicketsClosed()
        {
            ticketRepository.GetApprovedActivationRequests(default)
                .ReturnsForAnyArgs(testRequests);
            accountManager.TryAddUser(Arg.Any<ActivationRequest>(), new MailAddress("test@example.com"), Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(false);

            await subject.CreateAccounts(CancellationToken.None);

            foreach (var ticket in testRequests)
            {
                await accountManager.Received().TryAddUser(Arg.Is<ActivationRequest>(r => r.Id == ticket.Id), new MailAddress("test2@example.com"), "czuwaj!.8", CancellationToken.None);
                await ticketRepository.Received().MarkAsDone(ticket.Id, Arg.Any<string?>(), Arg.Any<CancellationToken>());
            }
        }

        [Fact]
        public async Task CreatingFailsWithException_AllTicketsMarkedForManualReview()
        {
            ticketRepository.GetApprovedActivationRequests(default)
                .ReturnsForAnyArgs(testRequests);
            accountManager.TryAddUser(null!, null!, null!, default)
                .ReturnsForAnyArgs((Func<CallInfo, bool>)(x => throw new Exception("Some message")));

            await subject.CreateAccounts(CancellationToken.None);

            foreach (var ticket in testRequests)
            {
                logger.Entries.Should().Contain(e => e.Exception != null && e.Exception.Message.Contains("Some message"));
                await ticketRepository.Received().MarkForManualReview(ticket.Id, Arg.Is<string>(s => s.Contains("Some message")), Arg.Any<CancellationToken>());
            }
        }

        [Fact]
        public async Task DupplicateEntries_UniqueAccountsCreated_AllTicketsClosedOrMarkedForManualReview()
        {
            Assert.True(false, "TODO");
        }
    }
}
