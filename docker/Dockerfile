FROM ubuntu:impish as BASE

ENV DEBIAN_FRONTEND=noninteractive

ARG BUILD_DEST="/opt/backend"
ARG BACKEND_DEST="backend"
ARG SRC_DEST="/root/backend-src"
ARG BACKEND_ZIP_FILE="backend.zip"

WORKDIR /root

RUN mkdir ${BUILD_DEST}

RUN apt-get update && apt-get install -y --no-install-recommends git wget curl vim gnupg2 \
ca-certificates apt-transport-https lsb-release unzip

RUN wget https://packages.microsoft.com/config/ubuntu/21.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb && \
dpkg -i packages-microsoft-prod.deb && rm packages-microsoft-prod.deb

RUN apt-get update && apt-get install -y --no-install-recommends dotnet-sdk-3.1

COPY / ${SRC_DEST}

WORKDIR ${SRC_DEST}

RUN dotnet build -c Release && dotnet publish -c Release -o ${BUILD_DEST} 

FROM ubuntu:impish as PRODUCTION
ARG BUILD_DEST="/opt/backend"
COPY --from=BASE ${BUILD_DEST} /opt/backend/

COPY docker/start-service.sh /usr/local/bin/start-service.sh

RUN apt-get update && apt-get install -y --no-install-recommends wget apt-transport-https ca-certificates

RUN wget https://packages.microsoft.com/config/ubuntu/21.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb && \
dpkg -i packages-microsoft-prod.deb && rm packages-microsoft-prod.deb

RUN apt-get update && apt-get install -y --no-install-recommends aspnetcore-runtime-3.1

ENTRYPOINT ["start-service.sh"]

