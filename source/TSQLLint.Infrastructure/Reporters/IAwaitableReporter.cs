using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TSQLLint.Common;

namespace TSQLLint.Infrastructure.Reporters
{
    public interface  IAwaitableReporter : IReporter
    {
        Task ReportingTask { get; }
    }
}
