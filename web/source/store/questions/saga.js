import {
  callApi,
  dispatch,
  apiCaller,
} from '../helpers/sagas';

import {
  createApiCallResultAction,
} from '../helpers/actions';

import {
  fetchQuestions,
  pinQuestionById,
  moveQuestion,
} from './actions';

const questionsFetched = createApiCallResultAction(fetchQuestions);
const questionMoved = createApiCallResultAction(moveQuestion);

function apiFetcher(api, resultAction) {
  return function* fetcher({ payload }) {
    const params = payload;
    try {
      const result = yield callApi(api, params);
      yield dispatch(resultAction(result));
    } catch (error) {
      yield dispatch(resultAction(error));
    }
  };
}

export default {
  [fetchQuestions]: apiFetcher('fetchQuestions', questionsFetched),
  [pinQuestionById]: apiCaller('fetchQuestionById', createApiCallResultAction(pinQuestionById)),
  [moveQuestion]: apiFetcher('moveQuestion', questionMoved),
};
