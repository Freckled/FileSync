FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /app

COPY FileSync.csproj .
RUN dotnet restore FileSync.csproj

COPY . .
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

EXPOSE 42069/tcp
EXPOSE 42068/tcp
ENTRYPOINT ["dotnet", "FileSync.dll"]
CMD [ "server" ]