import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';
import type { CreateMoodLogDto, MoodLogDto } from '../dtos/mood-tracking/models';

@Injectable({
  providedIn: 'root',
})
export class MoodTrackingService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateMoodLogDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, MoodLogDto>({
      method: 'POST',
      url: '/api/app/mood-tracking',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  getEmotionStats = (startDate: string, endDate: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, Record<string, number>>({
      method: 'GET',
      url: '/api/app/mood-tracking/emotion-stats',
      params: { startDate, endDate },
    },
    { apiName: this.apiName,...config });
  

  getHistory = (startDate: string, endDate: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, MoodLogDto[]>({
      method: 'GET',
      url: '/api/app/mood-tracking/history',
      params: { startDate, endDate },
    },
    { apiName: this.apiName,...config });
}