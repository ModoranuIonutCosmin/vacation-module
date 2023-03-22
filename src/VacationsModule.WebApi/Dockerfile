FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ./src .
RUN dotnet restore "VacationsModule.WebApi/VacationsModule.WebApi.csproj"

WORKDIR "/src/VacationsModule.WebApi"
RUN dotnet build "VacationsModule.WebApi.csproj" -c Release -o /app/build


FROM build AS publish

WORKDIR "/src/VacationsModule.WebApi"
RUN dotnet publish "VacationsModule.WebApi.csproj" -c Release -o /app/publish

FROM base AS final

WORKDIR /app

COPY --from=publish /app/publish .

WORKDIR /app

ENTRYPOINT ["dotnet", "VacationsModule.WebApi.dll"]
