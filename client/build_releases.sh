#!/bin/zsh
# ZSH-specific features are used to find resulting .ipa file
source build_releases_config.sh

DATE=`date +%Y-%m-%d_%H-%M-%S`
BUILD_DIR="_builds/$DATE"
mkdir -p "$BUILD_DIR"
BUILD_DIR=(`cd "$BUILD_DIR" && pwd`)

# Note: internet says that iOS codesign needs to unlock keychain, but it works without it
# security unlock-keychain ~/Library/Keychains/login.keychain

IOS=${IOS:=1}
ANDROID=${ANDROID:=1}
if [[ $IOS = "1" ]]; then
	source build_releases_ios.sh
fi
if [[ $ANDROID = "1" ]]; then
	source build_releases_android.sh
fi

echo "==================== Build Finished ===================="
