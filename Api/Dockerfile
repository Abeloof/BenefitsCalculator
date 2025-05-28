# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# copy csproj and restore as distinct layers
COPY ../Api .
COPY ../Directory.Packages.props .
RUN dotnet restore
RUN dotnet publish -c release -o ../app --self-contained false --no-restore

# # final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:9.0.5-bookworm-slim AS runner
WORKDIR /app
COPY --from=build /app .
EXPOSE 3000
ENTRYPOINT ["dotnet", "./Api.dll"]
#CMD [ "/bin/bash" ]
