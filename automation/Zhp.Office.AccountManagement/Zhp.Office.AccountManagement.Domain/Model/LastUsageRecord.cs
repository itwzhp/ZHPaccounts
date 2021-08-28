using System;
using System.Net.Mail;

namespace Zhp.Office.AccountManagement.Domain.Model
{
    public record LastUsageRecord(MailAddress Username, DateTime? LastUsage);
}
