using CpuScheduler.Models;

namespace CpuScheduler.Schedulers;

public interface IScheduler
{
    public List<SchedulingResult> Schedule(List<ProcessData> processes);
}