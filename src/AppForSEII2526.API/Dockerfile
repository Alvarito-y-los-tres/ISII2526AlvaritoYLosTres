FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /AppForSEII2526.API
# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -o out
# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /AppForSEII2526.API
COPY --from=build /AppForSEII2526.API/out .
ENTRYPOINT ["dotnet", "AppForSEII2526.API.dll"]