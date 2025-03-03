FROM mcr.microsoft.com/dotnet/sdk:7.0 AS aspnet_build
WORKDIR /aspnet_app
COPY ./ExampleAPI.csproj .
RUN dotnet restore ExampleAPI.csproj
COPY . .
RUN dotnet build "ExampleAPI.csproj" -c Release -o /aspnet_app/build
RUN dotnet publish "ExampleAPI.csproj" -c Release -o /aspnet_app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS deploy
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=aspnet_build /aspnet_app/publish .

ENTRYPOINT ["dotnet", "ExampleAPI.dll"]