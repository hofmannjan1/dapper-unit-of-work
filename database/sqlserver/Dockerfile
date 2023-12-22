FROM mcr.microsoft.com/mssql/server:2022-latest AS build
ARG SA_PASSWORD="your\$password1"
ENV ACCEPT_EULA=Y \
    MSSQL_SA_PASSWORD=$SA_PASSWORD
RUN /opt/mssql/bin/sqlservr & sleep 30 \
    && /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P $SA_PASSWORD -q "CREATE DATABASE MinimalExample" \
    && pkill sqlservr

FROM mcr.microsoft.com/mssql/server:2022-latest AS release
ENV ACCEPT_EULA=Y
COPY --from=build /var/opt/mssql/data/ /var/opt/mssql/data/
