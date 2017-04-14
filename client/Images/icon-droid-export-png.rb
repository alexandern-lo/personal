#!/usr/bin/env ruby

force_generate = ARGV.include? 'force'

require 'fileutils'

CurRoot = File.dirname(__FILE__)

SvgFiles = "#{CurRoot}/icon_droid.svg"

DroidRoot = "#{CurRoot}/../Droid/Resources"

DroidSizes = [
  ["#{DroidRoot}/mipmap-mdpi/Icon.png", 48],
  ["#{DroidRoot}/mipmap-hdpi/Icon.png", 72],
  ["#{DroidRoot}/mipmap-xhdpi/Icon.png", 96],
  ["#{DroidRoot}/mipmap-xxhdpi/Icon.png", 144],
  ["#{DroidRoot}/mipmap-xxxhdpi/Icon.png", 192],
]

Dir.glob(SvgFiles) do |svg|
  DroidSizes.each do |out, size|
    is_modified = force_generate || !File.file?(out) || File.mtime(svg) > File.mtime(out)
    next unless is_modified

    cmd = "rsvg-convert -a -f png --width=#{size} --height=#{size} #{svg} -o #{out}"
    puts cmd

    FileUtils.mkdir_p File.dirname out
    `#{cmd}`
  end
end
