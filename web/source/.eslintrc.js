const _ = require('lodash')
const provide = require('../webpack/provide');

module.exports = {
  "extends": [
    "airbnb",
  ],
  "globals": _.mapValues(provide, () => false),
  "settings": {
    "import/extensions": ['always', { 'js': 'newer' }],
    "import/resolver": {
      "webpack": {
        "config": './webpack/webpack.config.js',
        "config-index": 1,
      },
    },
  },
  "rules": {
    "semi": 1,
    "strict": 0,
    "jsx-quotes": [2, "prefer-single"],
    "react/jsx-filename-extension": [1, { "extensions": [".js", ".jsx"] }],
    "react/jsx-first-prop-new-line": 0,
    "react/require-default-props": 0,
    "jsx-a11y/no-static-element-interactions": 0,
    "import/prefer-default-export": 0,
    "import/no-extraneous-dependencies": [2, { "devDependencies": true }],
  },
};
