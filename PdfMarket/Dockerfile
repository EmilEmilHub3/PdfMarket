# Base image til runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Kopiér csproj og restore først (hurtigere builds)
COPY *.csproj ./
RUN dotnet restore

# Kopiér resten af koden
COPY . ./

# Publish til /app/publish
RUN dotnet publish -c Release -o /app/publish

# Final image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PdfMarket.dll"]
