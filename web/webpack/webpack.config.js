const path = require('path');
const webpack = require('webpack');
const LodashPlugin = require('lodash-webpack-plugin');
const ExtractTextPlugin = require('extract-text-webpack-plugin');
const SWPrecacheWebpackPlugin = require('sw-precache-webpack-plugin');

const env = require('./env');
const loaders = require('./loaders');
const assets = require('./assets');
const provide = require('./provide');

const extractCSS = new ExtractTextPlugin({
  filename: env.devServer ? '[name].css' : '[name].[contenthash:8].css',
  allChunks: true,
});

const plugins = [
  new LodashPlugin(),
  new webpack.optimize.OccurrenceOrderPlugin(true),
  new webpack.DefinePlugin(env.globals),
  new webpack.NoEmitOnErrorsPlugin(),
  assets.plugin(),
  new webpack.ProvidePlugin(provide),
];

if (extractCSS) {
  plugins.unshift(extractCSS);
}

if (!env.pro) {
  plugins.unshift(new webpack.NamedModulesPlugin());
}

if (!env.devServer) {
  plugins.push(new SWPrecacheWebpackPlugin({
    cacheId: 'liveoak',
    filepath: path.join(env.outputDir, 'sw.js'),
    directoryIndex: false,
    stripPrefix: env.outputDir,
    staticFileGlobsIgnorePatterns: [/\.map$/],
    minify: env.pro,
    runtimeCaching: [{
      urlPattern: /^https:\/\/fonts.googleapis.com/,
      handler: 'cacheFirst',
    }, {
      urlPattern: /^https:\/\/fonts.gstatic.com/,
      handler: 'cacheFirst',
    }, {
      urlPattern: /^https:\/\/cdn.polyfill.io/,
      handler: 'cacheFirst',
    }],
  }));
}

module.exports = {
  devtool: 'source-map',
  entry: {
    bundle: path.join(env.sourceDir, 'index.js'),
  },
  output: {
    path: path.join(env.outputDir, env.publicPath),
    publicPath: env.publicPath,
    filename: env.devServer ? '[name].js' : '[name].[chunkhash:8].js',
  },
  plugins,
  module: {
    loaders: [].concat(
      loaders.codeLoaders()
    ).concat(
      loaders.cssLoaders({ extractCSS })
    ).concat(
      loaders.assetsLoaders()
    ),
  },
  resolve: {
    modules: [
      'node_modules',
      path.resolve(__dirname, '../source'),
      path.resolve(__dirname, '../src'),
    ],
  },
};
