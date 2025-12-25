import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';
import type { ChatMessageDto, ChatSessionDto, CreateUpdateChatMessageDto, CreateUpdateChatSessionDto } from '../dtos/chats/models';

@Injectable({
  providedIn: 'root',
})
export class ChatService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  askAi = (input: CreateUpdateChatMessageDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ChatMessageDto>({
      method: 'POST',
      url: '/api/app/chat/ask-ai',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  create = (input: CreateUpdateChatSessionDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ChatSessionDto>({
      method: 'POST',
      url: '/api/app/chat',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/chat/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ChatSessionDto>({
      method: 'GET',
      url: `/api/app/chat/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<ChatSessionDto>>({
      method: 'GET',
      url: '/api/app/chat',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getMessages = (chatSessionId: string, input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<ChatMessageDto>>({
      method: 'GET',
      url: `/api/app/chat/messages/${chatSessionId}`,
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  sendMessage = (input: CreateUpdateChatMessageDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ChatMessageDto>({
      method: 'POST',
      url: '/api/app/chat/send-message',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdateChatSessionDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ChatSessionDto>({
      method: 'PUT',
      url: `/api/app/chat/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}