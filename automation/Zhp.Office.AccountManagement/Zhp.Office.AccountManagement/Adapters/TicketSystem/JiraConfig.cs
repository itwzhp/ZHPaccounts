namespace Zhp.Office.AccountManagement.Adapters.TicketSystem
{
    public class JiraConfig
    {
        public string JiraUri { get; private set; } = string.Empty;

        public string User { get; private set; } = string.Empty;
        public string Password { get; private set; } = string.Empty;

        public int? JiraQueryBatchSize { get; private set; }

        public QueriesType Queries { get; private set; } = new QueriesType();

        public class QueriesType
        {
            public string ApprovedActivationsTicket { get; private set; } = string.Empty;
        }
    }
}
