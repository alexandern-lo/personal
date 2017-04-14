module.exports = {
  extends: [
    'stylelint-config-standard',
    'stylelint-config-css-modules',
  ],
  rules: {
    'rule-empty-line-before': ['always', {
      ignore: ['after-comment', 'inside-block'],
    }],
    'at-rule-empty-line-before': ['always', {
      ignoreAtRules: ['import', 'apply'],
    }],
    'custom-property-empty-line-before': 'never',
    'color-hex-case': 'upper',
  },
};
