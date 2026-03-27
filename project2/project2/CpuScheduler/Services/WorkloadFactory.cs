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
    
    public static List<ProcessData> FcfsVerificationWorkload()
    {
        return new List<ProcessData>
        {
            new("P1", burstTime: 4, priority: 0, arrivalTime: 0),
            new("P2", burstTime: 3, priority: 0, arrivalTime: 1),
            new("P3", burstTime: 2, priority: 0, arrivalTime: 2),

        };
    }
    

    // ===================== Normal use workloads =====================
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

    
    // ===================== Small / verification workloads =====================

    public static List<ProcessData> CreateSmallVerificationWorkload()
    {
        return new List<ProcessData>
        {
            new("P1", 0, 5, 2),
            new("P2", 1, 3, 1),
            new("P3", 2, 8, 3),
            new("P4", 3, 6, 2),
            new("P5", 4, 2, 4)
        };
    }
    
    
    // ===================== Edge case workloads =====================

    public static List<ProcessData> CreateAllArrivalZeroWorkload()
    {
        return new List<ProcessData>
        {
            new("P1", 0, 7, 3),
            new("P2", 0, 4, 1),
            new("P3", 0, 1, 4),
            new("P4", 0, 9, 2),
            new("P5", 0, 3, 5),
            new("P6", 0, 6, 2)
        };
    }

    public static List<ProcessData> CreateIdenticalBurstWorkload()
    {
        return new List<ProcessData>
        {
            new("P1", 0, 5, 3),
            new("P2", 1, 5, 1),
            new("P3", 2, 5, 4),
            new("P4", 3, 5, 2),
            new("P5", 4, 5, 5),
            new("P6", 5, 5, 2)
        };
    }

    public static List<ProcessData> CreateBurstMixWorkload()
    {
        return new List<ProcessData>
        {
            new("P1", 0, 1, 3),
            new("P2", 1, 100, 1),
            new("P3", 2, 2, 4),
            new("P4", 3, 120, 2),
            new("P5", 4, 1, 5),
            new("P6", 5, 90, 2),
            new("P7", 6, 2, 1),
            new("P8", 7, 110, 3)
        };
    }

    public static List<ProcessData> CreateStarvationWorkload()
    {
        return new List<ProcessData>
        {
            new("P1", 0, 25, 1),  // low priority long job
            new("P2", 1, 2, 9),
            new("P3", 2, 2, 9),
            new("P4", 3, 2, 9),
            new("P5", 4, 2, 9),
            new("P6", 5, 2, 9),
            new("P7", 6, 2, 9),
            new("P8", 7, 2, 9)
        };
    }
    
    public static List<ProcessData> CreatePriorityInversionWorkload()
    {
        return new List<ProcessData>
        {
            new("P1", 0, 25, 1), // low priority long job
            new("P2", 1, 2, 9),  // high priority
            new("P3", 2, 2, 5),  // medium priority
            new("P4", 3, 2, 5),
        };
    }

    public static List<ProcessData> CreateMediumWorkload()
    {
        return new List<ProcessData>
        {
            new("P1", 0, 8, 3),
            new("P2", 1, 4, 1),
            new("P3", 2, 9, 4),
            new("P4", 3, 5, 2),
            new("P5", 4, 2, 5),
            new("P6", 5, 7, 3),
            new("P7", 6, 3, 2),
            new("P8", 7, 6, 1),
            new("P9", 8, 10, 4),
            new("P10", 9, 1, 5),
            new("P11", 10, 8, 3),
            new("P12", 11, 4, 2),
            new("P13", 12, 12, 1),
            new("P14", 13, 2, 4),
            new("P15", 14, 5, 2),
            new("P16", 15, 9, 3),
            new("P17", 16, 3, 5),
            new("P18", 17, 7, 1),
            new("P19", 18, 6, 4),
            new("P20", 19, 2, 2)
        };
    }
    

    public static List<ProcessData> CreateLargeWorkload()
    {
        return new List<ProcessData>
        {
            new("P1", 0, 4, 1),   new("P2", 1, 7, 2),   new("P3", 2, 3, 3),   new("P4", 3, 9, 4),   new("P5", 4, 2, 5),
            new("P6", 5, 5, 1),   new("P7", 6, 8, 2),   new("P8", 7, 6, 3),   new("P9", 8, 1, 4),   new("P10", 9, 10, 5),
            new("P11", 10, 4, 2), new("P12", 11, 7, 3), new("P13", 12, 3, 4), new("P14", 13, 9, 5), new("P15", 14, 2, 1),
            new("P16", 15, 5, 2), new("P17", 16, 8, 3), new("P18", 17, 6, 4), new("P19", 18, 1, 5), new("P20", 19, 10, 1),

            new("P21", 20, 4, 3), new("P22", 21, 7, 4), new("P23", 22, 3, 5), new("P24", 23, 9, 1), new("P25", 24, 2, 2),
            new("P26", 25, 5, 3), new("P27", 26, 8, 4), new("P28", 27, 6, 5), new("P29", 28, 1, 1), new("P30", 29, 10, 2),
            new("P31", 30, 4, 4), new("P32", 31, 7, 5), new("P33", 32, 3, 1), new("P34", 33, 9, 2), new("P35", 34, 2, 3),
            new("P36", 35, 5, 4), new("P37", 36, 8, 5), new("P38", 37, 6, 1), new("P39", 38, 1, 2), new("P40", 39, 10, 3),

            new("P41", 40, 4, 5), new("P42", 41, 7, 1), new("P43", 42, 3, 2), new("P44", 43, 9, 3), new("P45", 44, 2, 4),
            new("P46", 45, 5, 5), new("P47", 46, 8, 1), new("P48", 47, 6, 2), new("P49", 48, 1, 3), new("P50", 49, 10, 4),

            new("P51", 50, 4, 1), new("P52", 51, 7, 2), new("P53", 52, 3, 3), new("P54", 53, 9, 4), new("P55", 54, 2, 5),
            new("P56", 55, 5, 1), new("P57", 56, 8, 2), new("P58", 57, 6, 3), new("P59", 58, 1, 4), new("P60", 59, 10, 5),

            new("P61", 60, 4, 2), new("P62", 61, 7, 3), new("P63", 62, 3, 4), new("P64", 63, 9, 5), new("P65", 64, 2, 1),
            new("P66", 65, 5, 2), new("P67", 66, 8, 3), new("P68", 67, 6, 4), new("P69", 68, 1, 5), new("P70", 69, 10, 1),

            new("P71", 70, 4, 3), new("P72", 71, 7, 4), new("P73", 72, 3, 5), new("P74", 73, 9, 1), new("P75", 74, 2, 2),
            new("P76", 75, 5, 3), new("P77", 76, 8, 4), new("P78", 77, 6, 5), new("P79", 78, 1, 1), new("P80", 79, 10, 2),

            new("P81", 80, 4, 4), new("P82", 81, 7, 5), new("P83", 82, 3, 1), new("P84", 83, 9, 2), new("P85", 84, 2, 3),
            new("P86", 85, 5, 4), new("P87", 86, 8, 5), new("P88", 87, 6, 1), new("P89", 88, 1, 2), new("P90", 89, 10, 3),

            new("P91", 90, 4, 5), new("P92", 91, 7, 1), new("P93", 92, 3, 2), new("P94", 93, 9, 3), new("P95", 94, 2, 4),
            new("P96", 95, 5, 5), new("P97", 96, 8, 1), new("P98", 97, 6, 2), new("P99", 98, 1, 3), new("P100", 99, 10, 4)
        };
    }
}
    