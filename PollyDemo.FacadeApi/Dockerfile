FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["PollyDemo.FacadeApi/PollyDemo.FacadeApi.csproj", "PollyDemo.FacadeApi/"]
RUN dotnet restore "PollyDemo.FacadeApi/PollyDemo.FacadeApi.csproj"
COPY . .
WORKDIR "/src/PollyDemo.FacadeApi"
RUN dotnet build "PollyDemo.FacadeApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PollyDemo.FacadeApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PollyDemo.FacadeApi.dll"]
