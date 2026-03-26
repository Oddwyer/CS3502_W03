using System.Diagnostics;
using System.Net;
using CpuScheduler.Models;

namespace CpuScheduler.Services;

/// <summary>
/// Predefined sets of processes used to evaluate scheduling algorithms consistently across multiple runs
/// Each method produces a dataset: CPU bounds, I/O bounds, and mixed workloads (CPU + I/O)
/// </summary>

public static class WorkloadFactory
{

    public static List<ProcessData> CreateCpuBoundWorkload()
        {
            return new List<ProcessData>
            {
                new("P1", burstTime: 12, priority: 2, arrivalTime: 0),
                new("P2", burstTime: 15, priority: 1, arrivalTime: 0),
                new("P3", burstTime: 10, priority: 3, arrivalTime: 1),
                new("P4", burstTime: 18, priority: 2, arrivalTime: 2),
                new("P5", burstTime: 14, priority: 1, arrivalTime: 3)
            };
        }

        public static List<ProcessData> CreateIOBoundWorkload()
        {
            return new List<ProcessData>
            {
                new("P1", burstTime: 2, priority: 2, arrivalTime: 0),
                new("P2", burstTime: 1, priority: 1, arrivalTime: 1),
                new("P3", burstTime: 3, priority: 3, arrivalTime: 2),
                new("P4", burstTime: 2, priority: 2, arrivalTime: 3),
                new("P5", burstTime: 1, priority: 1, arrivalTime: 4)
            };
        }

        public static List<ProcessData> CreateMixedWorkload()
        {
            return new List<ProcessData>
            {
                new("P1", burstTime: 12, priority: 2, arrivalTime: 0),
                new("P2", burstTime: 2,  priority: 1, arrivalTime: 1),
                new("P3", burstTime: 8,  priority: 3, arrivalTime: 2),
                new("P4", burstTime: 1,  priority: 2, arrivalTime: 3),
                new("P5", burstTime: 6,  priority: 1, arrivalTime: 4)
            };
        }
    }