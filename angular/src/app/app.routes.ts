import { authGuard, permissionGuard } from '@abp/ng.core';
import { Routes } from '@angular/router';

export const APP_ROUTES: Routes = [
  {
    path: '',
    pathMatch: 'full',
    loadComponent: () => import('./home/home.component').then(c => c.HomeComponent),
  },
  {
    path: 'journal-entries',
    canActivate: [authGuard, permissionGuard],
    loadComponent: () => import('./journal-entries/journal-entries').then(c => c.JournalEntries),
    data: { requiredPermission: 'JournalEntries' },
  },

  {
    path: 'chat',
    canActivate: [authGuard],
    loadComponent: () => import('./chat/chat').then(c => c.Chat),
  },
  {
    path: 'mood-tracking',
    canActivate: [authGuard],
    loadComponent: () => import('./mood-tracking/mood-tracking').then(c => c.MoodTracking),
  },
  {
    path: 'recommendations',
    canActivate: [authGuard],
    loadComponent: () => import('./recommendations/recommendations').then(c => c.Recommendations),
  },
  {
    path: 'account',
    loadChildren: () => import('@abp/ng.account').then(c => c.createRoutes()),
  },
  {
    path: 'identity',
    loadChildren: () => import('@abp/ng.identity').then(c => c.createRoutes()),
  },
  {
    path: 'setting-management',
    loadChildren: () => import('@abp/ng.setting-management').then(c => c.createRoutes()),
  },
];