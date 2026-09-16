# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files directly from root layout
COPY ["Bus Pos Solution.sln", "./"]
COPY ["BPS.API/BPS.API.csproj", "BPS.API/"]
COPY ["BPS.Application/BPS.Application.csproj", "BPS.Application/"]
COPY ["BPS.Domain/BPS.Domain.csproj", "BPS.Domain/"]
COPY ["BPS.Infrastructure/BPS.Infrastructure.csproj", "BPS.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "BPS.API/BPS.API.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/BPS.API"
RUN dotnet build "BPS.API.csproj" -c Release -o /app/build

# Publish Stage
FROM build AS publish
RUN dotnet publish "BPS.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BPS.API.dll"]