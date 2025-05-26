# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# copy csproj and restore as distinct layers
COPY . .
RUN dotnet restore
RUN dotnet publish -c release -o ../app -r linux-x64 --self-contained false --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/runtime:9.0-bookworm-slim AS runner
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "run"]
