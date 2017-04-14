import _ from 'lodash';

import {
  handleActions,
  isApiResultAction,
} from '../helpers/actions';

import {
  readQuestion,
} from '../data/question';

import {
  fetchQuestions,
  pinQuestionById,
  moveQuestion,
} from './actions';

const defaultState = {
  loading: false,
  questions: [],
  pinnedQuestion: null,
  pinnedError: null,
};

export const isLoading = ({ loading }) => loading;
export const getError = ({ error }) => error;
export const getQuestions = ({ questions }) => questions;

export const getPinnedQuestion = ({ pinnedQuestion }) => pinnedQuestion;
export const getPinnedError = ({ pinnedError }) => pinnedError;

const onFetchQuestions = (state, action) => {
  if (isApiResultAction(action)) {
    const { error, payload } = action;
    if (error) {
      return { ...state, loading: false, error: payload };
    }
    const questions = payload.data.map(readQuestion);
    return {
      ...state,
      questions,
      loading: false,
      error: null,
    };
  }
  return { ...state, loading: true };
};

const onPinQuestion = (state, action) => {
  if (isApiResultAction(action)) {
    const { error, payload } = action;
    if (error) {
      return { ...state, pinnedQuestion: null, pinnedError: payload };
    }
    const question = readQuestion(payload.data);
    return { ...state, pinnedQuestion: question, pinnedError: null };
  }
  const { payload: uid } = action;
  const pinned = getPinnedQuestion(state);
  const question = pinned && pinned.uid === uid
    ? pinned
    : _.find(getQuestions(state), q => q.uid === uid);
  return { ...state, pinnedQuestion: question, pinnedError: null };
};

export default handleActions({
  [fetchQuestions]: onFetchQuestions,
  [moveQuestion]: onFetchQuestions,
  [pinQuestionById]: onPinQuestion,
}, defaultState);
