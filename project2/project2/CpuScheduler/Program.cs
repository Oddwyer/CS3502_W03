using CpuScheduler.Models;
using CpuScheduler.Schedulers;
using CpuScheduler.Services;

var processes = new List<Process>
{
    new("P1", 0, 5, 2),
    new("P2", 1, 3, 1),
    new("P3", 2, 8, 3)
};

var scheduler = new FcfsScheduler();
var results = scheduler.Schedule(processes);

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
Console.WriteLine("=== Performance Metrics ===");
Console.WriteLine($"Average Waiting Time: {metrics.AverageWaitingTime:F1}");
Console.WriteLine($"Average Turnaround Time: {metrics.AverageTurnaroundTime:F1}");