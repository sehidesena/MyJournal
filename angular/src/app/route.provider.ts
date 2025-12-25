import { RoutesService, eLayoutType } from '@abp/ng.core';
import { provideAppInitializer, inject } from '@angular/core';

export const APP_ROUTE_PROVIDER = [
  provideAppInitializer(() => {
    configureRoutes();
  }),
];

function configureRoutes() {
  const routes = inject(RoutesService);
  routes.add([
    {
      path: '/',
      name: '::Menu:Home',
      iconClass: 'fas fa-home',
      order: 1,
      layout: eLayoutType.application,
    },
    {
      path: 'journal-entries',
      name: '::Menu:JournalEntries',
      iconClass: 'fas fa-book',
      order: 2,
      layout: eLayoutType.application,
    },

    {
      path: 'chat',
      name: '::Menu:Chat',
      iconClass: 'fas fa-comments',
      order: 8,
      layout: eLayoutType.application,
    },
    {
      path: 'mood-tracking',
      name: '::Menu:MoodTracker',
      iconClass: 'fas fa-smile',
      order: 9,
      layout: eLayoutType.application,
    },
    {
      path: 'recommendations',
      name: '::Menu:Recommendations',
      iconClass: 'fas fa-lightbulb',
      order: 10,
      layout: eLayoutType.application,
    }

  ]);
}
