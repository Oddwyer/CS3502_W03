namespace CpuScheduler.Models;

/// <summary>
/// Context object to store results from metrics calculator for console
/// display.
/// </summary>

public class PerformanceMetrics
{
    public double AverageWaitingTime { get; set; }
    public double AverageTurnaroundTime { get; set; }
    public double CpuUtilization { get; set; }
    public double Throughput { get; set; }
    public double ResponseTime { get; set; }
}