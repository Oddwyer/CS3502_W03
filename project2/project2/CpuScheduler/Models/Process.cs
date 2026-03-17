namespace CpuScheduler.Models;

/// <summary>
/// Data structure for process information used when implementing custom scheduling algorithms.
/// </summary>
public class Process
{
    public string ProcessId { get; set; }
    public int BurstTime { get; set; }
    public int Priority { get; set; }
    public int ArrivalTime { get; set; }

    public Process(string processId, int burstTime, int priority, int arrivalTime)
    {
        ProcessId = processId;
        BurstTime = burstTime;
        Priority = priority;
        ArrivalTime = arrivalTime;
    }
}