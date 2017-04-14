import _ from 'lodash';
import { defaultCountry } from 'config/geography';
import typeReader from '../helpers/type_reader';
import typeShape, { required, optional } from '../helpers/type_shape';

const LeadType = {
  uid: required(String, 'lead_uid'),

  firstName: optional(String, 'first_name'),
  lastName: optional(String, 'last_name'),
  companyName: optional(String, 'company_name'),
  companyUrl: optional(String, 'company_url'),
  jobTitle: optional(String, 'job_title'),

  qualification: required(String),  // cold, hot, ...
  notes: String,

  address: String,
  zip: optional(String, 'zip_code'),
  city: String,
  state: String,
  country: String,
  location: optional(String, 'location_string'),

  photoUrl: optional(String, 'photo_url'),
  businessCardFrontUrl: optional(String, 'business_card_front_url'),
  businessCardBackUrl: optional(String, 'business_card_back_url'),

  phones: [{
    lead_phone_uid: required(String),
    phone: required(String),
    designation: required(String),
  }],
  emails: [{
    lead_email_uid: required(String),
    email: required(String),
    designation: required(String),
  }],

  eventId: required(String, 'event.event_uid'),
  event: {
    uid: required(String, 'event_uid'),
    name: required(String),
    questions: [{
      uid: required(String),
      text: required(String),
      answers: [{
        uid: required(String),
        text: required(String),
      }],
    }],
  },

  tenant: {
    uid: required(String, 'subscription_uid'),
    name: required(String),
  },

  ownerId: required(String, 'owner.uid'),
  owner: {
    uid: required(String, 'uid'),
    email: String,
  },

  questionAnswers: {
    from: 'question_answers',
    type: [{
      lead_answer_uid: required(String),
      question_uid: required(String),
      answer_uid: required(String),
    }],
  },

  // "export_statuses": [],

  createdAt: required(String, 'created_at'),
  // "updated_at": "2017-02-20T10:34:04.48"
  // "clientside_updated_at": "2017-02-20T10:34:02",
};

export const readLead = typeReader(LeadType);
export const leadShape = typeShape(LeadType);

export const leadDisplayName = lead => lead && [lead.firstName, lead.lastName].join(' ');

export const phoneDesignations = ['Mobile', 'Work', 'Home'];
export const blankPhone = { phone: '', designation: phoneDesignations[0] };
export const emailDesignations = ['Work', 'Personal', 'Other'];
export const blankEmail = { email: '', designation: emailDesignations[0] };

export const makeBlankLead = () => ({
  phones: [blankPhone],
  emails: [blankEmail],
  country: defaultCountry,
});

const nonEmptyList = (list, blankItem) => (list && list.length > 0 ? list : [blankItem]);

export const editLead = lead => lead && ({
  lead_uid: lead.uid,
  event_uid: lead.eventId,
  first_name: lead.firstName,
  last_name: lead.lastName,
  company_name: lead.companyName,
  company_url: lead.companyUrl,
  job_title: lead.jobTitle,
  qualification: lead.qualification,
  notes: lead.notes,
  address: lead.address,
  zip_code: lead.zip,
  city: lead.city,
  state: lead.state,
  country: lead.country,
  location_string: lead.location,
  photo_url: lead.photoUrl,
  business_card_front_url: lead.businessCardFrontUrl,
  business_card_back_url: lead.businessCardBackUrl,
  phones: nonEmptyList(lead.phones, blankPhone),
  emails: nonEmptyList(lead.emails, blankEmail),
  question_answers: lead.question_answers,
});

export const startEditLeadQuestionnaire = lead => lead && ({
  lead_uid: lead.uid,
  event_uid: lead.eventId,
  question_answers: _.map(lead.event && lead.event.questions, (question) => {
    const answer = _.find(lead.questionAnswers, a => a.question_uid === question.uid) || {
      question_uid: question.uid,
    };
    return { ...answer, question };
  }),
});

const cleanAnswers = (answers) => {
  const clean = _.map(answers, ({ question, ...answer }) => (
    answer.answer_uid && answer.answer_uid.length ? answer : null
  ));
  return _.filter(clean, a => a);
};

export const endEditLeadQuestionnaire = lead => lead && ({
  ...lead,
  question_answers: cleanAnswers(lead.question_answers),
});
