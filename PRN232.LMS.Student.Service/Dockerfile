FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["PRN232.LMS.Student.Service/PRN232.LMS.Student.Service.csproj", "PRN232.LMS.Student.Service/"]
COPY ["PRN232.LMS.Contracts/PRN232.LMS.Contracts.csproj", "PRN232.LMS.Contracts/"]
RUN dotnet restore "PRN232.LMS.Student.Service/PRN232.LMS.Student.Service.csproj"
COPY . .
WORKDIR "/src/PRN232.LMS.Student.Service"
RUN dotnet build "PRN232.LMS.Student.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PRN232.LMS.Student.Service.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PRN232.LMS.Student.Service.dll"]
