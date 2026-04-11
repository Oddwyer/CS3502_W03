using CpuScheduler.Models;
using CpuScheduler.Schedulers;
using System.Globalization;
using System.Text;

/// <summary>
/// Simulator engine. Handles running of schedulers, process display, exporting of data
/// </summary>

namespace CpuScheduler.Services;

public static class SimulatorEngine
{
    // Run selected predefined workload
    public static SavedResult RunWorkload(int input)
    {
        // Dictionary of available workload creators
        var workloads = new Dictionary<int, (string name, Func<List<ProcessData>> Create)>
        {
            { 1, ("CPU Bound", WorkloadFactory.CreateCpuBoundWorkload) },
            { 2, ("I/O Bound", WorkloadFactory.CreateIOBoundWorkload) },
            { 3, ("Mixed Bound", WorkloadFactory.CreateMixedWorkload) },
            { 4, ("FCFS Verification", WorkloadFactory.FcfsVerificationWorkload) },
            { 5, ("Zero Arrival", WorkloadFactory.CreateAllArrivalZeroWorkload) },
            { 6, ("Identical Burst", WorkloadFactory.CreateIdenticalBurstWorkload) },
            { 7, ("Mixed Burst", WorkloadFactory.CreateBurstMixWorkload) },
            { 8, ("Starvation", WorkloadFactory.CreateStarvationWorkload) },
            { 9, ("Priority/Inversion", WorkloadFactory.CreatePriorityInversionWorkload) },
            { 10, ("Idle Gap", WorkloadFactory.CreateIdleGapWorkload) },
            { 11, ("Small", WorkloadFactory.CreateSmallVerificationWorkload) },
            { 12, ("Medium", WorkloadFactory.CreateMediumWorkload) },
            { 13, ("Large", WorkloadFactory.CreateLargeWorkload) }
        };

        // Search for workload choice in dictionary. If exists, run workload against all schedulers
        if (workloads.TryGetValue(input, out var workload))
        {
            Console.WriteLine($"\n*************** Metrics for {workload.name} Workload ***************");
            var processes = workload.Create();
            PrintProcesses(processes);
            return RunAllSchedulers(workload.name, processes);
        }

        else
        {
            Console.WriteLine("Invalid selection.");
            return null;
        }
    }
    
    // Run one selected scheduler
    public static (string algorithm, List<SchedulingResult> results, PerformanceMetrics metrics) RunScheduler(int choice,
        List<ProcessData> processes, int quantum)
    {
        // Reference lists for CSV exports.
        List<(string Workload, string Algorithm, PerformanceMetrics Metrics)> metricsExport = new();
        List<(string Workload, string Algorithm, List<SchedulingResult> Results)> processExport = new();

        // Dictionary of available schedulers
        var schedulers = new Dictionary<int, (string name, IScheduler scheduler)>
        {
            { 1, ("FCFS", new FcfsScheduler()) },
            { 2, ("HRRN", new HrrnScheduler()) },
            { 3, ("Priority", new PriorityScheduler()) },
            { 4, ("RR", new RRScheduler(quantum)) },
            { 5, ("SJF", new SjfScheduler()) },
            { 6, ("SRTF", new SrtfScheduler()) }
        };

        // Search for scheduler choice in dictionary. If exists, run scheduler
        if (schedulers.TryGetValue(choice, out var selected))
        {
            PrintProcesses(processes);
            var (results, metrics) = Run(selected.name, selected.scheduler, processes);
            metricsExport.Add(("Custom Workload", selected.name, metrics));
            processExport.Add(("Custom Workload", selected.name, results));

            return (selected.name, results, metrics);
        }
        else
        {
            throw new ArgumentException("Invalid selection.");
        }
    }

    
    //============================= Helper methods ============================== 
    
    // Run all algorithms using entered workload
    private static SavedResult RunAllSchedulers(string workload, List<ProcessData> processes)
    {
        // Reference lists for CSV exports
        List<(string Workload, string Algorithm, PerformanceMetrics Metrics)> metricsExport = new();
        List<(string Workload, string Algorithm, List<SchedulingResult> Results)> processExport = new();
        
        // Run all algorithms and save results for export
        
        // First Come, First Served
        var (fcfsResults, fcfsMetrics) = Run("FCFS", new FcfsScheduler(), processes);
        metricsExport.Add((workload, "FCFS", fcfsMetrics));
        processExport.Add((workload, "FCFS", fcfsResults));
        
        // Highest Response Ratio Next
        var (hrrnResults, hrrnMetrics) = Run("HRRN", new HrrnScheduler(), processes);
        metricsExport.Add((workload, "HRRN", hrrnMetrics));
        processExport.Add((workload, "HRRN", hrrnResults));
        
        // Priority
        var (priorityResults, priorityMetrics) = Run("Priority", new PriorityScheduler(), processes);
        metricsExport.Add((workload, "Priority", priorityMetrics));
        processExport.Add((workload, "Priority", priorityResults));

        // Round Robin
        var (rrResults, rrMetrics) = Run("RR", new RRScheduler(4), processes);
        metricsExport.Add((workload, "RR", rrMetrics));
        processExport.Add((workload, "RR", rrResults));
        
        // Shortest Job First
        var (sjfResults, sjfMetrics) = Run("SJF", new SjfScheduler(), processes);
        metricsExport.Add((workload, "SJF", sjfMetrics));
        processExport.Add((workload, "SJF", sjfResults));

        // Shortest Remaining Time First
        var (srtfResults, srtfMetrics) = Run("SRTF", new SrtfScheduler(), processes);
        metricsExport.Add((workload, "SRTF", srtfMetrics));
        processExport.Add((workload, "SRTF", srtfResults));
        
        var fileName = workload.Replace(" ", "_").Replace("/", "_").ToLower();
        return new SavedResult(fileName, metricsExport, processExport);
    }
    
    // Helper run method to abstract logic for calculating and displaying results
    private static (List<SchedulingResult> results, PerformanceMetrics metrics) Run(string algorithm,
        IScheduler scheduler,
        List<ProcessData> processes)
    {
        List<SchedulingResult> results;
        var schedulerType = scheduler;
        results = schedulerType.Schedule(processes);
        var calculator = new MetricsCalculator();
        var metrics = calculator.Calculate(results);

        Console.WriteLine($"\n=== {algorithm} Scheduled Processes ===");

        foreach (var result in results)
        {
            Console.WriteLine(
                $"{result.ProcessId,-5} | Start: {result.StartTime,-3} | " +
                $"Finish: {result.FinishTime,-3} | Wait: {result.WaitingTime,-3} | " +
                $"Turnaround: {result.TurnaroundTime,-3}");
        }

        Console.WriteLine();
        Console.WriteLine($"=== {algorithm} Performance Metrics ===");
        Console.WriteLine($"Average Waiting Time: {metrics.AverageWaitingTime:F2}");
        Console.WriteLine($"Average Turnaround Time: {metrics.AverageTurnaroundTime:F2}");
        Console.WriteLine($"CPU Utilization: {metrics.CpuUtilization:F1}%");
        Console.WriteLine($"Throughput: {metrics.Throughput:F3}");
        Console.WriteLine($"Response Time: {metrics.ResponseTime:F2}");
        Console.WriteLine($"\n----------------------------------------------------------------");
        return (results, metrics);
    }

    // Display entered processes
    private static void PrintProcesses(List<ProcessData> processes)
    {
        Console.WriteLine("\nCurrent Processes:");
        for (int i = 0; i < processes.Count; i++)
        {
            Console.WriteLine(
                $"P{i + 1}: Arrival Time: {processes[i].ArrivalTime}\tBurst Time: {processes[i].BurstTime}\tPriority: {processes[i].Priority}");
        }
    }
    
    
    //================= CSV export functionality for results data =================
    
    // Saving to specified folder (Results) in project path
    public static void ExportData(SavedResult result)
    {
        string fileName = result.FileName;
        List<(string Workload, string Algorithm, PerformanceMetrics Metrics)> metricsExport = result.MetricsExport;
        List<(string Workload, string Algorithm, List<SchedulingResult> Results)> processExport = result.ProcessExport;

        string workload = metricsExport.FirstOrDefault().Workload;
        var outputDir = "Results";
        Directory.CreateDirectory(outputDir);

        var metricsPath = Path.Combine(outputDir, $"{fileName}_metrics.csv");
        var processPath = Path.Combine(outputDir, $"{fileName}_process_results.csv");

        ExportMetricsToCsv(metricsPath, metricsExport);
        ExportProcessResultsToCsv(processPath, processExport);

        Console.WriteLine($"\nCSV export complete for {workload}.");
        Console.WriteLine($"Saved to: {metricsPath}");
        Console.WriteLine($"Saved to: {processPath}");
    }

    // Helper "export results" method: saves performance metrics summary for each algorithm tested
    private static void ExportMetricsToCsv(string filePath,
        List<(string Workload, string Algorithm, PerformanceMetrics Metrics)> rows)
    {
        // Create text builder
        var sb = new StringBuilder();

        // Header row for csv file
        sb.AppendLine("Algorithm,AverageWaitingTime,AverageTurnaroundTime,CpuUtilization,Throughput,ResponseTime");

        // For each row, add new algorithm and respective metrics
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

        // Save to file
        File.WriteAllText(filePath, sb.ToString());
    }

    // Helper "export results" method: saves individual process results
    private static void ExportProcessResultsToCsv(string filePath,
        List<(string Workload, string Algorithm, List<SchedulingResult> Results)> rows)
    {
        // Create a text builder
        var sb = new StringBuilder();

        // Header row for csv file
        sb.AppendLine("Workload,Algorithm,ProcessId,ArrivalTime,BurstTime,StartTime,FinishTime,WaitingTime,TurnaroundTime");

        // For each algorithm in the list...
        foreach (var row in rows)
        {
            // ...Add details related to each process in the algorithm's results list
            foreach (var result in row.Results)
            {
                sb.AppendLine(
                    $"{row.Workload}," +
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

        // Save to file
        File.WriteAllText(filePath, sb.ToString());
    }
}