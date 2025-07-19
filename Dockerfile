FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ContactAppNLayer.Api/ContactAppNLayer.Api.csproj", "ContactAppNLayer.Api/"]
COPY ["ContactApp.DataAccess/ContactAppNLayer.DataAccess.csproj", "ContactApp.DataAccess/"]
COPY ["ContactApp.Models/ContactAppNLayer.Models.csproj", "ContactApp.Models/"]
COPY ["ContactApp.Services/ContactAppNLayer.Services.csproj", "ContactApp.Services/"]
RUN dotnet restore "ContactAppNLayer.Api/ContactAppNLayer.Api.csproj"

COPY . .
WORKDIR "/src/ContactAppNLayer.Api"
RUN dotnet build "ContactAppNLayer.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ContactAppNLayer.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ContactAppNLayer.Api.dll"]
