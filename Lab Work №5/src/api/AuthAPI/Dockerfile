FROM mcr.microsoft.com/dotnet/sdk:7.0 AS aspnet_build
WORKDIR /aspnet_app
COPY ./AuthAPI.csproj .
RUN dotnet restore AuthAPI.csproj
COPY . .
RUN dotnet build "AuthAPI.csproj" -c Release -o /aspnet_app/build
RUN dotnet publish "AuthAPI.csproj" -c Release -o /aspnet_app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS deploy
WORKDIR /app
COPY --from=aspnet_build /aspnet_app/publish .

ENTRYPOINT ["dotnet", "AuthAPI.dll"]