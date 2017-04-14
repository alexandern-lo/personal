const filenameRegex = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;

export const saveFileResponse = (resp) => {
  const { data } = resp;
  const { 'content-disposition': disposition, 'content-type': type } = resp.headers;
  const matches = filenameRegex.exec(disposition);
  const filename = (matches != null && matches[1])
    ? matches[1].replace(/['"]/g, '')
    : '';
  const blob = new Blob([data], { type });
  if (typeof window.navigator.msSaveBlob !== 'undefined') {
    // IE workaround for "HTML7007: One or more blob URLs were
    // revoked by closing the blob for which they were created.
    // These URLs will no longer resolve as the data backing
    // the URL has been freed."
    window.navigator.msSaveBlob(blob, filename);
  } else {
    const fileURL = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = fileURL;
    link.setAttribute('download', filename);
    link.setAttribute('target', '_blank');
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  }
};
