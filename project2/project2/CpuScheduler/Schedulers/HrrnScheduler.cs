using CpuScheduler.Models;

namespace CpuScheduler.Schedulers;

/// <summary>
/// Highest Response Ratio Next Algorithm: Each process is weighed by response ratio (waiting time + burst time)/ burst time
/// This is done to balance short jobs with fairness (considers waiting time)
/// If more than one process has the exact same ratio, then priority is given to the process with the earliest arrival
/// </summary>


public class HrrnScheduler: IScheduler
{
    // Method to execute processes according to HRRN design
    public List<SchedulingResult> Schedule(List<ProcessData> processes)
    {
        var results = new List<SchedulingResult>();
        var currentTime = 0;
        var processResults = new Dictionary<string, SchedulingResult>();
        var currentProcesses = new List<ProcessData>(processes);

        // Initialize remaining burst times and results
        foreach (var process in processes)
        {
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
        while (currentProcesses.Count > 0)
        {
            // Assign current process to the process with the highest response ratio
            // If ratios are the same for one than one process, earliest arrival is tie-breaker
            var selectedProcess = currentProcesses
                .Where(p => p.ArrivalTime <= currentTime)
                .OrderByDescending(p =>
                    {
                        var waitingTime = currentTime - p.ArrivalTime;
                        return (waitingTime + p.BurstTime) / (double) p.BurstTime;
                    })
                    .ThenBy(p => p.ArrivalTime).FirstOrDefault();
            var currentProcess = selectedProcess;
           
            // Error handling: there may be no process available at the current time
            if(currentProcess == null){
                currentTime++;
                continue;
            }
            
            var result = processResults[currentProcess.ProcessId];

            // Set start time if this is the first execution
            if (result.StartTime == -1)
            {
                result.StartTime = currentTime;
            }

            // Update current time (old current + burst time)
            currentTime += currentProcess.BurstTime;
            
            // Check if process is completed
             result.FinishTime = currentTime;
             result.TurnaroundTime = result.FinishTime - result.ArrivalTime;
             result.WaitingTime = result.TurnaroundTime - result.BurstTime;
             
             // As is non-preemptive (runs entire job), remove from current processes
             currentProcesses.Remove(currentProcess);
            
        }

        return processResults.Values.OrderBy(r => r.StartTime).ToList();
    }
}