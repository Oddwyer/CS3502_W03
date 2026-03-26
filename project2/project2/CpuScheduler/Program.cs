using CpuScheduler.Models;
using CpuScheduler.Schedulers;
using CpuScheduler.Services;
using System.Globalization;
using System.Text;

int input = 0;
string menu = """
              ================== CPU Schedule Simulator ==================
              
              1. Use a predefined workload.
              2. Enter processes manually.
              3. Exit program.    
              
              Please select an option from the menu above: 
              """;
do
{
    Console.WriteLine();
    Console.Write(menu);
    try
    {
        input = int.Parse(Console.ReadLine());
        switch (input)
        {
            case 1:
                CaseOne();
                break;
            case 2:
                CaseTwo();
                break;
            case 3: 
                Console.WriteLine("Program exited");
                return;
            default:
                Console.Write("\nPlease select from the menu options above. ");
                Console.WriteLine();
                break;
        }
    }
    catch (Exception e)
    {
        Console.WriteLine("\nInvalid entry, please try again.");
    }
} while (input != 3);


RunWorkload("CPU Bound", WorkloadFactory.CreateCpuBoundWorkload());
RunWorkload("I/O Bound", WorkloadFactory.CreateIOBoundWorkload());
RunWorkload("Mixed Bound", WorkloadFactory.CreateMixedWorkload());


//==========================Menu Methods==============================

static void CaseOne()
{
    string predefined = """
                        
                        1. Simulate with CPU bound workload.
                        2. Simulate with I/O bound workload.
                        3. Simulate with Mixed bound workload.    
                        
                        Please select a workload from the menu above: 
                        """;
    int input;
    Console.Write(predefined);
    input = int.Parse(Console.ReadLine());
    while (input != 1 && input != 2 && input != 3)
    {
        Console.WriteLine("Invalid entry.\nPlease select an option from the menu above: ");
        input = int.Parse(Console.ReadLine());
    }

    PreDefined(input);
}

static void CaseTwo()
{
    List<ProcessData> enteredProcesses;

    Console.Write("How many processes? ");
    int count = int.Parse(Console.ReadLine());
    enteredProcesses = new List<ProcessData>(count);

    for (int i = 1; i <= enteredProcesses.Count; i++)
    {
        try
        {
            Console.WriteLine($"\nEnter data for P{i}:");

            Console.Write("Arrival Time: ");
            int arrival = int.Parse(Console.ReadLine());

            Console.Write("Burst Time: ");
            int burst = int.Parse(Console.ReadLine());

            Console.Write("Priority: ");
            int priority = int.Parse(Console.ReadLine());

            enteredProcesses.Add(new ProcessData($"P{i}", arrival, burst, priority));
        }
        catch (Exception e)
        {
            Console.WriteLine("Invalid entry. Please try again.");
            i--;
        }
    }
}

//==========================Helper Methods==============================

// Run selected predefined workload
static void PreDefined(int input)
{
    switch (input)
    {
        case 1:
            Console.WriteLine("\n*************** Metrics for CPU Bound Workload ***************");
            RunWorkload("CPU bound", WorkloadFactory.CreateCpuBoundWorkload());
            break;
        case 2:
            Console.WriteLine("\n*************** Metrics for I/O Bound Workload ***************");
            RunWorkload("I/O bound", WorkloadFactory.CreateIOBoundWorkload());
            break;
        case 3:
            Console.WriteLine("\n*************** Metrics for Mixed Bound Workload ***************");
            RunWorkload("Mixed bound", WorkloadFactory.CreateMixedWorkload());
            break;
        default:
            Console.WriteLine("Invalid workload.");
            break;
    }
}

// Run method to abstract logic for calculating and displaying results
static (List<SchedulingResult> results, PerformanceMetrics metrics) Run(string algorithm, IScheduler scheduler,
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

// Run algorithms using entered workload.
static void RunWorkload(string workload, List<ProcessData> processes)
{
    // Reference lists for CSV exports.
    List<(string Workload, string Algorithm, PerformanceMetrics Metrics)> metricsExport = new();
    List<(string Workload, string Algorithm, List<SchedulingResult> Results)> processExport = new();

    var (fcfsResults, fcfsMetrics) = Run("FCFS", new FcfsScheduler(), processes);
    metricsExport.Add((workload, "FCFS", fcfsMetrics));
    processExport.Add((workload, "FCFS", fcfsResults));

    var (sjfResults, sjfMetrics) = Run("SJF", new SjfScheduler(), processes);
    metricsExport.Add((workload, "SJF", sjfMetrics));
    processExport.Add((workload, "SJF", sjfResults));

    var (rrResults, rrMetrics) = Run("RR", new RRScheduler(4), processes);
    metricsExport.Add((workload, "RR", rrMetrics));
    processExport.Add((workload, "RR", rrResults));

    var (priorityResults, priorityMetrics) = Run("Priority", new PriorityScheduler(), processes);
    metricsExport.Add((workload, "Priority", priorityMetrics));
    processExport.Add((workload, "Priority", priorityResults));

    var (srtfResults, srtfMetrics) = Run("SRTF", new SrtfScheduler(), processes);
    metricsExport.Add((workload, "SRTF", srtfMetrics));
    processExport.Add((workload, "SRTF", srtfResults));

    var (hrrnResults, hrrnMetrics) = Run("HRRN", new HrrnScheduler(), processes);
    metricsExport.Add((workload, "HRRN", hrrnMetrics));
    processExport.Add((workload, "HRRN", hrrnResults));

    var fileName = workload.Replace(" ", "_").Replace("/", "_").ToLower();

    // Saving to specified folder (Results) in project path
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

// CSV export functionality for results data: 
// "Export Results" to save: Performance metrics summary for each algorithm tested
// Takes a file path and list of rows to write to file
static void ExportMetricsToCsv(string filePath,
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

// "Export Results" to save: Individual process results
static void ExportProcessResultsToCsv(string filePath,
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