FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /LogViewer
# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -o out
# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /LogViewer
COPY --from=build /LogViewer/out .
ENTRYPOINT ["dotnet", "LogViewer.dll"]