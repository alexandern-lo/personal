import React from 'react';
import { render } from 'react-dom';
import { browserHistory, Router } from 'react-router';
import { syncHistoryWithStore } from 'react-router-redux';
import { Provider as ReduxProvider } from 'react-redux';
import AppContainer from 'react-hot-loader/lib/AppContainer';

import initialRoutes from './routes';
import configureStore from './store/configureStore';

import './styles/reset.css';
import '../src/css/main.styl';

// const PRODUCTION = process.env.NODE_ENV === 'production';
// eslint-disable-next-line no-underscore-dangle
const devToolsExtension = window.__REDUX_DEVTOOLS_EXTENSION_COMPOSE__;
const compose = devToolsExtension ? devToolsExtension({ name: 'AVEND' }) : undefined;

const store = configureStore({}, {
  compose,
  history: browserHistory,
});

const history = syncHistoryWithStore(browserHistory, store);

const renderApp = (routes) => {
  const vdom = (
    <ReduxProvider store={store}>
      <AppContainer>
        <Router history={history} routes={routes} />
      </AppContainer>
    </ReduxProvider>
  );
  render(vdom, document.getElementById('root'));
};

renderApp(initialRoutes);

if (module.hot) {
  module.hot.accept('./routes', () => {
    renderApp(require('./routes').default); // eslint-disable-line global-require
  });
}
