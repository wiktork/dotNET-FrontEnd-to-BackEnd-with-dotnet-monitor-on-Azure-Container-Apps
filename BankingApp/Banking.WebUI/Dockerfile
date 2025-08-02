#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.



FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 443
ENV DOTNET_EnableDiagnostics=0

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG FEED_PAT
RUN curl -L https://aka.ms/install-artifacts-credprovider.sh | sh
ENV VSS_NUGET_EXTERNAL_FEED_ENDPOINTS="{\"endpointCredentials\":[{\"endpoint\":\"https://devdiv.pkgs.visualstudio.com/OnlineServices/_packaging/DiagnosticAnalysis-Images/nuget/v3/index.json\",\"username\":\"docker\",\"password\":\"${FEED_PAT}\"}]}"
WORKDIR /src
COPY ["Banking.WebUI/Banking.WebUI.csproj", "Banking.WebUI/"]
COPY ["Banking.WebUI/Nuget.config", "Banking.WebUI/"]
RUN dotnet restore "Banking.WebUI/Banking.WebUI.csproj" --configfile "Banking.WebUI/Nuget.config"
COPY . .
WORKDIR /diag/cli
RUN ls -l /root/.nuget/packages
RUN cp -r /root/.nuget/packages/microsoft.visualstudio.diagnosticanalysiscli.linux.x64/2.0.3095.201/content/net8.0/* .
COPY "Banking.WebUI/analyze.sh" /diag/cli/analyze.sh

WORKDIR "/src/Banking.WebUI"
RUN dotnet build "Banking.WebUI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Banking.WebUI.csproj" -c Release -o /app/publish

FROM base AS final
EXPOSE 8080
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=publish /diag/cli /diag/cli

# TESTONLY We will bring in procdump here right away.
# The more correct long term approach for ACA:
# Use a sidecar image instead of exec (i.e. debug)
# Download procdump in the side car
# Transfer procdump to the main container via procfs. 
# exec procdump in the main container

RUN apt-get update
RUN apt-get install -y wget
RUN wget -q https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
RUN dpkg -i packages-microsoft-prod.deb
RUN apt-get update
RUN apt-get install -y apt-transport-https
RUN apt-get update
RUN apt-get install -y procdump
#/proc/net/unix is populated by non-1 domain sockets that are stale. How do we get rid of these?
ENV DOTNET_EnableDiagnostics=1 
# END TEST ONLY

ENTRYPOINT ["dotnet", "Banking.WebUI.dll"]