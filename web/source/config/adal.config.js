let tenant;
let clientId;
let host;

const endpoint = 'https://graph.microsoft.com';
const policySignup = 'B2C_1_sign_up';
const policySignin = 'B2C_1_sign_in';
const policyReset = 'b2c_1_reset_pass';
const policyEdit = 'b2c_1_edit';

if (process.env.NODE_ENV === 'production') {
  tenant = 'avend.onmicrosoft.com';
  clientId = '39d6697d-7baa-48b3-a6eb-1dd8c2c83bbe';
  host = 'https://portal.avend.co';
} else if (process.env.NODE_ENV === 'stage') {
  tenant = 'avendstage.onmicrosoft.com';
  clientId = 'c6673a37-206d-49dc-ab56-2050057c1cca';
  host = 'https://avend-stage-web.azurewebsites.net';
} else {
  tenant = 'avenddev.onmicrosoft.com';
  clientId = 'b6c7b614-c44d-4d5b-af7b-b826b54085e2';
  host = 'https://avend-dev-web.azurewebsites.net';
}

if (window.location.hostname.indexOf('localhost') >= 0) {
  host = 'https://localhost:3000';
}

const configTemplate = {
  tenant,
  clientId,
  endpoints: {
    [endpoint]: endpoint,
  },
  disableRenewal: true,
  redirectUri: host,
};

export const adalConfigSignIn = {
  ...configTemplate,
  policy: policySignin,
  params: {
    prompt: 'login',
  },
};

export const adalConfigSignUp = {
  ...configTemplate,
  policy: policySignup,
  redirectUri: `${host}/signed_up`,
  params: {
    prompt: 'login',
  },
};

export const adalConfigResetPassword = {
  ...configTemplate,
  policy: policyReset,
};

export const adalConfigEdit = {
  ...configTemplate,
  policy: policyEdit,
};
