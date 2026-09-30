# ==============================================================================
# Dockerfile - Sistema LucroImpresso (.NET 9 / Blazor Server)
# Portabilidade e Execução Universal Multiplataforma (Windows, Linux, macOS)
# ==============================================================================

# Etapa 1: Build e Compilação
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar csproj e restaurar dependências
COPY ["src/LucroImpresso.Web/LucroImpresso.Web.csproj", "src/LucroImpresso.Web/"]
RUN dotnet restore "src/LucroImpresso.Web/LucroImpresso.Web.csproj"

# Copiar código-fonte completo e publicar bundle de produção
COPY ["src/LucroImpresso.Web/", "src/LucroImpresso.Web/"]
WORKDIR "/src/src/LucroImpresso.Web"
RUN dotnet publish "LucroImpresso.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa 2: Runtime Minimalista de Produção (ASP.NET Core 9)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Configuração de portas e variáveis de ambiente
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Copiar artefatos compilados da etapa de build
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "LucroImpresso.Web.dll"]
