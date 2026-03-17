using CpuScheduler.Models;

namespace CpuScheduler.Services;

/// <summary>
/// Calculates data for performance metrics context object properties.
/// Data derived from respective algorithm's scheduling results.
/// </summary>
public class MetricsCalculator
{
    public PerformanceMetrics Calculate(List<SchedulingResult> schedulingResults)
    {
        // Add summary statistics
        
        // Error handling: No results produced.
        if (schedulingResults == null || schedulingResults.Count == 0)
        {
            return new PerformanceMetrics();
        }
        // Required metrics for your project report:
        
        // 1. Average Waiting Time (AWT) - sum of all waiting times / number of processes
        var avgWaiting = schedulingResults.Average(r => r.WaitingTime);
        
        // 2. Average Turnaround Time (ATT) - sum of all turnaround times / number of processes  
        var avgTurnaround = schedulingResults.Average(r => r.TurnaroundTime);

        var totalTime = (double) schedulingResults.Max(r => r.FinishTime);

        // Error handling: Addresses edge case where process has no arrival time or burst time.
        if (totalTime == 0)
        {
            return new PerformanceMetrics();
        }
        
        // 3. CPU Utilization (%) - (total burst time / total time) * 100
        var cpuUtilization = (schedulingResults.Sum(r => r.BurstTime) / totalTime) * 100;
        
        // 4. Throughput (processes/second) - number of processes / total time
        var throughPut = schedulingResults.Count / totalTime;
        
        // 5. Response Time (RT) [Optional] - average time from arrival to first execution
        var responseTime = schedulingResults.Average(r => r.StartTime - r.ArrivalTime);
        
        return new PerformanceMetrics
        {
            AverageWaitingTime = avgWaiting, 
            AverageTurnaroundTime = avgTurnaround,
            CpuUtilization = cpuUtilization,
            Throughput = throughPut,
            ResponseTime = responseTime
        };
    }
    
}