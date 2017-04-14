const fs = require('fs');
const https = require('https');
const http = require('http');
const express = require('express');
const webpack = require('webpack');
const webpackDevMiddleware = require('webpack-dev-middleware');
const webpackHotMiddleware = require('webpack-hot-middleware');

const config = require('./webpack/webpack.config');
const assets = require('./webpack/assets');

const app = express();
const httpsPort = 3000;
const httpPort = 3001;

app.use(express.static('static'));

const html = assets.renderHTML({
  devServer: true,
  stylesheets: [
    assets.stylesheetPath('bundle.css'),
  ],
  scripts: [
    assets.scriptPath('bundle.js'),
  ],
})

config.plugins.unshift(new webpack.HotModuleReplacementPlugin());
config.entry.bundle = [
  'react-hot-loader/patch',
  'webpack-hot-middleware/client',
  config.entry.bundle,
];


const privateKey  = fs.readFileSync('./test-certs/key.pem', 'utf8');
const certificate = fs.readFileSync('./test-certs/cert.pem', 'utf8');
const credentials = { key: privateKey, cert: certificate };

const compiler = webpack(config);
app.use(webpackDevMiddleware(compiler, {
  publicPath: config.output.publicPath,
  stats: {
    hash: false,
    assets: false,
    chunks: false,
    modules: false,
    timings: false,
    version: false,
    errorDetails: false,
  },
}));
app.use(webpackHotMiddleware(compiler));

app.get("*", function(req, res) {
  res.set('Content-Type', 'text/html');
  res.send(html);
});

const httpsServer = https.createServer(credentials, app);
httpsServer.listen(httpsPort);

const httpServer = http.createServer(app);
httpServer.listen(httpPort);
