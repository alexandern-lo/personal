#!/bin/bash
ENV=$1
if [ -z "$1" ]; then
  ENV=development
fi

echo $ENV

case $ENV in
  "stage")
    FTP="waws-prod-bay-065.ftp.azurewebsites.windows.net"
    FTP_USER="avend-stage-web\\avend-deployer"
    FTP_PASS="qweASD123"
    ;;
  "development")
    FTP="waws-prod-bay-065.ftp.azurewebsites.windows.net"
    FTP_USER="avend-dev-web\\avend-deployer"
    FTP_PASS="qweASD123"
    ;;
  "production")
    FTP="waws-prod-bay-065.ftp.azurewebsites.windows.net"
    FTP_USER="avend\\avend-deployer"
    FTP_PASS="qweASD123"
    ;;
esac

ROOT=/site/wwwroot

npm run build-$ENV || exit 1

ncftpput -R -u $FTP_USER -p $FTP_PASS $FTP $ROOT azure
ncftpput -R -u $FTP_USER -p $FTP_PASS $FTP $ROOT dist.$ENV/*
