#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["IgalKF.Practices.MassTransit.ConsoleApplication/IgalKF.Practices.MassTransit.ConsoleApplication.csproj", "IgalKF.Practices.MassTransit.ConsoleApplication/"]
COPY ["IgalKF.Practices.MassTransit.PersistenceModel.EntityFramework/IgalKF.Practices.MassTransit.PersistenceModel.EntityFramework.csproj", "IgalKF.Practices.MassTransit.PersistenceModel.EntityFramework/"]
COPY ["IgalKF.Practices.MassTransit.MessagingModel/IgalKF.Practices.MassTransit.MessagingModel.csproj", "IgalKF.Practices.MassTransit.MessagingModel/"]
COPY ["IgalKF.Practices.MassTransit.FlowModel.Impl/IgalKF.Practices.MassTransit.FlowModel.Impl.csproj", "IgalKF.Practices.MassTransit.FlowModel.Impl/"]
COPY ["IgalKF.Practices.MassTransit.Database.Migrations/IgalKF.Practices.MassTransit.Database.csproj", "IgalKF.Practices.MassTransit.Database.Migrations/"]
RUN dotnet restore "IgalKF.Practices.MassTransit.ConsoleApplication/IgalKF.Practices.MassTransit.ConsoleApplication.csproj"
COPY . .
WORKDIR "/src/IgalKF.Practices.MassTransit.ConsoleApplication"
RUN dotnet build "IgalKF.Practices.MassTransit.ConsoleApplication.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IgalKF.Practices.MassTransit.ConsoleApplication.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IgalKF.Practices.MassTransit.ConsoleApplication.dll"]