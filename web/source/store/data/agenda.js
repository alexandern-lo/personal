import typeReader from '../helpers/type_reader';
import typeShape, { optional, required } from '../helpers/type_shape';

const AgendaType = {
  uid: required(String, 'agenda_item_uid'),
  name: required(String, 'name'),
  date: required(String, 'date'),
  startTime: required(String, 'start_time'),
  endTime: required(String, 'end_time'),
  description: optional(String, 'description'),
  website: optional(String),
  location: optional(String, 'location'),
  url: optional(String, 'location_url'),
};

export const readAgenda = typeReader(AgendaType);
export const agendaShape = typeShape(AgendaType);

export const editAgenda = agenda => (agenda && {
  uid: agenda.uid,
  name: agenda.name,
  date: agenda.date,
  start_time: agenda.startTime,
  end_time: agenda.endTime,
  description: agenda.description,
  website: agenda.website,
  location: agenda.location,
  location_url: agenda.url,
});
