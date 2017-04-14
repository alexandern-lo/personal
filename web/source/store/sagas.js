import bindSagas from './helpers/bindSagas';

import users from './users/saga';
import events from './events/saga';
import leads from './leads/saga';
import agenda from './agenda/saga';
import questions from './questions/saga';
import resources from './resources/saga';
import categories from './categories/saga';
import attendees from './attendees/saga';
import eventUsers from './event-users/saga';
import auth from './auth/saga';
import crmConfigs from './crm-configs/saga';
import terms from './terms/saga';
import plans from './plans/saga';
import search from './search/saga';
import home from './home/saga';
import mobile from './mobile-settings/saga';

export default bindSagas({
  users,
  events,
  leads,
  agenda,
  questions,
  resources,
  categories,
  attendees,
  eventUsers,
  auth,
  crmConfigs,
  terms,
  plans,
  search,
  home,
  mobile,
});
