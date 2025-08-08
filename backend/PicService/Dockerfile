FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

ARG Jwt__Secret
ARG Db__Connection
ARG Distribution
ARG Access__Key
ARG Secret__Key

ENV Jwt__Secret=$Jwt__Secret
ENV Db__Connection=$Db__Connection
ENV Distribution=$Distribution
ENV Access__Key=$Access__Key
ENV Secret__Key=$Secret__Key

COPY . .

RUN dotnet restore PicService.csproj
RUN dotnet publish PicService.csproj -c Release -o /app/out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

COPY --from=build /app/out ./

ENTRYPOINT ["sh", "-c", "dotnet PicService.dll"]