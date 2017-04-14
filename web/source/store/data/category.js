import typeReader from '../helpers/type_reader';
import typeShape, { required } from '../helpers/type_shape';

const CategoryType = {
  uid: required(String, 'category_uid'),
  name: required(String),
  options: [
    {
      uid: required(String, 'option_uid'),
      name: required(String, 'name'),
    },
  ],
};

export const readCategory = typeReader(CategoryType);
export const categoryShape = typeShape(CategoryType);

export const editCategory = category => (category && {
  uid: category.uid,
  name: category.name,
  options: category.options.map(a => ({ option_uid: a.uid, name: a.name })),
});
