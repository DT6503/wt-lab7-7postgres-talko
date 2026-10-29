FROM mcr.microsoft.com/dotnet/sdk:9.0.319 AS build
WORKDIR /src
COPY *.csproj ./
RUN dotnet restore
COPY . ./
RUN dotnet build  # <--- ДОБАВЬТЕ ЭТУ СТРОКУ
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish ./
EXPOSE 10000
ENTRYPOINT ["dotnet", "wt-lab6-6minimalapi-talko.dll"]