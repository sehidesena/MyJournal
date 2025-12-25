import { Environment } from '@abp/ng.core';

const baseUrl = 'http://localhost:4200';

const oAuthConfig = {
  issuer: 'https://localhost:44376/',
  redirectUri: baseUrl,
  clientId: 'Mentalfull_App',
  responseType: 'code',
  scope: 'offline_access Mentalfull',
  requireHttps: true,
};

export const environment = {
  production: true,
  application: {
    baseUrl,
    name: 'MyJournal',
  },
  oAuthConfig,
  apis: {
    default: {
      url: 'https://localhost:44376',
      rootNamespace: 'Mentalfull',
    },
    AbpAccountPublic: {
      url: oAuthConfig.issuer,
      rootNamespace: 'AbpAccountPublic',
    },
  },
} as Environment;
