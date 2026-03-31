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
- CSV export of metrics and per-process results (exported to the Results/ folder). Files include:
  - Metrics summary CSV
  - Per-process scheduling results CSV

## Project Structure
- `Program.cs` — main menu and user interaction
- `Models/` — process, results, and metrics data models
- `Schedulers/` — scheduling algorithm implementations
- `Services/` — simulation engine, metrics calculation, workload generation, and export logic

---

## Platform & Build Specifications
### Platform Tested on:
- .NET 8
- JetBrains Rider
- Linux console
- Docker

#### Build Requirements
- .NET 8 SDK
- Terminal/console environment
- Optional: JetBrains Rider or Visual Studio
- Optional: Docker Desktop / Docker Engine

### Dependencies
- .NET 8 SDK
- No external NuGet packages required
---
## Build Instructions

These instructions allow the application to be executed either locally (console/IDE) or within a Docker container for consistent cross-platform behavior.

### Build & Run (IDE)
1. Open the solution in JetBrains Rider or Visual Studio.
2. Restore dependencies if prompted.
3. Set `CpuScheduler` as the startup project.
4. Build and run the project.

#### Rider
- Open `project2.sln`
- Select the `CpuScheduler` run configuration
- Click Run

### Build & Run (Console)

From the directory containing the solution file:
```bash
cd project2/CpuScheduler
dotnet restore
dotnet build
dotnet run
```
### Build & Run (Docker)

From the directory containing the Dockerfile: 
```bash
cd project2
```
Build the image:
```bash
docker build -t cpu-scheduler .
```
Run the container:
```bash
docker run -it --rm cpu-scheduler
```


---
