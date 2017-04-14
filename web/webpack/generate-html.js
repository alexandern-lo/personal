const fs = require('fs');
const path = require('path');
const readline = require('readline');

const env = require('./env');
const assets = require('./assets');

const filename = path.join(env.outputDir, 'index.html');

const rl = readline.createInterface({
  input: process.stdin,
  output: process.stdout,
  terminal: false
});

rl.on('line', (line) => {
  const html = assets.renderHTML({
    version: line,
    stylesheets: assets.loadStylesheets(),
    scripts: assets.loadScripts(),
  });
  fs.writeFileSync(filename, html);
  rl.close();
});
