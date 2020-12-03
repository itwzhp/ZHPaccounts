using System;

namespace Zhp.Office.AccountManagement.Infrastructure
{
    internal class FunctionConfig
    {
        public JiraConfig Jira { get; private set; } = new JiraConfig();
        
        internal class JiraConfig
        {
            public string JiraUri { get; private set; } = string.Empty;
            public string ConsumerKey { get; private set; } = string.Empty;
            public string ConsumerSecret { get; private set; } = string.Empty;
            public string OAuthAccessToken { get; private set; } = string.Empty;
            public string OAuthTokenSecret { get; private set; } = string.Empty;
        }
    }
}
