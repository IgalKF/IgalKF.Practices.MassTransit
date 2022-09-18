#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["IgalKF.Practices.MassTransit.WebApiApplication/IgalKF.Practices.MassTransit.WebApiApplication.csproj", "IgalKF.Practices.MassTransit.WebApiApplication/"]
COPY ["IgalKF.Practices.MassTransit.PersistenceModel.EntityFramework/IgalKF.Practices.MassTransit.PersistenceModel.EntityFramework.csproj", "IgalKF.Practices.MassTransit.PersistenceModel.EntityFramework/"]
COPY ["IgalKF.Practices.MassTransit.MessagingModel/IgalKF.Practices.MassTransit.MessagingModel.csproj", "IgalKF.Practices.MassTransit.MessagingModel/"]
RUN dotnet restore "IgalKF.Practices.MassTransit.WebApiApplication/IgalKF.Practices.MassTransit.WebApiApplication.csproj"
COPY . .
WORKDIR "/src/IgalKF.Practices.MassTransit.WebApiApplication"
RUN dotnet build "IgalKF.Practices.MassTransit.WebApiApplication.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IgalKF.Practices.MassTransit.WebApiApplication.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IgalKF.Practices.MassTransit.WebApiApplication.dll"]