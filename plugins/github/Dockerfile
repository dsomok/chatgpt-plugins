FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ChatGPT.Plugins.Github/ChatGPT.Plugins.Github.csproj", "ChatGPT.Plugins.Github/"]
RUN dotnet restore "ChatGPT.Plugins.Github/ChatGPT.Plugins.Github.csproj"
COPY . .
WORKDIR "/src/ChatGPT.Plugins.Github"
RUN dotnet build "ChatGPT.Plugins.Github.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ChatGPT.Plugins.Github.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
ARG GITHUB_TOKEN
ENV GITHUB__TOKEN=$GITHUB_TOKEN

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChatGPT.Plugins.Github.dll"]