FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY *.sln .
COPY AwakeningLifeBackend/*.csproj ./AwakeningLifeBackend/
COPY AwakeningLifeBackend.Infrastructure.Presentation/*.csproj ./AwakeningLifeBackend.Infrastructure.Presentation/
COPY AwakeningLifeBackend.Core.Domain/*.csproj ./AwakeningLifeBackend.Core.Domain/
COPY AwakeningLifeBackend.Core.Services/*.csproj ./AwakeningLifeBackend.Core.Services/
COPY AwakeningLifeBackend.Core.Services.Abstractions/*.csproj ./AwakeningLifeBackend.Core.Services.Abstractions/
COPY AwakeningLifeBackend.Infrastructure.Persistence/*.csproj ./AwakeningLifeBackend.Infrastructure.Persistence/
COPY LoggingService/*.csproj ./LoggingService/
COPY Shared/*.csproj ./Shared/

RUN dotnet restore

COPY AwakeningLifeBackend/ ./AwakeningLifeBackend/
COPY AwakeningLifeBackend.Infrastructure.Presentation/ ./AwakeningLifeBackend.Infrastructure.Presentation/
COPY AwakeningLifeBackend.Core.Domain/ ./AwakeningLifeBackend.Core.Domain/
COPY AwakeningLifeBackend.Core.Services/ ./AwakeningLifeBackend.Core.Services/
COPY AwakeningLifeBackend.Core.Services.Abstractions/ ./AwakeningLifeBackend.Core.Services.Abstractions/
COPY AwakeningLifeBackend.Infrastructure.Persistence/ ./AwakeningLifeBackend.Infrastructure.Persistence/
COPY LoggingService/ ./LoggingService/
COPY Shared/ ./Shared/

WORKDIR "/src/AwakeningLifeBackend"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish
# Ensure XML documentation files are included in the publish output
RUN cp /src/AwakeningLifeBackend.Infrastructure.Presentation/AwakeningLifeBackend.Infrastructure.Presentation.xml /app/publish/

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AwakeningLifeBackend.dll"]
