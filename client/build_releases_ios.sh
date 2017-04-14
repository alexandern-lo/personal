#!/bin/zsh

# Note: iOS builds are located in "ProjectName CurrentDateTime" dir so we look for the most recent .ipa file
IPA="print -lr -- $PROJECT_IOS/bin/iPhone/Release/$SOLUTION.$PROJECT_IOS\ */*.ipa(om[1,1])"
IPA_DSYM_DIR="$PROJECT_IOS/bin/iPhone/Release/"

# Note: iOS builds are located in "ProjectName CurrentDateTime" dir so we look for the most recent .ipa file
IPA_PROD="print -lr -- $PROJECT_IOS/bin/iPhone/ReleaseProduction/$SOLUTION.$PROJECT_IOS\ */*.ipa(om[1,1])"
IPA_DSYM_DIR_PROD="$PROJECT_IOS/bin/iPhone/ReleaseProduction/"
IPA_PROD_APPSTORE="print -lr -- $PROJECT_IOS/bin/iPhone/ReleaseProductionAppStore/$SOLUTION.$PROJECT_IOS\ */*.ipa(om[1,1])"
IPA_DSYM_DIR_PROD_APPSTORE="$PROJECT_IOS/bin/iPhone/ReleaseProductionAppStore/"


"$MDTOOL" build -t:Clean --configuration:"Release|iPhone" $SOLUTION.sln
"$MDTOOL" build -t:Build --configuration:"Release|iPhone" $SOLUTION.sln --project:$SOLUTION.$PROJECT_IOS
cp -p "`eval $IPA`" "$BUILD_DIR/$RESULT_NAME_STAGE.ipa"
(cd $IPA_DSYM_DIR && zip --recurse-paths "$BUILD_DIR/$RESULT_NAME_STAGE.ipa.dSYM.zip" "$IPA_DSYM_NAME")


"$MDTOOL" build -t:Clean --configuration:"ReleaseProduction|iPhone" $SOLUTION.sln
"$MDTOOL" build -t:Build --configuration:"ReleaseProduction|iPhone" $SOLUTION.sln --project:$SOLUTION.$PROJECT_IOS
cp -p "`eval $IPA_PROD`" "$BUILD_DIR/$RESULT_NAME_PROD.ipa"
(cd $IPA_DSYM_DIR_PROD && zip --recurse-paths "$BUILD_DIR/$RESULT_NAME_PROD.ipa.dSYM.zip" "$IPA_DSYM_NAME")


"$MDTOOL" build -t:Clean --configuration:"ReleaseProductionAppStore|iPhone" $SOLUTION.sln
"$MDTOOL" build -t:Build --configuration:"ReleaseProductionAppStore|iPhone" $SOLUTION.sln --project:$SOLUTION.$PROJECT_IOS
cp -p "`eval $IPA_PROD_APPSTORE`" "$BUILD_DIR/$RESULT_NAME_PROD_APPSTORE.ipa"
(cd $IPA_DSYM_DIR_PROD_APPSTORE && zip --recurse-paths "$BUILD_DIR/$RESULT_NAME_PROD_APPSTORE.ipa.dSYM.zip" "$IPA_DSYM_NAME")


# Upload dSYM files to HockeyApp
curl -H X-HockeyAppToken:$HOCKEYAPP_IOS_TOKEN -F "dsym=@$BUILD_DIR/$RESULT_NAME_STAGE.ipa.dSYM.zip" https://rink.hockeyapp.net/api/2/apps/$HOCKEYAPP_IOS_ID/app_versions
curl -H X-HockeyAppToken:$HOCKEYAPP_IOS_TOKEN -F "dsym=@$BUILD_DIR/$RESULT_NAME_PROD.ipa.dSYM.zip" https://rink.hockeyapp.net/api/2/apps/$HOCKEYAPP_IOS_ID/app_versions
curl -H X-HockeyAppToken:$HOCKEYAPP_IOS_TOKEN -F "dsym=@$BUILD_DIR/$RESULT_NAME_PROD_APPSTORE.ipa.dSYM.zip" https://rink.hockeyapp.net/api/2/apps/$HOCKEYAPP_IOS_ID/app_versions
