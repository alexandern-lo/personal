# README #

### How do I get set up? ###

* Install Xamarin with Indie or better license
* Install librsvg:
```
#!bash
brew install librsvg
brew link librsvg
```
* Run `./Images/icon-droid-export-png.rb && ./Images/icon-ios-export-png.rb && ./Images/export-png.rb` to generate PNG images from SVG
* Build

###For Android
* Clone https://github.com/studiomobile/StickyListHeaders/tree/xamarin-bindings repository and follow instructions from README to build .dll.
* Put this .dll to Libs folder
