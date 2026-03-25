using CpuScheduler.Models;
using CpuScheduler.Schedulers;
using CpuScheduler.Services;


Run("FCFS", new FcfsScheduler(), WorkloadFactory.CreateCpuBoundWorkload());
Run("SJF", new SjfScheduler(), WorkloadFactory.CreateCpuBoundWorkload());
Run("RR", new RRScheduler(4), WorkloadFactory.CreateCpuBoundWorkload());
Run("Priority", new PriorityScheduler(), WorkloadFactory.CreateCpuBoundWorkload());
Run("SRTF", new SrtfScheduler(), WorkloadFactory.CreateCpuBoundWorkload());
Run("HRRN", new HrrnScheduler(), WorkloadFactory.CreateCpuBoundWorkload());



//==========================Helper Method==============================
static void Run(string schedulerName, IScheduler scheduler, List<ProcessData> processes)
{
    string name =  schedulerName;
    List<SchedulingResult> results;
    var schedulerType = scheduler;
    results = schedulerType.Schedule(processes);
    var calculator = new MetricsCalculator();
    var metrics = calculator.Calculate(results);

    Console.WriteLine($"\n===================== {name} Scheduler =========================");

    foreach (var result in results)
    {
        Console.WriteLine(
            $"{result.ProcessId,-5} | Start: {result.StartTime,-3} | " +
            $"Finish: {result.FinishTime,-3} | Wait: {result.WaitingTime,-3} | " +
            $"Turnaround: {result.TurnaroundTime,-3}");
    }

    Console.WriteLine();
    Console.WriteLine($"=== {name} Performance Metrics ===");
    Console.WriteLine($"Average Waiting Time: {metrics.AverageWaitingTime:F2}");
    Console.WriteLine($"Average Turnaround Time: {metrics.AverageTurnaroundTime:F2}");
    Console.WriteLine($"CPU Utilization: {metrics.CpuUtilization:F1}%");
    Console.WriteLine($"Throughput: {metrics.Throughput:F3}");
    Console.WriteLine($"Response Time: {metrics.ResponseTime:F2}");
}

// TODO: STUDENTS - Add CSV export functionality for results data
// Create a "Export Results" button in the results panel to save:
// - Individual process results (what's shown in listView1)
// - Performance metrics summary for each algorithm tested
// Reference the SaveData_Click() method above to learn CSV file handling
// This will help you create tables/charts for your project report
