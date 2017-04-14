import { createApiCallAction } from '../helpers/actions';
import buildActions from '../collections/actions_builder';

const collection = buildActions('categories');

export const fetchCategories = collection.fetchItems;
export const pinCategoryById = collection.pinItemById;

export const createCategory = createApiCallAction('categories/CREATE', 'createCategory');
export const updateCategory = createApiCallAction('categories/UPDATE', 'updateCategory');
export const deleteCategories = createApiCallAction('categories/DELETE_ALL', 'deleteCategories');
