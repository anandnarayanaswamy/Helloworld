FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["Kubernetes/Kubernetes.csproj", "Kubernetes/"]

RUN dotnet restore "Kubernetes/Kubernetes.csproj"
COPY . .
WORKDIR "/src/Kubernetes"
RUN dotnet build "Kubernetes.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Kubernetes.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Kubernetes.dll"]