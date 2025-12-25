import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class AiChatService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  getResponse = (userMessage: string, systemPrompt?: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, string>({
      method: 'GET',
      responseType: 'text',
      url: '/api/app/ai-chat/response',
      params: { userMessage, systemPrompt },
    },
    { apiName: this.apiName,...config });
}