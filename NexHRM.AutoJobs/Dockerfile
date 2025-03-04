FROM hub.nexdev.net/base/dotnet/core/sdk:8.0-alpine AS builder

WORKDIR /src

COPY ["NexHRM.AutoJobs/NexHRM.AutoJobs.csproj", "NexHRM.AutoJobs/"]

RUN dotnet nuget add source https://nuget.nexdev.net/api/v2
RUN dotnet restore -v n -r linux-x64 "NexHRM.AutoJobs/NexHRM.AutoJobs.csproj"

COPY ["NexHRM.AutoJobs/", "NexHRM.AutoJobs/"]

WORKDIR /src/NexHRM.AutoJobs

RUN dotnet publish -r linux-x64 --self-contained false -o /app

FROM hub.nexdev.net/base/dotnet/core/aspnet:8.0-slim AS base

# Don’t run as root
RUN adduser --disabled-password \
  --home /app \
  --gecos '' dotnetuser && chown -R dotnetuser /app && chown -R dotnetuser /tmp
USER dotnetuser
#

WORKDIR /app
COPY --from=builder /app .
ENV TZ="Asia/Bangkok"

ENTRYPOINT ["dotnet", "NexHRM.AutoJobs.dll"]
