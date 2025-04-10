# Use the official .NET SDK image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY MakeupReviewApp/MakeupReviewApp.csproj ./MakeupReviewApp/
RUN dotnet restore "./MakeupReviewApp/MakeupReviewApp.csproj"

# Copy everything else
COPY . .
WORKDIR /src/MakeupReviewApp
RUN dotnet publish -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "MakeupReviewApp.dll"]
