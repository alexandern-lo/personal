import { createFetchAction, createApiCallAction } from '../helpers/actions';

export const searchQuestions = createApiCallAction('questions/SEARCH', 'fetchQuestions');

export const fetchQuestions = createFetchAction('questions/FETCH');
export const pinQuestionById = createFetchAction('questions/PIN_BY_ID');
export const createQuestion = createApiCallAction('questions/CREATE', 'createQuestion');
export const updateQuestion = createApiCallAction('questions/UPDATE', 'updateQuestion');
export const deleteQuestions = createApiCallAction('questions/DELETE', 'deleteQuestions');
export const moveQuestion = createFetchAction('questions/MOVE');
