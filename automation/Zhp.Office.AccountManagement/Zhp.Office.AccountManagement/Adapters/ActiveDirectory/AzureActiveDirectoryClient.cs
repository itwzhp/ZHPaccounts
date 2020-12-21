﻿using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using System;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Zhp.Office.AccountManagement.Domain.Ports;
using Zhp.Office.AccountManagement.Infrastructure;
using Zhp.Office.AccountManagement.Model;

namespace Zhp.Office.AccountManagement.Adapters.ActiveDirectory
{
    public class AzureActiveDirectoryClient : IAccountManager
    {
        private readonly IGraphServiceClient client;
        private readonly ILogger<AzureActiveDirectoryClient> logger;
        private readonly bool enableChanges;
        private readonly ActiveDirectoryConfig activeDirectoryConfig;

        public AzureActiveDirectoryClient(IGraphServiceClient client, FunctionConfig config, ILogger<AzureActiveDirectoryClient> logger)
        {
            this.client = client;
            this.logger = logger;
            this.enableChanges = config.EnableChanges;
            this.activeDirectoryConfig = config.ActiveDirectory;
        }

        public async ValueTask<bool> TryAddUser(ActivationRequest request, MailAddress email, string password, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var existingUser = await client.Users.Request()
                .Select(nameof(User.UserPrincipalName))
                .Filter($"{nameof(User.UserPrincipalName)} eq '{email}'")
                .GetAsync(token);

            if (existingUser.Count > 0)
                return false;

            if (!enableChanges)
            {
                logger.LogInformation($"Sandbox graphAPI client: adding user {email}");
                return true;
            }

            var user = new User
            {
                Mail = email.ToString(),
                UserPrincipalName = email.ToString(),
                MailNickname = email.User,
                PasswordProfile = new PasswordProfile
                {
                    Password = password,
                    ForceChangePasswordNextSignIn = true
                },

                GivenName = request.FirstName,
                Surname = request.LastName,
                DisplayName = $"{request.FirstName} {request.LastName}",
                JobTitle = request.MembershipNumber,
                Department = request.FirstLevelUnit,
                OfficeLocation = request.SecondLevelUnit,

                AccountEnabled = true,
                UsageLocation = "PL",
            };

            var licenses = new[] { new AssignedLicense { SkuId = new Guid(activeDirectoryConfig.DefaultLicenseSku) } };

            token.ThrowIfCancellationRequested();

            user = await client.Users.Request().AddAsync(user);
            await client.Users[user.Id].AssignLicense(licenses, Enumerable.Empty<Guid>()).Request().PostAsync();
            return true;
        }
    }
}