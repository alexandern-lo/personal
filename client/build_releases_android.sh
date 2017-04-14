#!/bin/zsh

APK="$PROJECT_ANDROID/bin/Release/$ANDROID_APP_ID-Signed.apk"

APK_PROD="$PROJECT_ANDROID/bin/ReleaseProduction/$ANDROID_APP_ID-Signed.apk"


# Note: xbuild is the preferred method for android
"$XBUILD" $PROJECT_ANDROID/$SOLUTION.$PROJECT_ANDROID.csproj /p:Configuration=Release /t:Clean
"$XBUILD" $PROJECT_ANDROID/$SOLUTION.$PROJECT_ANDROID.csproj /p:Configuration=Release /t:SignAndroidPackage
cp -p "$APK" "$BUILD_DIR/$RESULT_NAME_STAGE.apk"

# Note: xbuild don't know about configuration settings in .sln so it uses the same configuration
# that's why we use mdtool for custom configuration
# Note: mdtool archive always throws System.NullReferenceException after successful build
"$MDTOOL" build -t:Clean --configuration:"ReleaseProduction" $SOLUTION.sln
"$MDTOOL" build --configuration:"ReleaseProduction" $SOLUTION.sln --project:$SOLUTION.$PROJECT_ANDROID
"$MDTOOL" archive --configuration:"ReleaseProduction" $SOLUTION.sln --project:$SOLUTION.$PROJECT_ANDROID || true
cp -p "$APK_PROD" "$BUILD_DIR/$RESULT_NAME_PROD.apk"
