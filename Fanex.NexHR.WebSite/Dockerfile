

FROM hub.nexdev.net/base/dotnet/core/sdk:9.0-alpine AS builder

WORKDIR /src

COPY ["Fanex.NexHR.WebSite/Fanex.NexHR.WebSite.csproj", "Fanex.NexHR.WebSite/"]
COPY ["Fanex.NexHR.Criteria/Fanex.NexHR.Criteria.csproj", "Fanex.NexHR.Criteria/"]
COPY ["Fanex.NexHR.Core/Fanex.NexHR.Core.csproj", "Fanex.NexHR.Core/"]
COPY ["Fanex.NexHR.Resources/Fanex.NexHR.Resources.csproj", "Fanex.NexHR.Resources/"]
COPY ["Fanex.NexHR.Models/Fanex.NexHR.Models.csproj", "Fanex.NexHR.Models/"]
COPY ["Fanex.NexHR.Repositories/Fanex.NexHR.Repositories.csproj", "Fanex.NexHR.Repositories/"]
COPY ["Fanex.NexHR.Services/Fanex.NexHR.Services.csproj", "Fanex.NexHR.Services/"]
COPY ["Fanex.NexHR.ViewModels/Fanex.NexHR.ViewModels.csproj", "Fanex.NexHR.ViewModels/"]
COPY ["Fanex.NexHR.Controllers/Fanex.NexHR.Controllers.csproj", "Fanex.NexHR.Controllers/"]
COPY ["nuget.config", ""]

RUN dotnet nuget add source https://nuget.nexdev.net/api/v2

RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet restore -v n -r linux-x64 ./Fanex.NexHR.WebSite/Fanex.NexHR.WebSite.csproj

COPY . .

WORKDIR "/src/Fanex.NexHR.WebSite"

RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet publish -r linux-x64 --self-contained false -o /app

FROM hub.nexdev.net/base/dotnet/core/aspnet:9.0-alpine AS base

WORKDIR /app

COPY --from=builder /app .

ENV TZ="Asia/Bangkok"
ENV ASPNETCORE_URLS=http://+:5000

ENTRYPOINT ["dotnet", "Fanex.NexHR.WebSite.dll"]