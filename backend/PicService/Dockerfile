FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

ARG Jwt__Secret
ARG Db__Connection

ENV Jwt__Secret=$Jwt__Secret
ENV Db__Connection=$Db__Connection

COPY PicService.csproj ./
RUN dotnet restore PicService.csproj

COPY PicService/ ./
RUN dotnet publish PicService.csproj -c Release -o /app/out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

COPY --from=build /app/out ./

ENTRYPOINT ["sh", "-c", "dotnet PicService.dll"]