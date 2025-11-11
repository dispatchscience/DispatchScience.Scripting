using System;
using System.Collections.Generic;
using System.Linq;

namespace Dispatch.Scripts
{
    public class ServiceLevelInfo
    {
        public TimeSpan OperationsStartTime { get; set; }
        public TimeSpan OperationsEndTime { get; set; }

        public IList<ServiceLevelDayOfOperation> DaysOfOperation { get; set; } = new List<ServiceLevelDayOfOperation>();

        public bool IsArchived { get; set; }

        public (bool IsOperationDay, TimeSpan? StartTime, TimeSpan? EndTime) GetOperationHours(DayOfWeek dayOfWeek)
        {
            var dayOfOperation = DaysOfOperation.FirstOrDefault(x => x.DayOfOperation == dayOfWeek);
            if (dayOfOperation is null)
            {
                return (false, null, null);
            }

            return (true, dayOfOperation.OverrideOperationHours ? dayOfOperation.OperationsStartTime : OperationsStartTime, dayOfOperation.OverrideOperationHours ? dayOfOperation.OperationsEndTime : OperationsEndTime);
        }
    }

    public class ServiceLevelDayOfOperation
    {
        public DayOfWeek DayOfOperation { get; set; }
        public bool OverrideOperationHours { get; set; }
        public TimeSpan? OperationsStartTime { get; set; }
        public TimeSpan? OperationsEndTime { get; set; }
    }
}
