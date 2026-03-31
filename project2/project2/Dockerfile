FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY CpuScheduler/CpuScheduler.csproj CpuScheduler/
RUN dotnet restore CpuScheduler/CpuScheduler.csproj

COPY . .
WORKDIR /src/CpuScheduler
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CpuScheduler.dll"]