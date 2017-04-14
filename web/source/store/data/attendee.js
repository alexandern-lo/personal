import _ from 'lodash';
import { defaultCountry } from 'config/geography';
import typeReader from '../helpers/type_reader';
import typeShape, { optional, required } from '../helpers/type_shape';

const AttendeeType = {
  uid: required(String, 'attendee_uid'),
  eventUid: optional(String, 'event_uid'),
  firstName: required(String, 'first_name'),
  lastName: required(String, 'last_name'),
  title: optional(String),
  company: optional(String),
  phone: optional(String),
  email: optional(String),
  avatarUrl: optional(String, 'avatar_url'),
  country: optional(String),
  state: optional(String),
  city: optional(String),
  zipCode: optional(String, 'zip_code'),
  categories: [
    {
      category_uid: optional(String),
      option_uid: optional(String),
      categoryName: optional(String, 'category_name'),
      optionName: optional(String, 'option_name'),
    },
  ],
};

export const readAttendee = typeReader(AttendeeType);
export const attendeeShape = typeShape(AttendeeType);

const prepareCategoryData = ({ category_uid, option_uid }) => {
  if (!Array.isArray(option_uid)) return { category_uid, option_uid };
  return option_uid.map(o => ({ category_uid, option_uid: o }));
};

export const prepareDataForUpload = data => ({
  ...data,
  categories: data.categories.reduce(
    (acc, cat) => acc.concat(prepareCategoryData(cat)),
    [],
  ),
});

export const editAttendee = (attendee, event) => (attendee && event && {
  attendee_uid: attendee.uid,
  first_name: attendee.firstName,
  last_name: attendee.lastName,
  title: attendee.title,
  company: attendee.company,
  phone: attendee.phone,
  email: attendee.email,
  avatar_url: attendee.avatarUrl,
  country: attendee.country,
  state: attendee.state,
  city: attendee.city,
  zip_code: attendee.zipCode,
  categories: _.map(event.categories, ec => ({
    name: ec.name,
    category_uid: ec.category_uid,
    option_uid: _.filter(attendee.categories, ac => ac.category_uid === ec.category_uid)
      .map(c => c.option_uid),
    options: ec.options,
  })),
});

export const makeBlankAttendee = event => ({ ...editAttendee({}, event), country: defaultCountry });
