import typeReader from '../helpers/type_reader';
import typeShape, { required } from '../helpers/type_shape';

const QuestionType = {
  uid: required(String),
  position: required(Number),
  text: required(String),
  answers: [
    {
      uid: required(String),
      text: required(String),
    },
  ],
};

export const readQuestion = typeReader(QuestionType);
export const questionShape = typeShape(QuestionType);

export const editQuestion = question => (question && {
  uid: question.uid,
  text: question.text,
  answers: question.answers.map(a => ({ text: a.text })),
});
