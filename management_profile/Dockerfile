# Imagen base de .NET SDK para compilar
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar csproj y restaurar dependencias
COPY management_profile.csproj ./
RUN dotnet restore

# Copiar todo el proyecto
COPY . .
RUN dotnet publish -c Release -o /app

# Imagen base de runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app


# Instalar librerías necesarias para Npgsql
RUN apt-get update && apt-get install -y libkrb5-3 libgssapi-krb5-2 && rm -rf /var/lib/apt/lists/*


COPY --from=build /app .

EXPOSE 8080
ENTRYPOINT ["dotnet", "management_profile.dll"]

