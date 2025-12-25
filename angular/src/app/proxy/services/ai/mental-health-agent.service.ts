import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';
import type { ChatMessageDto } from '../dtos/chats/models';

@Injectable({
  providedIn: 'root',
})
export class MentalHealthAgentService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  chat = (userMessage: string, history: ChatMessageDto[], config?: Partial<Rest.Config>) =>
    this.restService.request<any, string>({
      method: 'POST',
      responseType: 'text',
      url: '/api/app/mental-health-agent/chat',
      params: { userMessage },
      body: history,
    },
    { apiName: this.apiName,...config });
}