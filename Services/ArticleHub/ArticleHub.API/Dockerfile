# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

ARG BUILD_CONFIGURATION=Release
ARG APP_NAME=ArticleHub.API
ARG PROJECT_PATH=Services/ArticleHub/${APP_NAME}

WORKDIR /src
COPY ["${PROJECT_PATH}/${APP_NAME}.csproj", "${PROJECT_PATH}/"]
COPY ["BuildingBlocks/Articles.Abstractions/Articles.Abstractions.csproj", "BuildingBlocks/Articles.Abstractions/"]
COPY ["BuildingBlocks/Articles.Security/Articles.Security.csproj", "BuildingBlocks/Articles.Security/"]
COPY ["BuildingBlocks/Blocks.AspNetCore/Blocks.AspNetCore.csproj", "BuildingBlocks/Blocks.AspNetCore/"]
COPY ["BuildingBlocks/Blocks.Core/Blocks.Core.csproj", "BuildingBlocks/Blocks.Core/"]
COPY ["BuildingBlocks/Blocks.Domain/Blocks.Domain.csproj", "BuildingBlocks/Blocks.Domain/"]
COPY ["BuildingBlocks/Blocks.EntityFramework/Blocks.EntityFrameworkCore.csproj", "BuildingBlocks/Blocks.EntityFramework/"]
COPY ["BuildingBlocks/Blocks.Exceptions/Blocks.Exceptions.csproj", "BuildingBlocks/Blocks.Exceptions/"]
COPY ["BuildingBlocks/Blocks.MediatR/Blocks.MediatR.csproj", "BuildingBlocks/Blocks.MediatR/"]
COPY ["BuildingBlocks/Blocks.Messaging/Blocks.Messaging.csproj", "BuildingBlocks/Blocks.Messaging/"]
COPY ["BuildingBlocks/Articles.IntegrationEvents.Contracts/Articles.IntegrationEvents.Contracts.csproj", "BuildingBlocks/Articles.IntegrationEvents.Contracts/"]
COPY ["Services/ArticleHub/ArticleHub.Domain/ArticleHub.Domain.csproj", "Services/ArticleHub/ArticleHub.Domain/"]
COPY ["Services/ArticleHub/ArticleHub.Persistence/ArticleHub.Persistence.csproj", "Services/ArticleHub/ArticleHub.Persistence/"]
RUN dotnet restore "./${PROJECT_PATH}/${APP_NAME}.csproj"
COPY . .
WORKDIR "/src/${PROJECT_PATH}"

# This stage is used to publish the service project to be copied to the final stage
RUN dotnet publish ${APP_NAME}.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ARG APP_NAME=ArticleHub.API
ENV APP_NAME=${APP_NAME}

ENTRYPOINT ["/bin/sh", "-c", "exec dotnet /app/${APP_NAME}.dll"]
