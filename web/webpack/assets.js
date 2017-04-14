const _ = require('lodash');
const fs = require('fs');
const path = require('path');
const pug = require('pug');

const AssetsPlugin = require('assets-webpack-plugin');

const env = require('./env');
const filename = 'assets.json';
const template = fs.readFileSync(path.join(env.sourceDir, 'templates/index.pug'));
const renderHTML = pug.compile(template, { pretty: true });

const plugin = () => new AssetsPlugin({
  filename,
  path: env.outputDir,
  fullPath: true,
  update: true,
  prettyPrint: true,
});

const loadAssets = () => JSON.parse(fs.readFileSync(path.join(env.outputDir, filename)));
const loadStylesheets = () => _.map(loadAssets(), a => a.css);
const loadScripts = () => _.map(loadAssets(), a => a.js);

const stylesheetPath = name => path.join(env.publicPath, name);
const scriptPath = name => path.join(env.publicPath, name);

module.exports = {
  filename,
  plugin,
  renderHTML,
  loadStylesheets,
  loadScripts,
  stylesheetPath,
  scriptPath,
};
