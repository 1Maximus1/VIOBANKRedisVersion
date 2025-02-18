
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

#RUN dotnet tool install --global dotnet-ef
#ENV PATH="$PATH:/root/.dotnet/tools"

COPY VIOBANK.API/appsettings.json /app/appsettings.json
COPY VIOBANK.API/appsettings.Development.json /app/appsettings.Development.json
#COPY ["VIOBANK.sln", "./"]
#COPY ["VIOBANK.API/VIOBANK.API.csproj", "VIOBANK.API/"]
#COPY ["VIOBANK.Application/VIOBANK.Application.csproj", "VIOBANK.Application/"]
#COPY ["VIOBANK.Domain/VIOBANK.Domain.csproj", "VIOBANK.Domain/"]
#COPY ["VIOBANK.Infrastructure/VIOBANK.Infrastructure.csproj", "VIOBANK.Infrastructure/"]
#COPY ["VIOBANK.PostgresPersistence/VIOBANK.PostgresPersistence.csproj", "VIOBANK.PostgresPersistence/"]
#COPY ["VIOBANK.RedisPersistence/VIOBANK.RedisPersistence.csproj", "VIOBANK.RedisPersistence/"]

COPY ./VIOBANK.API/appsettings.json /app/appsettings.json
COPY ./VIOBANK.API/appsettings.Development.json /app/appsettings.Development.json
COPY ./VIOBANK.sln ./
COPY ./VIOBANK.API/VIOBANK.API.csproj ./VIOBANK.API/
COPY ./VIOBANK.Application/VIOBANK.Application.csproj ./VIOBANK.Application/
COPY ./VIOBANK.Domain/VIOBANK.Domain.csproj ./VIOBANK.Domain/
COPY ./VIOBANK.Infrastructure/VIOBANK.Infrastructure.csproj ./VIOBANK.Infrastructure/
COPY ./VIOBANK.PostgresPersistence/VIOBANK.PostgresPersistence.csproj ./VIOBANK.PostgresPersistence/
COPY ./VIOBANK.RedisPersistence/VIOBANK.RedisPersistence.csproj ./VIOBANK.RedisPersistence/


RUN dotnet restore

COPY . .

RUN dotnet publish "VIOBANK.API/VIOBANK.API.csproj" -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .

#RUN dotnet ef database update --project VIOBANK.PostgresPersistence/VIOBANK.PostgresPersistence.csproj --startup-project VIOBANK.API/VIOBANK.API.csproj
#RUN dotnet ef оновлення бази даних --project VIOBANK.PostgresPersistence/VIOBANK.PostgresPersistence.csproj --startup-проект VIOBANK.API/VIOBANK.API.csproj


ENTRYPOINT ["dotnet", "VIOBANK.API.dll"]
