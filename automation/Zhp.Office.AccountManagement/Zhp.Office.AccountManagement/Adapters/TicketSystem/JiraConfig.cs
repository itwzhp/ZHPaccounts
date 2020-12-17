namespace Zhp.Office.AccountManagement.Adapters.TicketSystem
{
    public class JiraConfig
    {
        public string JiraUri { get; private set; } = string.Empty;

        public string User { get; private set; } = string.Empty;
        public string Password { get; private set; } = string.Empty;

        public int JiraQueryBatchSize { get; private set; } = 20;

        public QueriesType Queries { get; } = new QueriesType();

        public WorkflowsType Workflows { get; } = new WorkflowsType();

        public class QueriesType
        {
            public string ApprovedActivationsTicket { get; private set; } = string.Empty;
        }

        public class WorkflowsType
        {
            public int MarkAsDone { get; private set; }

            public int MarkForManualReview { get; private set; }
        }
    }
}
