using CpuScheduler.Models;
using CpuScheduler.Schedulers;
using CpuScheduler.Services;
using System.Globalization;
using System.Text;

int input = 0;
SavedResult result = null;
string menu = """
              ================== CPU Schedule Simulator ==================

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

static SavedResult CaseOne()
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

    return SimulatorEngine.PreDefined(input);
}

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

    // Request scheduler for simulation
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
                        5. Run Shortest Job First (SFJ) Scheduler
                        6. Run Shortest Remaining Time First (SRTF) Scheduler 
                        7. Add Results for Export
                        8. Finish simulation

                        Please select an option above: 
                        """;
    do
    {
        Console.Write(predefined);
        input = int.Parse(Console.ReadLine());
        while (input < 1 || input > 8)
        {
            Console.WriteLine("Invalid entry.\nPlease select an algorithm from the menu above: ");
            input = int.Parse(Console.ReadLine());
        }

        // Run simulation based on selected scheduler and entered process data
        switch (input)
        {
            case 1:
                (results, metrics) = SimulatorEngine.RunFcfs("Custom Entry", enteredProcesses);
                algorithm = "FCFS";
                break;
            case 2:
                (results, metrics) = SimulatorEngine.RunHrrn("Custom Entry", enteredProcesses);
                algorithm = "HRRN";
                break;
            case 3:
                (results, metrics)  = SimulatorEngine.RunPriority("Custom Entry", enteredProcesses);
                algorithm = "Priority";
                break;
            case 4:
                Console.Write("Enter a quantum value (default = 4): ");
                if (!int.TryParse(Console.ReadLine(), out input))
                {
                    Console.WriteLine("\nInvalid input. Please enter a number.");
                    return null;
                }
                if (input != 4)
                {
                    quantum = input;
                }
                (results, metrics) = SimulatorEngine.RunRoundRobin("Custom Entry", quantum, enteredProcesses);
                algorithm = "RR";
                break;
            case 5:
                (results, metrics)  = SimulatorEngine.RunSjf("Custom Entry", enteredProcesses);
                algorithm = "SJF";
                break;
            case 6:
                (results, metrics)  = SimulatorEngine.RunSrtf("Custom Entry", enteredProcesses);
                algorithm = "SRTF";
                break;
            case 7:
                result.MetricsExport.Add(("Custom Entry", algorithm, metrics));
                result.ProcessExport.Add(("Custom Entry", algorithm, results));
                break;
            case 8:
                Console.WriteLine("Returning to menu.");
                break;
        }
    } while (input != 8);

    return result;
}

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