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

    return SimulatorEngine.RunWorkload("Custom Entry", enteredProcesses);
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