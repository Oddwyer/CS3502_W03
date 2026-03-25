using CpuScheduler.Models;
using CpuScheduler.Schedulers;
using CpuScheduler.Services;
using System.Globalization;
using System.Text;


// Reference lists for CSV exports.
List<(string Algorithm, PerformanceMetrics Metrics)> metricsExport = new();
List<(string Algorithm, List<SchedulingResult> Results)> processExport = new();

var (fcfsResults, fcfsMetrics) = Run("FCFS", new FcfsScheduler(), WorkloadFactory.CreateCpuBoundWorkload());
metricsExport.Add(("FCFS", fcfsMetrics));
processExport.Add(("FCFS", fcfsResults));

var (sjfResults, sjfMetrics) = Run("SJF", new SjfScheduler(), WorkloadFactory.CreateCpuBoundWorkload());
metricsExport.Add(("SJF", sjfMetrics));
processExport.Add(("SJF", sjfResults));

var (rrResults, rrMetrics) = Run("RR", new RRScheduler(4), WorkloadFactory.CreateCpuBoundWorkload());
metricsExport.Add(("RR", rrMetrics));
processExport.Add(("RR", rrResults));

var (priorityResults, priorityMetrics) = Run("Priority", new PriorityScheduler(), WorkloadFactory.CreateCpuBoundWorkload());
metricsExport.Add(("Priority", priorityMetrics));
processExport.Add(("Priority", priorityResults));

var (srtfResults, srtfMetrics) = Run("SRTF", new SrtfScheduler(), WorkloadFactory.CreateCpuBoundWorkload());
metricsExport.Add(("SRTF", srtfMetrics));
processExport.Add(("SRTF", srtfResults));

var (hrrnResults, hrrnMetrics) = Run("HRRN", new HrrnScheduler(), WorkloadFactory.CreateCpuBoundWorkload());
metricsExport.Add(("HRRN", hrrnMetrics));
processExport.Add(("HRRN", hrrnResults));

ExportMetricsToCsv("cpu_bound_metrics.csv", metricsExport);
ExportProcessResultsToCsv("cpu_bound_process_results.csv", processExport);

Console.WriteLine("\nCSV export complete.");

//==========================Helper Methods==============================

// Run method to abstract logic for calculating and displaying results
static (List<SchedulingResult> results, PerformanceMetrics metrics) Run(string algorithm, IScheduler scheduler, List<ProcessData> processes)
{
    string name =  algorithm;
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

    return (results, metrics);
}

// TODO: STUDENTS - Add CSV export functionality for results data
// Create a "Export Results" to save:
// - Individual process results
// - Performance metrics summary for each algorithm tested
// This helps create tables/charts for generated results

// Export of performance metrics per algorthim
static void ExportMetricsToCsv(string filePath, List<(string Algorithm, PerformanceMetrics Metrics)> rows)
{
    var sb = new StringBuilder();
    sb.AppendLine("Algorithm,AverageWaitingTime,AverageTurnaroundTime,CpuUtilization,Throughput,ResponseTime");

    foreach (var row in rows)
    {
        sb.AppendLine(
            $"{row.Algorithm}," +
            $"{row.Metrics.AverageWaitingTime.ToString("F2", CultureInfo.InvariantCulture)}," +
            $"{row.Metrics.AverageTurnaroundTime.ToString("F2", CultureInfo.InvariantCulture)}," +
            $"{row.Metrics.CpuUtilization.ToString("F1", CultureInfo.InvariantCulture)}," +
            $"{row.Metrics.Throughput.ToString("F3", CultureInfo.InvariantCulture)}," +
            $"{row.Metrics.ResponseTime.ToString("F2", CultureInfo.InvariantCulture)}");
    }

    File.WriteAllText(filePath, sb.ToString());
}

// Export of individual process results
static void ExportProcessResultsToCsv(string filePath, List<(string Algorithm, List<SchedulingResult> Results)> rows)
{
    var sb = new StringBuilder();
    sb.AppendLine("Algorithm,ProcessId,ArrivalTime,BurstTime,StartTime,FinishTime,WaitingTime,TurnaroundTime");

    foreach (var row in rows)
    {
        foreach (var result in row.Results)
        {
            sb.AppendLine(
                $"{row.Algorithm}," +
                $"{result.ProcessId}," +
                $"{result.ArrivalTime}," +
                $"{result.BurstTime}," +
                $"{result.StartTime}," +
                $"{result.FinishTime}," +
                $"{result.WaitingTime}," +
                $"{result.TurnaroundTime}");
        }
    }

    File.WriteAllText(filePath, sb.ToString());
}

