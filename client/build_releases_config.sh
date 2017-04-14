#!/bin/zsh

SOLUTION="LiveOakApp"
PROJECT_IOS="iOS"
PROJECT_ANDROID="Droid"
ANDROID_APP_ID="com.liveoakinc.liveoakapp"
IPA_DSYM_NAME="$SOLUTION.$PROJECT_IOS.app.dSYM"
HOCKEYAPP_IOS_ID="4c8529b7834046a694ee90501132cabc"
HOCKEYAPP_IOS_TOKEN="74d232c7a1f04398905c8d4a02f7ea96"

RESULT_NAME_STAGE="avend_staging"
RESULT_NAME_PROD="avend_production"
RESULT_NAME_PROD_APPSTORE="avend_appstore"

XBUILD="xbuild"
MDTOOL="/Applications/Xamarin Studio.app/Contents/MacOS/mdtool"

set -e
set -x
