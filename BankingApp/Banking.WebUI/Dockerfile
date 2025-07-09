#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.


#ARG FEED_PAT
#ENV VSS_NUGET_EXTERNAL_FEED_ENDPOINTS="{\"endpointCredentials\":[{\"endpoint\":\"https://pkgs.dev.azure.com/your-org/_packaging/your-feed/nuget/v3/index.json\",\"username\":\"docker\",\"password\":\"${FEED_PAT}\"}]}"

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Banking.WebUI/Banking.WebUI.csproj", "Banking.WebUI/"]
RUN dotnet restore "Banking.WebUI/Banking.WebUI.csproj"
COPY . .
WORKDIR "/src/Banking.WebUI"
RUN dotnet build "Banking.WebUI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Banking.WebUI.csproj" -c Release -o /app/publish

FROM base AS final
EXPOSE 8080
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Banking.WebUI.dll"]