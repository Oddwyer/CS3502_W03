using CpuScheduler.Models;

namespace CpuScheduler.Schedulers;

/// <summary>
/// Round Robin Algorithm: Each process gets a time quantum, then cycles to next process.
/// </summary>
public class RRScheduler : IScheduler
{
    private int _quantumTime = 4;
    // Method to execute processes according to Round Robin design
    public RRScheduler(int quantumTime)
    {
        _quantumTime = quantumTime;
    }
    
    // Method to include quantumTime to execute processes according to Round Robin design
    public List<SchedulingResult> Schedule(List<ProcessData> processes)
        {
            var results = new List<SchedulingResult>();
            var currentTime = 0;
            var processQueue = new Queue<ProcessData>();
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
            
            // Add processes that arrive at time 0
            foreach (var process in processes.Where(p => p.ArrivalTime <= currentTime).OrderBy(p => p.ArrivalTime))
            {
                processQueue.Enqueue(process);
            }
            
            var processesNotInQueue = processes.Where(p => p.ArrivalTime > currentTime).OrderBy(p => p.ArrivalTime).ToList();
            
            // Continue so long as any processes remain in either the ready or waiting queues
            while (processQueue.Count > 0 || processesNotInQueue.Count > 0)
            {
                // Add any processes that have now arrived
                while (processesNotInQueue.Count > 0 && processesNotInQueue[0].ArrivalTime <= currentTime)
                {
                    processQueue.Enqueue(processesNotInQueue[0]);
                    processesNotInQueue.RemoveAt(0);
                }
                
                if (processQueue.Count == 0)
                {
                    // No processes in queue, jump to next arrival
                    currentTime = processesNotInQueue[0].ArrivalTime;
                    continue;
                }
                
                var currentProcess = processQueue.Dequeue();
                var result = processResults[currentProcess.ProcessId];
                
                // Set start time if this is the first execution
                if (result.StartTime == -1)
                {
                    result.StartTime = currentTime;
                }
                
                // Execute for quantum time or remaining burst time, whichever is smaller
                var executionTime = Math.Min(_quantumTime, remainingBurstTimes[currentProcess.ProcessId]);
                currentTime += executionTime;
                remainingBurstTimes[currentProcess.ProcessId] -= executionTime;
                
                // Add any processes that arrived during this execution
                while (processesNotInQueue.Count > 0 && processesNotInQueue[0].ArrivalTime <= currentTime)
                {
                    processQueue.Enqueue(processesNotInQueue[0]);
                    processesNotInQueue.RemoveAt(0);
                }
                
                // Check if process is completed
                if (remainingBurstTimes[currentProcess.ProcessId] == 0)
                {
                    result.FinishTime = currentTime;
                    result.TurnaroundTime = result.FinishTime - result.ArrivalTime;
                    result.WaitingTime = result.TurnaroundTime - result.BurstTime;
                }
                else
                {
                    // Process not completed, add back to queue
                    processQueue.Enqueue(currentProcess);
                }
            }
            
            return processResults.Values.OrderBy(r => r.StartTime).ToList();
        }

}