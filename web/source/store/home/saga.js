import {
  callApi,
  fromStore,
  dispatch,
} from '../helpers/sagas';

import { createApiCallResultAction } from '../helpers/actions';

import { fetchUsersLeadGoals, fetchUsersExpenses } from './actions';
import { getTotalLeadsByUserEvent, getTotalExpensesByUserEvent } from './index';

const usersLeadGoals = createApiCallResultAction(fetchUsersLeadGoals);
const usersExpenses = createApiCallResultAction(fetchUsersExpenses);

function* usersLeadGoalsFetcher() {
  try {
    const event = yield fromStore(getTotalLeadsByUserEvent);
    const eventUids = event && [event.uid];
    const result = yield callApi(
      'getUsersLeadGoals',
      { eventUids },
    );
    yield dispatch(usersLeadGoals(result));
  } catch (error) {
    yield dispatch(usersLeadGoals(error));
  }
}

function* usersExpensesFetcher() {
  try {
    const event = yield fromStore(getTotalExpensesByUserEvent);
    const eventUids = event && [event.uid];
    const result = yield callApi(
      'getUsersExpenses',
      { eventUids },
    );
    yield dispatch(usersExpenses(result));
  } catch (error) {
    yield dispatch(usersExpenses(error));
  }
}

export default {
  [fetchUsersLeadGoals]: usersLeadGoalsFetcher,
  [fetchUsersExpenses]: usersExpensesFetcher,
};
