import {
  createStore,
  applyMiddleware,
  compose as reduxCompose,
} from 'redux';

import { routerMiddleware } from 'react-router-redux';
import createSagaMiddleware, { END as END_SAGAS } from 'redux-saga';

import rootReducer from './reducers';
import sagas from './sagas';

import Api from './api/api';
import apiMiddleware from './api/middleware';
import { internalSetupApi } from './api/actions';

export default function configureStore(initialState, {
  compose = reduxCompose,
  history,
}) {
  const sagaMiddleware = createSagaMiddleware();
  const middlewares = [
    apiMiddleware,
    sagaMiddleware,
  ];
  if (history) {
    middlewares.unshift(routerMiddleware(history));
  }

  const store = createStore(
    rootReducer,
    initialState,
    compose(
      applyMiddleware(...middlewares),
    ),
  );

  const setupApi = (ApiClass) => {
    store.dispatch(internalSetupApi(new ApiClass()));
  };

  let sagaTasks = [];
  const runSagas = (newSagas) => {
    sagaTasks = Object.values(newSagas).map(sagaMiddleware.run);
  };

  store.endSagas = function endSagas() {
    store.dispatch(END_SAGAS);
    const tasks = sagaTasks.filter(task => task.isRunning());
    return Promise.all(tasks.map(task => task.done));
  };

  setupApi(Api);
  runSagas(sagas);

  if (module.hot) {
    module.hot.accept('./reducers', () => {
      store.replaceReducer(require('./reducers').default); // eslint-disable-line global-require
    });
    module.hot.accept('./sagas', () => {
      store.dispatch(END_SAGAS);
      runSagas(require('./sagas').default); // eslint-disable-line global-require
    });
    module.hot.accept('./api/api', () => {
      setupApi(require('./api/api').default); // eslint-disable-line global-require
    });
  }

  return store;
}
