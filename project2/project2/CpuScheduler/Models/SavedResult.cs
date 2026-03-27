namespace CpuScheduler.Models;

public class SavedResult
{
    public string FileName { get;}
    public List<(string Workload, string Algorithm, PerformanceMetrics Metrics)> MetricsExport { get; }
    public List<(string Workload, string Algorithm, List<SchedulingResult> Results)> ProcessExport { get;}

    public SavedResult(string fileName, List<(string Workload, string Algorithm, PerformanceMetrics Metrics)> metrics, List<(string Workload, string Algorithm, List<SchedulingResult> Results)> processes)
    {
        FileName = fileName;
        MetricsExport = metrics;
        ProcessExport = processes;
    }
}