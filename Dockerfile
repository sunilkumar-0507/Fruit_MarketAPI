FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Fruitmarket.API/Fruitmarket.API.csproj", "Fruitmarket.API/"]
COPY ["Fruitmarket.Application/Fruitmarket.Application.csproj", "Fruitmarket.Application/"]
COPY ["Fruitmarket.Infrastructure/Fruitmarket.Infrastructure.csproj", "Fruitmarket.Infrastructure/"]
COPY ["Fruitmarket.Domain/Fruitmarket.Domain.csproj", "Fruitmarket.Domain/"]

RUN dotnet restore "Fruitmarket.API/Fruitmarket.API.csproj"

COPY . .

RUN dotnet publish "Fruitmarket.API/Fruitmarket.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Fruitmarket.API.dll"]
