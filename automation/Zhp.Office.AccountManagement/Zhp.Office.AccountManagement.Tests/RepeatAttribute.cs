using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit.Sdk;

namespace Zhp.Office.AccountManagement.Tests
{
    public sealed class RepeatAttribute : DataAttribute
    {
        private readonly int count;

        public RepeatAttribute(int count)
        {
            if (count < 1)
                throw new System.ArgumentOutOfRangeException(
                    nameof(count),
                    count,
                    "Repeat count must be greater than 0.");
            this.count = count;
        }

        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
            => Enumerable.Range(1, this.count).Select(i => new object[] { i });
    }
}
