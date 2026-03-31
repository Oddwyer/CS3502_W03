using CpuScheduler.Models;

namespace CpuScheduler.Schedulers;

/// <summary>
/// Interface for CPU scheduling algorithms
/// </summary>
public interface IScheduler
{
    
    // Computes a scheduling order for the given list of processes.
    public List<SchedulingResult> Schedule(List<ProcessData> processes);
}