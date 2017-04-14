import { combineReducers } from 'redux';
import { reducer as form } from 'redux-form';
import { routerReducer as routing } from 'react-router-redux';
import { bindReducers } from './helpers/bind';

import api from './api';
import auth from './auth';
import users from './users';
import events from './events';
import leads from './leads';
import questions from './questions';
import resources from './resources';
import agenda from './agenda';
import categories from './categories';
import attendees from './attendees';
import eventUsers from './event-users';
import crmConfigs from './crm-configs';
import terms from './terms';
import plans from './plans';
import account from './account';
import search from './search';
import home from './home';
import mobile from './mobile-settings';

export default combineReducers(bindReducers({
  routing: () => routing,
  form: () => form,
  api,
  auth,
  users,
  events,
  leads,
  agenda,
  questions,
  resources,
  categories,
  attendees,
  eventUsers,
  crmConfigs,
  terms,
  plans,
  account,
  search,
  home,
  mobile,
}));
