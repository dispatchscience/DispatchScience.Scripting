using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dispatch.Scripts
{
    public interface IWorkflowUpdater : IWorkflowReader
    {
        new IList<IWorkflowStepUpdater> Steps { get; }

        public interface IWorkflowStepUpdater : IWorkflowStepReader
        {
            Task SetIsActive(bool isActive);
        }
    }

    public interface IWorkflowReader
    {
        string Id { get; }
        IList<IWorkflowStepReader> Steps { get; }

        public interface IWorkflowStepReader
        {
            string Id { get; }
            string TitlePrimary { get; }
            string? TitleSecondary { get; }
            bool CanSkip { get; }
            WorkflowStepType StepType { get; }
            bool IsActive { get; }
        }
    }
}