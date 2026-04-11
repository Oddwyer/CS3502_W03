using CpuScheduler.Models;
using CpuScheduler.Schedulers;
using CpuScheduler.Services;
using System.Globalization;
using System.Text;

int input = 0;
SavedResult result = null;

string menu = """
              ****************** CPU Schedule Simulator ******************
              
              1. Use a predefined workload.
              2. Enter processes manually.
              3. Export data.
              4. Exit program.
              
              Please select an option from the menu above: 
              """;
do
{
    Console.WriteLine();
    Console.Write(menu);

    if (!int.TryParse(Console.ReadLine(), out input))
    {
        Console.WriteLine("\nInvalid input. Please enter a number.");
        return;
    }

    switch (input)
    {
        case 1:
            result = CaseOne();
            break;
        case 2:
            result = CaseTwo();
            break;
        case 3:
            CaseThree(result);
            break;
        case 4:
            Console.WriteLine("\nProgram exited.");
            return;
        default:
            Console.Write("\nPlease select from the menu options above. ");
            Console.WriteLine();
            break;
    }
} while (input != 4);


//==========================Menu Methods==============================


// Predefined Workloads scenario.
static SavedResult? CaseOne()
{
    string predefined = """

                        ----------Workload Options---------
                        Normal Use:
                        1. CPU Bound
                        2. I/O Bound
                        3. Mixed Bound    
                        
                        Special Case:
                        4. FCFS Verification
                        5. Zero Arrival
                        6. Identical Burst
                        7. Mixed Burst
                        8. Starvation
                        9. Priority/Inversion
                        10. Idle Gap
                        
                        Size:
                        11. Small
                        12. Medium
                        13. Large
                        
                        Please select a workload from the menu above: 
                        """;
    int input;
    Console.Write(predefined);
    if (!int.TryParse(Console.ReadLine(), out input))
    {
        Console.WriteLine("\nInvalid input. Please enter a number.");
        return null;
    }

    while (input < 1 || input > 13)
    {
        Console.WriteLine("Invalid entry.\nPlease select an option from the menu above: ");
        if (!int.TryParse(Console.ReadLine(), out input))
        {
            Console.WriteLine("\nInvalid input. Please enter a number.");
            return null;
        }
    }

    return SimulatorEngine.RunWorkload(input);
}

// Custom Process Entry scenario.
static SavedResult CaseTwo()
{
    List<ProcessData> enteredProcesses = new List<ProcessData>();

    // Request number of processes
    int count;
    while (true)
    {
        try
        {
            Console.Write("\nHow many processes? ");
            count = int.Parse(Console.ReadLine());
            break;
        }
        catch (Exception e)
        {
            Console.WriteLine("\nInvalid entry. Please try again.");
        }
    }

    // Request data for each process
    for (int i = 1; i <= count; i++)
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

    // Select scheduler for simulation
    int input;
    string algorithm = null;
    int quantum = 4;
    List<SchedulingResult> results = null;
    PerformanceMetrics metrics = null;
    SavedResult result = new SavedResult(
        "custom_entry",
        new List<(string, string, PerformanceMetrics)>(),
        new List<(string, string, List<SchedulingResult>)>()
    );
    string predefined = """

                        1. Run First Come First Serve (FCFS) Scheduler
                        2. Run Highest Response Ratio Next (HRRN) Scheduler
                        3. Run Priority Scheduler  
                        4. Run Round Robin (RR) Scheduler
                        5. Run Shortest Job First (SJF) Scheduler
                        6. Run Shortest Remaining Time First (SRTF) Scheduler 
                        7. Add Results for Export
                        8. Return to Main Menu

                        Please select an option from the menu above: 
                        """;
    
    do
    {
        Console.Write(predefined);
        if (!int.TryParse(Console.ReadLine(), out input))
        {
            Console.WriteLine("\nInvalid input. Please enter a number.");
            continue;
        }

        if (input < 1 || input > 8)
        {
            Console.WriteLine("Invalid entry.\nPlease select an option from the menu above: ");
            continue;
        }
        
        // Run simulation based on selected scheduler and entered processes data
        switch (input)
        {
            case 1:
            case 2:
            case 3:
            case 4:
            case 5: 
            case 6:
                if (input == 4)
                {
                    Console.Write("Enter a quantum value (default = 4): ");
                    if (!int.TryParse(Console.ReadLine(), out int enteredQuantum))
                    {
                        Console.WriteLine("\nInvalid input. Please enter a number.");
                        return null;
                    }
                    if (enteredQuantum != 4)
                    {
                        quantum = enteredQuantum;
                    }
                }
                Console.WriteLine($"\n===================== Your CPU Simulation =========================");
                (algorithm, results, metrics) = SimulatorEngine.RunScheduler(input, enteredProcesses,quantum);
                break;
            case 7:
                if (results == null || metrics == null)
                {
                    Console.WriteLine("\nTo save data, run a scheduler first (options 1-6).");
                }
                else
                {
                    result.MetricsExport.Add(("Custom Entry", algorithm, metrics));
                    result.ProcessExport.Add(("Custom Entry", algorithm, results));
                }
                Console.WriteLine($"Metrics count: {result.MetricsExport.Count}");
                Console.WriteLine($"Process count: {result.ProcessExport.Count}");
                break;
            case 8:
                Console.WriteLine("Returning to menu.");
                break;
        }
    } while (input != 8);

    return result;
}

// Export results to CSV.
static void CaseThree(SavedResult result)
{
    if (result == null)
    {
        Console.WriteLine("\nTo export data, please select options 1 or 2 first.");
    }
    else
    {
        SimulatorEngine.ExportData(result);
    }
}