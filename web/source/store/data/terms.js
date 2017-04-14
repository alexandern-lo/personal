import typeReader from '../helpers/type_reader';
import typeShape, { required, optional } from '../helpers/type_shape';

export const TermsType = {
  uid: required(String, 'terms_uid'),
  text: required(String),
  outdated: required(Boolean),
  releaseDate: required(String, 'release_date'),
  acceptedDate: optional(String, 'accepted_date'),
};

export const readTerms = typeReader(TermsType);
export const termsShape = typeShape(TermsType);
