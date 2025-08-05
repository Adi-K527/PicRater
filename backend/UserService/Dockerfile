FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

ARG Jwt__Secret
ARG Db__Connection

ENV Jwt__Secret=$Jwt__Secret
ENV Db__Connection=$Db__Connection

COPY UserService/UserService.csproj ./
RUN dotnet restore UserService/UserService.csproj

COPY UserService/UserService/ ./
RUN dotnet publish UserService/UserService.csproj -c Release -o /app/out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

COPY --from=build /app/out ./

ENTRYPOINT ["sh", "-c", "dotnet UserService/UserService.dll"]