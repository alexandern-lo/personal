import { Route, IndexRoute } from 'react-router';

import loginRequired from './scenes/auth/login-required';

import NotFound from './scenes/404/404';

import Layout from './components/layout/layout';
import Home from './scenes/home/home-page';

import LoginPage from './scenes/auth/login-page';
import SelectPlanPage from './scenes/auth/select-plan-page';
import SignupResultPage from './scenes/auth/signup-result-page';
import TermsPage from './scenes/auth/terms-page';
import AcceptInvitePage from './scenes/auth/accept-invite-page';

import CrmConfigsListPage from './scenes/crm-configs/crm-configs-list-page';
import CreateCrmConfigPage from './scenes/crm-configs/crm-config-create-page';
import CrmConfigPinner from './scenes/crm-configs/crm-config-pinner';
import EditCrmConfigPage from './scenes/crm-configs/crm-config-edit-page';

import AccountSettingsPage from './scenes/account-settings/account-settings-page';
import MobileSettingsPage from './scenes/mobile-settings/mobile-settings-page';

import ListLeadsPage from './scenes/leads/leads-list-page';
import CreateLeadPage from './scenes/leads/lead-create-page';
import EditLeadPage from './scenes/leads/lead-edit-page';
import LeadPinner from './scenes/leads/lead-pinner';
import LeadQuestionnairePage from './scenes/leads/lead-questionnaire-page';

import ListResourcesPage from './scenes/resources/resources-list-page';
import CreateResourcePage from './scenes/resources/resource-create-page';
import ResourcePinner from './scenes/resources/resource-pinner';
import EditResourcePage from './scenes/resources/resource-edit-page';

import ListEventsPage from './scenes/events/events-list-page';
import CreateEventPage from './scenes/events/event-create-page';
import EditEventPage from './scenes/events/event-edit-page';
import EventPinner from './scenes/events/event-pinner';

import ListEventCategoriesPage from './scenes/categories/categories-list-page';
import CreateEventCategoryPage from './scenes/categories/category-create-page';
import EditEventCategoryPage from './scenes/categories/category-edit-page';

import ListEventQuestionsPage from './scenes/questions/questions-list-page';
import CreateEventQuestionPage from './scenes/questions/question-create-page';
import EditEventQuestionPage from './scenes/questions/question-edit-page';

import ListAgendaPage from './scenes/agenda/agenda-list-page';
import CreateAgendaPage from './scenes/agenda/agenda-create-page';
import EditAgendaPage from './scenes/agenda/agenda-edit-page';
import AgendaPinner from './scenes/agenda/agenda-pinner';

import AttendeesListPage from './scenes/attendees/attendees-list-page';
import AttendeeCreatePage from './scenes/attendees/attendee-create-page';
import AttendeeEditPage from './scenes/attendees/attendee-edit-page';
import AttendeePinner from './scenes/attendees/attendee-pinner';
import AttendeesImportPage from './scenes/attendees/attendees-import-page';

import ListEventUsersPage from './scenes/event-users/event-users-list-page';
import ListEventUsersInvitePage from './scenes/event-users/event-users-invite-list-page';

import {
  SalesforceResponsePage,
  Dynamics365ResponsePage,
} from './scenes/crm-configs/crm-config-response-page';

import SearchResultsPage from './scenes/search/search-results-page';

import UsersListPage from './scenes/users/users-list-page';
import UserInvitationPage from './scenes/users/user-invitation-page';

import ProfilePage from './scenes/profile/profile-page';

export default (
  <div>
    <Route path='/login' component={LoginPage} />
    <Route path='/signed_up' component={SignupResultPage} />

    <Route path='/accept_invite' component={AcceptInvitePage} />

    <Route path='/terms' component={loginRequired(TermsPage)} />
    <Route path='/select_plan' component={loginRequired(SelectPlanPage)} />

    <Route path='/salesforce_response' component={loginRequired(SalesforceResponsePage)} />
    <Route path='/dynamics365_response' component={loginRequired(Dynamics365ResponsePage)} />

    <Route path='/' component={loginRequired(Layout, { preserveURL: true })}>
      <IndexRoute component={Home} />

      <Route path='crm'>
        <IndexRoute component={CrmConfigsListPage} />
        <Route path='create' component={CreateCrmConfigPage} />
        <Route path=':crm_config_uid' component={CrmConfigPinner}>
          <Route path='edit' component={EditCrmConfigPage} />
        </Route>
      </Route>

      <Route path='account_settings' component={AccountSettingsPage} />
      <Route path='mobile_settings' component={MobileSettingsPage} />

      <Route path='leads'>
        <IndexRoute component={ListLeadsPage} />
        <Route path='create' component={CreateLeadPage} />
        <Route path=':lead_uid' component={LeadPinner}>
          <Route path='edit' component={EditLeadPage} />
          <Route path='questionnaire' component={LeadQuestionnairePage} />
        </Route>
      </Route>

      <Route path='resources'>
        <IndexRoute component={ListResourcesPage} />
        <Route path='create' component={CreateResourcePage} />
        <Route path=':resource_uid' component={ResourcePinner}>
          <Route path='edit' component={EditResourcePage} />
        </Route>
      </Route>

      <Route path='users'>
        <IndexRoute component={UsersListPage} />
        <Route path='invite' component={UserInvitationPage} />
      </Route>

      <Route path='profile' component={ProfilePage} />

      <Route path='events'>
        <IndexRoute component={ListEventsPage} />
        <Route path='create' component={CreateEventPage} />
        <Route path=':event_uid' component={EventPinner}>
          <Route path='edit' component={EditEventPage} />

          <Route path='attendees' component={AttendeesListPage} />
          <Route path='attendees/import' component={AttendeesImportPage} />
          <Route path='attendees/create' component={AttendeeCreatePage} />
          <Route path='attendees/:uid' component={AttendeePinner}>
            <Route path='edit' component={AttendeeEditPage} />
          </Route>

          <Route path='categories' component={ListEventCategoriesPage} />
          <Route path='categories/create' component={CreateEventCategoryPage} />
          <Route path='categories/:uid/edit' component={EditEventCategoryPage} />

          <Route path='questions' component={ListEventQuestionsPage} />
          <Route path='questions/create' component={CreateEventQuestionPage} />
          <Route path='questions/:uid/edit' component={EditEventQuestionPage} />

          <Route path='agenda_items' component={ListAgendaPage} />
          <Route path='agenda_items/create' component={CreateAgendaPage} />
          <Route path='agenda_items/:agenda_uid' component={AgendaPinner}>
            <Route path='edit' component={EditAgendaPage} />
          </Route>

          <Route path='users'>
            <IndexRoute component={ListEventUsersPage} />
            <Route path='invite' component={ListEventUsersInvitePage} />
          </Route>
        </Route>
      </Route>

      <Route path='search' component={SearchResultsPage} />

      <Route path='*' component={NotFound} />
    </Route>
  </div>
);
