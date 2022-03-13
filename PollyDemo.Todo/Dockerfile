FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["PollyDemo.Todo/PollyDemo.Todo.csproj", "PollyDemo.Todo/"]
RUN dotnet restore "PollyDemo.Todo/PollyDemo.Todo.csproj"
COPY . .
WORKDIR "/src/PollyDemo.Todo"
RUN dotnet build "PollyDemo.Todo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PollyDemo.Todo.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PollyDemo.Todo.dll"]
