namespace CpuScheduler.Models;

/// <summary>
/// Data structure for algorithm results used to store and display scheduling algorithm outcomes.
/// </summary>

public class SchedulingResult
{
    public string ProcessId { get; set; }
    public int ArrivalTime { get; set; }
    public int BurstTime { get; set; }
    public int StartTime { get; set; }
    public int FinishTime { get; set; }
    public int WaitingTime { get; set; }
    public int TurnaroundTime { get; set; }
}