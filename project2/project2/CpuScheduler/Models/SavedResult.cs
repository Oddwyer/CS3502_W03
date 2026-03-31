namespace CpuScheduler.Models;

/// <summary>
/// Data structure to store simulation results for export purposes.
/// </summary>
public class SavedResult
{
    /// <summary>
    /// Gets the base file name for the export.
    /// </summary>
    public string FileName { get;}

    /// <summary>
    /// Gets the list of performance metrics for various workloads and algorithms.
    /// </summary>
    public List<(string Workload, string Algorithm, PerformanceMetrics Metrics)> MetricsExport { get; }

    /// <summary>
    /// Gets the list of scheduling results for each process in the workloads.
    /// </summary>
    public List<(string Workload, string Algorithm, List<SchedulingResult> Results)> ProcessExport { get;}

    public SavedResult(string fileName, List<(string Workload, string Algorithm, PerformanceMetrics Metrics)> metrics, List<(string Workload, string Algorithm, List<SchedulingResult> Results)> processes)
    {
        FileName = fileName;
        MetricsExport = metrics;
        ProcessExport = processes;
    }
}