FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution
COPY PdfMarket.sln ./

# Copy project files (paths must match your real folders on disk)
COPY PdfMarket/PdfMarket.API.csproj PdfMarket/
COPY PdfMarket.Contracts/PdfMarket.Contracts.csproj PdfMarket.Contracts/

# Restore
RUN dotnet restore PdfMarket/PdfMarket.API.csproj

# Copy everything else
COPY . .

# Publish
WORKDIR /src/PdfMarket
RUN dotnet publish PdfMarket.API.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PdfMarket.API.dll"]
