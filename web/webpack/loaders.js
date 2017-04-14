const fs = require('fs');
const path = require('path');
const env = require('./env');

const eslintConfig = require('../.eslintrc');

const babel = JSON.parse(fs.readFileSync(path.resolve(__dirname, '../.babelrc')));

// configure es2015 preset to support webpack tree shaking
const es2015 = babel.presets.find(preset => /es2015/.test(preset));
const es2015Webpack = ['es2015', { modules: false }];
babel.presets.splice(babel.presets.indexOf(es2015), 1, es2015Webpack);

if (env.pro) {
  babel.plugins.push(
    'transform-react-remove-prop-types',
    'transform-remove-console'
  );
} else {
  babel.plugins.push(
    'transform-react-jsx-self'
  );
}

module.exports.codeLoaders = () => ([
  {
    test: /\.js$/,
    loader: 'eslint-loader',
    exclude: /node_modules/,
    enforce: 'pre',
    query: eslintConfig,
  },
  {
    test: /\.js$/,
    loader: 'babel-loader',
    exclude: /node_modules/,
    query: babel,
  },
]);

const assetLoader = (ext, loader, prefix, limit, inline, mimetype) => {
  const name = prefix + (env.devServer ? '[name].[ext]' : '[name].[hash:8].[ext]');
  if (inline === false) limit = 1; // eslint-disable-line no-param-reassign
  return {
    test: ext.exec ? ext : new RegExp(`\.${ext}$`),
    loader: loader || 'url-loader',
    query: { name, limit, mimetype },
  };
};

const imageLoader = (ext, { loader, mimetype, limit = 4 * 1024, inline } = {}) =>
  assetLoader(ext, loader, 'images/', limit, inline, mimetype);

const fontLoader = (ext, { loader, mimetype, limit = 16 * 1024, inline } = {}) =>
  assetLoader(ext, loader, 'fonts/', limit, inline, mimetype);

module.exports.assetsLoaders = () => ([
  imageLoader('svg', { loader: 'svg-url-loader', mimetype: 'image/svg+xml', limit: 1024 }),
  imageLoader('png', { mimetype: 'image/png' }),
  imageLoader('jpg', { mimetype: 'image/jpeg' }),
  imageLoader('jpeg', { mimetype: 'image/jpeg' }),
  fontLoader('eot', { mimetype: 'application/vnd.ms-fontobject' }),
  fontLoader('ttf', { mimetype: 'application/octet-stream' }),
  fontLoader('otf', { mimetype: 'application/octet-stream' }),
  fontLoader('woff', { mimetype: 'application/font-woff' }),
  fontLoader('woff2', { mimetype: 'application/font-woff2' }),
]);


const cssExtractor = (extractCSS, { withModules, postcss, loaders } = {}) => {
  const loaderChain = (postcss ? ['postcss-loader'] : []).concat(loaders || []);
  const cssLoader = {
    loader: 'css-loader',
    query: {
      sourceMap: true,
      importLoaders: loaders && loaderChain.length,
      modules: withModules,
      camelCase: withModules,
      localIdentName: env.dev ? '[local]--[name]' : '[hash:6]',
      minimize: env.pro,
    },
  };
  loaderChain.unshift(cssLoader);
  return extractCSS ? extractCSS.extract({
    fallback: 'style-loader',
    use: loaderChain,
  }) : ['style-loader'].concat(loaderChain);
}

module.exports.cssLoaders = ({ extractCSS } = {}) => ([
  {
    test: /^(?!.*modules?\.css$).+\.css$/,
    loader: cssExtractor(extractCSS, {
    }),
  },
  {
    test: /\.modules?\.css$/,
    loader: cssExtractor(extractCSS, {
      postcss: true,
      withModules: true,
    }),
  },
  {
    test: /\.scss$/,
    loader: cssExtractor(extractCSS, {
      loaders: [
        'sass-loader?sourceMap',
      ],
    }),
  },
  {
    test: /\.styl$/,
    loader: cssExtractor(extractCSS, {
      loaders: [
        'stylus-loader?sourceMap&include css',
      ],
    }),
  },
]);
