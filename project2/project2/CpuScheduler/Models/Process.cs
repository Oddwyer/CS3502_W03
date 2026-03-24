namespace CpuScheduler.Models;

/// <summary>
/// Data structure for process information used when implementing custom scheduling algorithms.
/// </summary>
public class Process
{
    public string ProcessId { get; set; }
    public int ArrivalTime { get; set; }
    public int BurstTime { get; set; }
    public int Priority { get; set; }


    public Process(string processId, int arrivalTime, int burstTime, int priority)
    {
        ProcessId = processId;
        ArrivalTime = arrivalTime;
        BurstTime = burstTime;
        Priority = priority;
    }
}