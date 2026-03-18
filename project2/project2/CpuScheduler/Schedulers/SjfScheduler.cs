using CpuScheduler.Models;

namespace CpuScheduler.Schedulers;

/// <summary>
/// SJF Algorithm: Shortest Job First - selects process with minimum burst time.
/// </summary>
public class SjfScheduler
{
    // Method to execute processes according to SJF design.
    public List<SchedulingResult> Schedule(List<Process> processes)
    {
        var results = new List<SchedulingResult>();
        var currentTime = 0;
        var remainingProcesses = processes.ToList();

        while (remainingProcesses.Count > 0)
        {
            // Get processes that have arrived by current time
            var availableProcesses = remainingProcesses.Where(p => p.ArrivalTime <= currentTime).ToList();

            if (availableProcesses.Count == 0)
            {
                // No process has arrived yet, jump to next arrival time
                currentTime = remainingProcesses.Min(p => p.ArrivalTime);
                continue;
            }

            // Select process with shortest burst time
            var nextProcess = availableProcesses.OrderBy(p => p.BurstTime).ThenBy(p => p.ArrivalTime).First();

            var startTime = Math.Max(currentTime, nextProcess.ArrivalTime);
            var finishTime = startTime + nextProcess.BurstTime;
            var waitingTime = startTime - nextProcess.ArrivalTime;
            var turnaroundTime = finishTime - nextProcess.ArrivalTime;

            results.Add(new SchedulingResult
            {
                ProcessId = nextProcess.ProcessId,
                ArrivalTime = nextProcess.ArrivalTime,
                BurstTime = nextProcess.BurstTime,
                StartTime = startTime,
                FinishTime = finishTime,
                WaitingTime = waitingTime,
                TurnaroundTime = turnaroundTime
            });

            currentTime = finishTime;
            remainingProcesses.Remove(nextProcess);
        }

        return results.OrderBy(r => r.StartTime).ToList();
    }
}