FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

ARG Jwt__Secret
ARG Db__Connection
ARG Service_Name

ENV Jwt__Secret=$Jwt__Secret
ENV Db__Connection=$Db__Connection

COPY ${Service_Name}/${Service_Name}.csproj ./
RUN dotnet restore ${Service_Name}.csproj

COPY ${Service_Name}/ ./
RUN dotnet publish ${Service_Name}.csproj -c Release -o /app/out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

COPY --from=build /app/out ./

ENTRYPOINT dotnet ${Service_Name}.dll