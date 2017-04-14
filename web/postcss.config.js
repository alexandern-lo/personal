const path = require('path');
const autoprefixer = require('autoprefixer');
const stylelint = require('stylelint');
const reporter = require('postcss-reporter');

const cssImport = require('postcss-import');
const nesting = require('postcss-nesting');
const colorFunction = require('postcss-color-function');
const customProperties = require("postcss-custom-properties")
const apply = require('postcss-apply');


const stylelintConfig = require('./stylelint.config.js');

const sourceDir = path.resolve(__dirname, 'source');

module.exports = {
  plugins: [
    stylelint(stylelintConfig),
    cssImport({
      path: sourceDir,
    }),
    customProperties(),
    colorFunction(),
    apply(),
    nesting(),
    autoprefixer,
    reporter({
      clearMessages: true,
    }),
  ],
};
