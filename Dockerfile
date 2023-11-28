FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

EXPOSE 80 
EXPOSE 5001

#COPY PROJECT FILES
COPY ../Permissions.Domain /app/Permissions.Domain
COPY ../Permissions.Infrastructure.SQLServer /app/Permissions.Infrastructure.SQLServer

#COPY RESTO
COPY . .
RUN dotnet publish -c Release -o out

#Build image
FROM mcr.microsoft.com/dotnet/sdk:6.0
WORKDIR /webapp
COPY --from=build /app/out /webapp
ENTRYPOINT ["dotnet", "Permissions.App.dll"]