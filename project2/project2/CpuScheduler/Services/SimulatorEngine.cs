using CpuScheduler.Models;
using CpuScheduler.Schedulers;
using System.Globalization;
using System.Text;

namespace CpuScheduler.Services;

public static class SimulatorEngine
{ 
    // Run selected predefined workload
    public static SavedResult PreDefined(int input)
    {
        switch (input)
        {
            case 1:
                Console.WriteLine("\n*************** Metrics for CPU Bound Workload ***************");
                return RunWorkload("CPU bound", WorkloadFactory.CreateCpuBoundWorkload());
            case 2:
                Console.WriteLine("\n*************** Metrics for I/O Bound Workload ***************");
                return RunWorkload("I/O bound", WorkloadFactory.CreateIOBoundWorkload());
            case 3:
                Console.WriteLine("\n*************** Metrics for Mixed Bound Workload ***************");
                return RunWorkload("Mixed bound", WorkloadFactory.CreateMixedWorkload());
            default:
                Console.WriteLine("\nInvalid workload.");
                return null;
        }
    }
    
    // Run all algorithms using entered workload
    private static SavedResult RunWorkload(string workload, List<ProcessData> processes)
    {
        // Reference lists for CSV exports.
        List<(string Workload, string Algorithm, PerformanceMetrics Metrics)> metricsExport = new();
        List<(string Workload, string Algorithm, List<SchedulingResult> Results)> processExport = new();

        var (fcfsResults, fcfsMetrics) = RunFcfs("FCFS", processes);
        metricsExport.Add((workload, "FCFS", fcfsMetrics));
        processExport.Add((workload, "FCFS", fcfsResults));

        var (sjfResults, sjfMetrics) = RunSjf("SJF", processes);
        metricsExport.Add((workload, "SJF", sjfMetrics));
        processExport.Add((workload, "SJF", sjfResults));

        var (rrResults, rrMetrics) = RunRoundRobin("RR", 4, processes);
        metricsExport.Add((workload, "RR", rrMetrics));
        processExport.Add((workload, "RR", rrResults));

        var (priorityResults, priorityMetrics) = RunPriority("Priority", processes);
        metricsExport.Add((workload, "Priority", priorityMetrics));
        processExport.Add((workload, "Priority", priorityResults));

        var (srtfResults, srtfMetrics) = RunSrtf("SRTF", processes);
        metricsExport.Add((workload, "SRTF", srtfMetrics));
        processExport.Add((workload, "SRTF", srtfResults));

        var (hrrnResults, hrrnMetrics) = RunHrrn("HRRN", processes);
        metricsExport.Add((workload, "HRRN", hrrnMetrics));
        processExport.Add((workload, "HRRN", hrrnResults));

        var fileName = workload.Replace(" ", "_").Replace("/", "_").ToLower();
        return new SavedResult(fileName, metricsExport, processExport);
    }
    
    //====================Individual algorithm methods using entered workload===============================
    
    // First Come, First Served
    public static (List<SchedulingResult> results, PerformanceMetrics metrics) RunFcfs(string workload, List<ProcessData> processes)
    {
        // Reference lists for CSV exports.
        List<(string Workload, string Algorithm, PerformanceMetrics Metrics)> metricsExport = new();
        List<(string Workload, string Algorithm, List<SchedulingResult> Results)> processExport = new();
        
        var (fcfsResults, fcfsMetrics) = Run("FCFS", new FcfsScheduler(), processes);
        metricsExport.Add((workload, "FCFS", fcfsMetrics));
        processExport.Add((workload, "FCFS", fcfsResults));
        
        return (fcfsResults, fcfsMetrics);
    }

    // Shortest Job First
    public static  (List<SchedulingResult> results, PerformanceMetrics metrics) RunSjf(string workload, List<ProcessData> processes)
    {
        List<(string Workload, string Algorithm, PerformanceMetrics Metrics)> metricsExport = new();
        List<(string Workload, string Algorithm, List<SchedulingResult> Results)> processExport = new();
        
        var (sjfResults, sjfMetrics) = Run("SJF", new SjfScheduler(), processes);
        metricsExport.Add((workload, "SJF", sjfMetrics));
        processExport.Add((workload, "SJF", sjfResults));
        
        return (sjfResults, sjfMetrics);
    }
    
    // Round Robin
    public static  (List<SchedulingResult> results, PerformanceMetrics metrics)RunRoundRobin(string workload, int quantum, List<ProcessData> processes)
    {
        int enteredQuantum = quantum;
        List<(string Workload, string Algorithm, PerformanceMetrics Metrics)> metricsExport = new();
        List<(string Workload, string Algorithm, List<SchedulingResult> Results)> processExport = new();
        
        var (rrResults, rrMetrics) = Run("RR", new RRScheduler(4), processes);
        metricsExport.Add((workload, "RR", rrMetrics));
        processExport.Add((workload, "RR", rrResults));
        
        return (rrResults, rrMetrics);
    }
    
    // Priority
    public static  (List<SchedulingResult> results, PerformanceMetrics metrics) RunPriority(string workload, List<ProcessData> processes)
    {
        List<(string Workload, string Algorithm, PerformanceMetrics Metrics)> metricsExport = new();
        List<(string Workload, string Algorithm, List<SchedulingResult> Results)> processExport = new();
        
        var (priorityResults, priorityMetrics) = Run("Priority", new PriorityScheduler(), processes);
        metricsExport.Add((workload, "Priority", priorityMetrics));
        processExport.Add((workload, "Priority", priorityResults));
        
        return (priorityResults, priorityMetrics);
    }
    
    // Shortest Remaining Time First
    public static  (List<SchedulingResult> results, PerformanceMetrics metrics) RunSrtf(string workload, List<ProcessData> processes)
    {
        // Reference lists for CSV exports.
        List<(string Workload, string Algorithm, PerformanceMetrics Metrics)> metricsExport = new();
        List<(string Workload, string Algorithm, List<SchedulingResult> Results)> processExport = new();

        var (srtfResults, srtfMetrics) = Run("SRTF", new SrtfScheduler(), processes);
        metricsExport.Add((workload, "SRTF", srtfMetrics));
        processExport.Add((workload, "SRTF", srtfResults));

        var fileName = workload.Replace(" ", "_").Replace("/", "_").ToLower();
        return (srtfResults, srtfMetrics);
    }
    
    // Highest Response Ratio Next
    public static  (List<SchedulingResult> results, PerformanceMetrics metrics) RunHrrn(string workload, List<ProcessData> processes)
    {
        // Reference lists for CSV exports.
        List<(string Workload, string Algorithm, PerformanceMetrics Metrics)> metricsExport = new();
        List<(string Workload, string Algorithm, List<SchedulingResult> Results)> processExport = new();

        var (hrrnResults, hrrnMetrics) = Run("HRRN", new HrrnScheduler(), processes);
        metricsExport.Add((workload, "HRRN", hrrnMetrics));
        processExport.Add((workload, "HRRN", hrrnResults));

        return (hrrnResults,  hrrnMetrics);
    }
    
    // Helper run method to abstract logic for calculating and displaying results
    private static (List<SchedulingResult> results, PerformanceMetrics metrics) Run(string algorithm, IScheduler scheduler,
        List<ProcessData> processes)
    {
        List<SchedulingResult> results;
        var schedulerType = scheduler;
        results = schedulerType.Schedule(processes);
        var calculator = new MetricsCalculator();
        var metrics = calculator.Calculate(results);

        Console.WriteLine($"\n===================== {algorithm} Scheduler =========================");

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

        return (results, metrics);
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
        sb.AppendLine("Algorithm,ProcessId,ArrivalTime,BurstTime,StartTime,FinishTime,WaitingTime,TurnaroundTime");

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