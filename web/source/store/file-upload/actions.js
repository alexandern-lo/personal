import { createApiCallAction } from '../helpers/actions';

export const requestUpload = createApiCallAction('FILE_UPLOAD/request', 'requestFileUpload');
export const uploadFile = createApiCallAction('FILE_UPLOAD/upload', 'uploadFile');
