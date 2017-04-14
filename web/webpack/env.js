const path = require('path');

const env = process.env.NODE_ENV || 'development';
const server = process.env.LO_SERVER || '';

const devServer = process.env.BABEL_ENV == 'dev_server';

const dev = /^development$/i.test(env);
const pro = /^production$/i.test(env);
const stage = /^stage$/i.test(env);

const globals = {
  'process.env.NODE_ENV': JSON.stringify(env),
  'process.env.LO_SERVER': JSON.stringify(server),
};

const publicPath = '/assets/';
const sourceDir = path.resolve(__dirname, '../source');
const outputDir = path.resolve(__dirname, `../dist.${env}`);

module.exports = {
  env,
  server,
  dev,
  pro,
  stage,
  globals,
  devServer,
  publicPath,
  sourceDir,
  outputDir,
};
