# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

ARG BUILD_CONFIGURATION=Release
ARG APP_NAME=Auth.API
ARG PROJECT_PATH=Services/Auth/${APP_NAME}

WORKDIR /src/${PROJECT_PATH}
COPY ${PROJECT_PATH}/${APP_NAME}.csproj ./
RUN dotnet restore 
COPY ${PROJECT_PATH}/ ./

# This stage is used to publish the service project to be copied to the final stage
RUN dotnet publish ${APP_NAME}.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ARG APP_NAME
ENV APP_NAME=${APP_NAME}


ENTRYPOINT ["dotnet", "/app/${APP_NAME}.dll"]