# Etapa de compilación - USAMOS SDK 9.0
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiamos el csproj
COPY ControlFarmacia.API.csproj .
RUN dotnet restore "ControlFarmacia.API.csproj"

# Copiamos todo lo demás
COPY . .
RUN dotnet publish "ControlFarmacia.API.csproj" -c Release -o /app/publish

# Etapa de ejecución - USAMOS ASPNET 9.0
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

ENTRYPOINT ["dotnet", "ControlFarmacia.API.dll"]