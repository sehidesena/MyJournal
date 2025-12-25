import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';
import type { RecommendationDto } from '../dtos/recommendations/models';

@Injectable({
  providedIn: 'root',
})
export class RecommendationService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  generateRefreshedRecommendations = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, RecommendationDto[]>({
      method: 'POST',
      url: '/api/app/recommendation/generate-refreshed-recommendations',
    },
    { apiName: this.apiName,...config });
  

  getMyRecommendations = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, RecommendationDto[]>({
      method: 'GET',
      url: '/api/app/recommendation/my-recommendations',
    },
    { apiName: this.apiName,...config });
}