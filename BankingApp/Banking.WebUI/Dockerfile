#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.



FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG FEED_PAT
RUN echo ${FEED_PAT}
RUN curl -L https://aka.ms/install-artifacts-credprovider.sh | sh
ENV VSS_NUGET_EXTERNAL_FEED_ENDPOINTS="{\"endpointCredentials\":[{\"endpoint\":\"https://devdiv.pkgs.visualstudio.com/OnlineServices/_packaging/DiagnosticAnalysis-Images/nuget/v3/index.json\",\"username\":\"docker\",\"password\":\"${FEED_PAT}\"}]}"
WORKDIR /src
COPY ["Banking.WebUI/Banking.WebUI.csproj", "Banking.WebUI/"]
COPY ["Banking.WebUI/Nuget.config", "Banking.WebUI/"]
RUN dotnet restore "Banking.WebUI/Banking.WebUI.csproj" --configfile "Banking.WebUI/Nuget.config"
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