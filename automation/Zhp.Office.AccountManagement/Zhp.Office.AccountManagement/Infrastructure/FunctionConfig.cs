using Zhp.Office.AccountManagement.Adapters.ActiveDirectory;
using Zhp.Office.AccountManagement.Adapters.TicketSystem;

namespace Zhp.Office.AccountManagement.Infrastructure
{
    public class FunctionConfig
    {
        public JiraConfig Jira { get; private set; } = new JiraConfig();

        public ActiveDirectoryConfig ActiveDirectory { get; private set; } = new ActiveDirectoryConfig();

        public bool EnableChanges { get; private set; }

        public uint OldAccountThresholdInDays { get; private set; }
    }
}
