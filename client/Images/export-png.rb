#!/usr/bin/env ruby

force_generate = ARGV.include? 'force'

require 'fileutils'

Density = 0.5

CurRoot = File.dirname(__FILE__)

SvgFiles = "#{CurRoot}/svg/*.svg"

IosRoot = "#{CurRoot}/../iOS/Resources"
DroidRoot = "#{CurRoot}/../Droid/Resources"

IosDensities = [
  ["#{IosRoot}/{FILE}.png", Density],
  ["#{IosRoot}/{FILE}@2x.png", Density * 2],
  ["#{IosRoot}/{FILE}@3x.png", Density * 3],
]

DroidDensities = [
  #  ["#{DroidRoot}/drawable/{FILE}.png", Density], # was density * 1
  # ["#{DroidRoot}/drawable-ldpi/{FILE}.png", Density * 0.75],
  # ["#{DroidRoot}/drawable-mdpi/{FILE}.png", Density],
  # ["#{DroidRoot}/drawable-hdpi/{FILE}.png", Density * 1.5],
  ["#{DroidRoot}/drawable-xhdpi/{FILE}.png", Density * 2 ],
  ["#{DroidRoot}/drawable-xxhdpi/{FILE}.png", Density * 3 ],
]

Dir.glob(SvgFiles) do |svg|
  (IosDensities.concat DroidDensities).each do |out, density|
    out = out.gsub('{FILE}', File.basename(svg, '.*'))

    is_modified = force_generate || !File.file?(out) || File.mtime(svg) > File.mtime(out)
    next unless is_modified

    cmd = "rsvg-convert -a -f png --zoom=#{density} #{svg} -o #{out}"
    puts cmd

    FileUtils.mkdir_p File.dirname out
    `#{cmd}`
  end
end
