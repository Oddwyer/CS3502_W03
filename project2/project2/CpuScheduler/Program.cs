using CpuScheduler.Models;
using CpuScheduler.Schedulers;
using CpuScheduler.Services;

//==================================FCFS Scheduler====================================

var processes = new List<Process>
{
    new("P1", 0, 4, 1),
    new("P2", 1, 3, 2),
    new("P3", 2, 2, 3)
};

var fcfsScheduler = new FcfsScheduler();
var results = fcfsScheduler.Schedule(processes);

foreach (var result in results)
{
    Console.WriteLine(
        $"{result.ProcessId}: Start={result.StartTime}, Finish={result.FinishTime}, " +
        $"Wait={result.WaitingTime}, Turnaround={result.TurnaroundTime}");
}

var calculator = new MetricsCalculator();
var metrics = calculator.Calculate(results);

Console.WriteLine("\n=== FCFS Results ===");

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



//==================================SJF Scheduler====================================

processes = new List<Process>
{
    new("P1", 0, 8, 1),
    new("P2", 1, 4, 1),
    new("P3", 2, 2, 1)
};

var sjfScheduler = new SjfScheduler();
results = sjfScheduler.Schedule(processes);

foreach (var result in results)
{
    Console.WriteLine(
        $"{result.ProcessId}: Start={result.StartTime}, Finish={result.FinishTime}, " +
        $"Wait={result.WaitingTime}, Turnaround={result.TurnaroundTime}");
}

calculator = new MetricsCalculator(); 
metrics = calculator.Calculate(results);

Console.WriteLine("\n=== SJF Results ===");

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


//==================================Priority Scheduler====================================

processes = new List<Process>
{
    new("P1", 0, 5, 2),
    new("P2", 0, 3, 1),
    new("P3", 0, 4, 3)
};

var priorityScheduler = new PriorityScheduler();
results = priorityScheduler.Schedule(processes);

foreach (var result in results)
{
    Console.WriteLine(
        $"{result.ProcessId}: Start={result.StartTime}, Finish={result.FinishTime}, " +
        $"Wait={result.WaitingTime}, Turnaround={result.TurnaroundTime}");
}

calculator = new MetricsCalculator(); 
metrics = calculator.Calculate(results);

Console.WriteLine("\n=== Priority Results (Higher Number = Higher Priority) ===");

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


//==================================RR Scheduler====================================

processes = new List<Process>
{
    new("P1", 0, 5, 2),
    new("P2", 1, 3, 1),
    new("P3", 2, 2, 3)
};

var rrScheduler = new RRScheduler();
results = rrScheduler.Schedule(processes, 2);

foreach (var result in results)
{
    Console.WriteLine(
        $"{result.ProcessId}: Start={result.StartTime}, Finish={result.FinishTime}, " +
        $"Wait={result.WaitingTime}, Turnaround={result.TurnaroundTime}");
}

calculator = new MetricsCalculator(); 
metrics = calculator.Calculate(results);

Console.WriteLine("\n=== RR Scheduler ===");

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

//==================================SRTF Scheduler====================================

/*processes = new List<Process>
{
    new("P1", 0, 5, 2),
    new("P2", 1, 3, 1),
    new("P3", 2, 2, 3)
};

var srtfScheduler = new SrtfScheduler();
results = srtfScheduler.Schedule(processes);

foreach (var result in results)
{
    Console.WriteLine(
        $"{result.ProcessId}: Start={result.StartTime}, Finish={result.FinishTime}, " +
        $"Wait={result.WaitingTime}, Turnaround={result.TurnaroundTime}");
}

calculator = new MetricsCalculator(); 
metrics = calculator.Calculate(results);

Console.WriteLine("\n=== RR Scheduler ===");

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
Console.WriteLine($"Throughput: {metrics.Throughput:F3}");*/


// TODO: STUDENTS - Add CSV export functionality for results data
// Create a "Export Results" button in the results panel to save:
// - Individual process results (what's shown in listView1)
// - Performance metrics summary for each algorithm tested
// Reference the SaveData_Click() method above to learn CSV file handling
// This will help you create tables/charts for your project report