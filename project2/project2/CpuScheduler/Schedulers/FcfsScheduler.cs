using CpuScheduler.Models;

namespace CpuScheduler.Schedulers;

/// <summary>
/// FCFS Algorithm: Execute processes in order of arrival time
/// </summary>

public class FcfsScheduler :  IScheduler
{
    // Method to execute processes according to FCFS design
    public List<SchedulingResult> Schedule(List<ProcessData> processes)
    {
        var results = new List<SchedulingResult>();
        
        // Step 1: sort processes by ArrivalTime
        var sortedProcesses = processes.OrderBy(p => p.ArrivalTime).ToList();

        // Step 2: track current time
        var currentTime = 0;

        // Step 3: loop through processes
        foreach (var process in sortedProcesses)
        {
            var startTime = Math.Max(currentTime, process.ArrivalTime);
            var finishTime = startTime + process.BurstTime;
            var waitingTime = startTime - process.ArrivalTime;
            var turnaroundTime = finishTime - process.ArrivalTime;
                
            results.Add(new SchedulingResult
            {
                ProcessId = process.ProcessId,
                ArrivalTime = process.ArrivalTime,
                BurstTime = process.BurstTime,
                StartTime = startTime,
                FinishTime = finishTime,
                WaitingTime = waitingTime,
                TurnaroundTime = turnaroundTime
            });
                
            currentTime = finishTime;
        }
            
        return results;
    }
}