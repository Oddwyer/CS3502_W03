using CpuScheduler.Models;
using CpuScheduler.Schedulers;
using CpuScheduler.Services;

//==================================FCFS Scheduler====================================

var processes = new List<ProcessData>
{
    new("P1", 0, 4, 1),
    new("P2", 1, 3, 2),
    new("P3", 2, 2, 3)
};

var fcfsScheduler = new FcfsScheduler();
var results = fcfsScheduler.Schedule(processes);
var calculator = new MetricsCalculator();
var metrics = calculator.Calculate(results);

Console.WriteLine("\n======================= FCFS Results =========================");

foreach (var result in results)
{
    Console.WriteLine(
        $"{result.ProcessId,-5} | Start: {result.StartTime,-3} | " +
        $"Finish: {result.FinishTime,-3} | Wait: {result.WaitingTime,-3} | " +
        $"Turnaround: {result.TurnaroundTime,-3}");
}

Console.WriteLine();
Console.WriteLine("=== FCFS Performance Metrics ===");
Console.WriteLine($"Average Waiting Time: {metrics.AverageWaitingTime:F2}");
Console.WriteLine($"Average Turnaround Time: {metrics.AverageTurnaroundTime:F2}");
Console.WriteLine($"CPU Utilization: {metrics.CpuUtilization:F1}%");
Console.WriteLine($"Throughput: {metrics.Throughput:F3}");
Console.WriteLine($"Response Time: {metrics.ResponseTime:F2}");


//==================================SJF Scheduler====================================

processes = new List<ProcessData>
{
    new("P1", 0, 8, 1),
    new("P2", 1, 4, 1),
    new("P3", 2, 2, 1)
};

var sjfScheduler = new SjfScheduler();
results = sjfScheduler.Schedule(processes);
calculator = new MetricsCalculator(); 
metrics = calculator.Calculate(results);

Console.WriteLine("\n======================= SJF Results ==========================");

foreach (var result in results)
{
    Console.WriteLine(
        $"{result.ProcessId,-5} | Start: {result.StartTime,-3} | " +
        $"Finish: {result.FinishTime,-3} | Wait: {result.WaitingTime,-3} | " +
        $"Turnaround: {result.TurnaroundTime,-3}");
}

Console.WriteLine();
Console.WriteLine("=== SJF Performance Metrics ===");
Console.WriteLine($"Average Waiting Time: {metrics.AverageWaitingTime:F2}");
Console.WriteLine($"Average Turnaround Time: {metrics.AverageTurnaroundTime:F2}");
Console.WriteLine($"CPU Utilization: {metrics.CpuUtilization:F1}%");
Console.WriteLine($"Throughput: {metrics.Throughput:F3}");
Console.WriteLine($"Response Time: {metrics.ResponseTime:F2}");


//==================================Priority Scheduler====================================

processes = new List<ProcessData>
{
    new("P1", 0, 5, 2),
    new("P2", 0, 3, 1),
    new("P3", 0, 4, 3)
};

var priorityScheduler = new PriorityScheduler();
results = priorityScheduler.Schedule(processes);
calculator = new MetricsCalculator(); 
metrics = calculator.Calculate(results);

Console.WriteLine("\n===== Priority Results (Higher Number = Higher Priority) =====");

foreach (var result in results)
{
    Console.WriteLine(
        $"{result.ProcessId,-5} | Start: {result.StartTime,-3} | " +
        $"Finish: {result.FinishTime,-3} | Wait: {result.WaitingTime,-3} | " +
        $"Turnaround: {result.TurnaroundTime,-3}");
}

Console.WriteLine();
Console.WriteLine("=== Priority Performance Metrics ===");
Console.WriteLine($"Average Waiting Time: {metrics.AverageWaitingTime:F2}");
Console.WriteLine($"Average Turnaround Time: {metrics.AverageTurnaroundTime:F2}");
Console.WriteLine($"CPU Utilization: {metrics.CpuUtilization:F1}%");
Console.WriteLine($"Throughput: {metrics.Throughput:F3}");
Console.WriteLine($"Response Time: {metrics.ResponseTime:F2}");


//==================================RR Scheduler====================================

processes = new List<ProcessData>
{
    new("P1", 3, 2, 2),
    new("P2", 5, 1, 1),
    new("P3", 2, 2, 3)
};

var rrScheduler = new RRScheduler();
results = rrScheduler.Schedule(processes, 2);
calculator = new MetricsCalculator(); 
metrics = calculator.Calculate(results);

Console.WriteLine("\n======================= RR Scheduler =========================");

foreach (var result in results)
{
    Console.WriteLine(
        $"{result.ProcessId,-5} | Start: {result.StartTime,-3} | " +
        $"Finish: {result.FinishTime,-3} | Wait: {result.WaitingTime,-3} | " +
        $"Turnaround: {result.TurnaroundTime,-3}");
}

Console.WriteLine();
Console.WriteLine("=== RR Performance Metrics ===");
Console.WriteLine($"Average Waiting Time: {metrics.AverageWaitingTime:F2}");
Console.WriteLine($"Average Turnaround Time: {metrics.AverageTurnaroundTime:F2}");
Console.WriteLine($"CPU Utilization: {metrics.CpuUtilization:F1}%");
Console.WriteLine($"Throughput: {metrics.Throughput:F3}");
Console.WriteLine($"Response Time: {metrics.ResponseTime:F2}");

//==================================SRTF Scheduler====================================

processes = new List<ProcessData>
{
    new("P1", 0, 8, 2),
    new("P2", 1, 4, 1),
    new("P3", 2, 2, 3)
};

var srtfScheduler = new SrtfScheduler();
results = srtfScheduler.Schedule(processes);
calculator = new MetricsCalculator(); 
metrics = calculator.Calculate(results);

Console.WriteLine("\n====================== SRTF Scheduler ========================");

foreach (var result in results)
{
    Console.WriteLine(
        $"{result.ProcessId,-5} | Start: {result.StartTime,-3} | " +
        $"Finish: {result.FinishTime,-3} | Wait: {result.WaitingTime,-3} | " +
        $"Turnaround: {result.TurnaroundTime,-3}");
}

Console.WriteLine();
Console.WriteLine("=== SRTF Performance Metrics ===");
Console.WriteLine($"Average Waiting Time: {metrics.AverageWaitingTime:F2}");
Console.WriteLine($"Average Turnaround Time: {metrics.AverageTurnaroundTime:F2}");
Console.WriteLine($"CPU Utilization: {metrics.CpuUtilization:F1}%");
Console.WriteLine($"Throughput: {metrics.Throughput:F3}");
Console.WriteLine($"Response Time: {metrics.ResponseTime:F2}");


//==================================HRRN Scheduler====================================

processes = new List<ProcessData>
{
    new("P1", 0, 5, 2),
    new("P2", 1, 2, 1),
    new("P3", 2, 1, 3)
};

var hrrnScheduler = new HrrnScheduler();
results = hrrnScheduler.Schedule(processes);
calculator = new MetricsCalculator(); 
metrics = calculator.Calculate(results);

Console.WriteLine("\n===================== HRRN Scheduler =========================");

foreach (var result in results)
{
    Console.WriteLine(
        $"{result.ProcessId,-5} | Start: {result.StartTime,-3} | " +
        $"Finish: {result.FinishTime,-3} | Wait: {result.WaitingTime,-3} | " +
        $"Turnaround: {result.TurnaroundTime,-3}");
}

Console.WriteLine();
Console.WriteLine("=== HRRN Performance Metrics ===");
Console.WriteLine($"Average Waiting Time: {metrics.AverageWaitingTime:F2}");
Console.WriteLine($"Average Turnaround Time: {metrics.AverageTurnaroundTime:F2}");
Console.WriteLine($"CPU Utilization: {metrics.CpuUtilization:F1}%");
Console.WriteLine($"Throughput: {metrics.Throughput:F3}");
Console.WriteLine($"Response Time: {metrics.ResponseTime:F2}");


// TODO: STUDENTS - Add CSV export functionality for results data
// Create a "Export Results" button in the results panel to save:
// - Individual process results (what's shown in listView1)
// - Performance metrics summary for each algorithm tested
// Reference the SaveData_Click() method above to learn CSV file handling
// This will help you create tables/charts for your project report
