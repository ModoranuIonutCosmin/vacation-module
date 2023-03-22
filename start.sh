#!/bin/sh
cd /

/opt/mssql-tools/bin/sqlcmd -S vacationsmodule.sqldb -U sa -P 'Sarandompassword1' -Q "CREATE DATABASE ReservationsDB;"

dotnet ef database update --project /src/VacationsModule.WebApi

dotnet /app/publish/VacationsModule.WebApi.dll



#sursele pentru ef database update

WORKDIR /
COPY --from=publish /start.sh .
COPY ./src /src

#update la db & run
RUN chmod +x /start.sh


# #### De comentat in prod
# RUN dotnet tool install --global dotnet-ef
# ENV PATH="$PATH:/root/.dotnet/tools"
# RUN dotnet ef
# ## install sqlcmd
# RUN apt-get update
# RUN apt-get upgrade
# RUN apt-get install sudo
# RUN sudo apt-get -y install curl
# RUN sudo apt-get -y install gnupg
# RUN curl https://packages.microsoft.com/keys/microsoft.asc	| sudo apt-key add -
# RUN curl https://packages.microsoft.com/config/ubuntu/20.04/prod.list | sudo tee /etc/apt/sources.list.d/msprod.list
# RUN sudo apt-get update
# RUN sudo ACCEPT_EULA=Y apt-get -y install mssql-tools unixodbc-dev
# RUN echo 'export PATH="$PATH:/opt/mssql-tools/bin"' >> ~/.bash_profile
# #### De comentat in prod
