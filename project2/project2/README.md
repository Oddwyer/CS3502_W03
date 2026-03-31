# CPU Scheduling Simulator

## Overview
This project is a console-based CPU Scheduling Simulator developed for CS 3502: Operating Systems at Kennesaw State University. It allows users to run and compare multiple CPU scheduling algorithms across predefined and custom workloads.

Implemented algorithms:
- FCFS
- SJF
- Round Robin
- Priority
- SRTF
- HRRN

The simulator supports:
- Predefined workloads
- Manual process entry
- Algorithm comparison
- CSV export of metrics and per-process results (exposred to the Resuts/folder). Files include:
  - Metrics summary CSV
  - Per-process scheduling results CSV

## Platform and Build Specs
Tested on:
- .NET 10
- JetBrains Rider
- Linux console
- Docker

## Project Structure
- `Program.cs` — main menu and user interaction
- `Models/` — process, results, and metrics data models
- `Schedulers/` — scheduling algorithm implementations
- `Services/` — simulation engine, metrics calculation, workload generation, and export logic

## Build & Run (IDE)
1. Open the solution in JetBrains Rider or Visual Studio.
2. Restore dependencies if prompted.
3. Set `CpuScheduler` as the startup project.
4. Build and run the project.

### Rider
- Open `project2.sln`
- Select the `CpuScheduler` run configuration
- Click Run

## Build & Run (Console)
From the project root:

```bash
cd CpuScheduler
dotnet restore
dotnet build
dotnet run
```
## Build & Run (Docker)

Build the image:
```bash
docker build -t cpu-scheduler .
```
Run the container:
```bash
docker run -it --rm cpu-scheduler
```


---

# Console Build Specs

## Build Requirements
- .NET 10 SDK
- Terminal/console environment
- Optional: JetBrains Rider or Visual Studio
- Optional: Docker Desktop / Docker Engine

You could also add:

```md
## Dependencies
- .NET 10 SDK
- No external NuGet packages required