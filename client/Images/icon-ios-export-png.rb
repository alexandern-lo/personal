#!/usr/bin/env ruby

force_generate = ARGV.include? 'force'

require 'fileutils'

CurRoot = File.dirname(__FILE__)

SvgFiles = "#{CurRoot}/icon_ios.svg"

IosRoot = "#{CurRoot}/../iOS/Resources"

IosSizes = [
  ["#{IosRoot}/Images.xcassets/AppIcons.appiconset/AvendIcon29@2x.png", 58],
  ["#{IosRoot}/Images.xcassets/AppIcons.appiconset/AvendIcon29@3x.png", 87],
  ["#{IosRoot}/Images.xcassets/AppIcons.appiconset/AvendIcon40@2x.png", 80],
  ["#{IosRoot}/Images.xcassets/AppIcons.appiconset/AvendIcon40@3x.png", 120],
  ["#{IosRoot}/Images.xcassets/AppIcons.appiconset/AvendIcon60@2x.png", 120],
  ["#{IosRoot}/Images.xcassets/AppIcons.appiconset/AvendIcon60@3x.png", 180],
]

Dir.glob(SvgFiles) do |svg|
  IosSizes.each do |out, size|
    is_modified = force_generate || !File.file?(out) || File.mtime(svg) > File.mtime(out)
    next unless is_modified

    cmd = "rsvg-convert -a -f png --width=#{size} --height=#{size} #{svg} -o #{out}"
    puts cmd

    FileUtils.mkdir_p File.dirname out
    `#{cmd}`
  end
end
