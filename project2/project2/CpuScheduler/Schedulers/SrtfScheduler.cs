using CpuScheduler.Models;

namespace CpuScheduler.Schedulers;

/// <summary>
/// Shortest Remaining Time First Algorithm: Each process is checked at each time unit to determine current shortest job. Any process
/// with a longer remaining time is interrupted and the process with shortest job is permitted to run.
/// </summary>
public class SrtfScheduler
{
    // Method to execute processes according to SRTF design.
    public List<SchedulingResult> Schedule(List<Process> processes)
    {
        var results = new List<SchedulingResult>();
        var currentTime = 0;
        var processResults = new Dictionary<string, SchedulingResult>();
        var remainingBurstTimes = new Dictionary<string, int>();

        // Initialize remaining burst times and results
        foreach (var process in processes)
        {
            remainingBurstTimes[process.ProcessId] = process.BurstTime;
            processResults[process.ProcessId] = new SchedulingResult
            {
                ProcessId = process.ProcessId,
                ArrivalTime = process.ArrivalTime,
                BurstTime = process.BurstTime,
                StartTime = -1, // Will be set on first execution
                FinishTime = 0,
                WaitingTime = 0,
                TurnaroundTime = 0
            };
        }
        
        // Continue so long as any processes remain unfinished
        while (remainingBurstTimes.Any(p=> p.Value > 0))
        {
            // Assign current process to the process with the shortest time. 
            var shortestProcess = processes
                .Where(p => p.ArrivalTime <= currentTime && remainingBurstTimes[p.ProcessId] > 0)
                .OrderBy(p => remainingBurstTimes[p.ProcessId])
                .ThenBy(p => p.ArrivalTime).FirstOrDefault();
            var currentProcess = shortestProcess;
           
            // Error handling: there may be no process available at the current time
            if(currentProcess ==null){
                currentTime++;
                continue;
            }
            
            var result = processResults[currentProcess.ProcessId];

            // Set start time if this is the first execution
            if (result.StartTime == -1)
            {
                result.StartTime = currentTime;
            }

            // Execute for one time unit
            remainingBurstTimes[currentProcess.ProcessId]--;
            currentTime++;
            
            // Check if process is completed
            if (remainingBurstTimes[currentProcess.ProcessId] == 0)
            {
                result.FinishTime = currentTime;
                result.TurnaroundTime = result.FinishTime - result.ArrivalTime;
                result.WaitingTime = result.TurnaroundTime - result.BurstTime;
            }
        }

        return processResults.Values.OrderBy(r => r.StartTime).ToList();
    }
}