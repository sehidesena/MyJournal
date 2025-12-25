import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';
import type { CreateUpdateJournalEntryDto, JournalEntryDto } from '../dtos/journal-entries/models';

@Injectable({
  providedIn: 'root',
})
export class JournalEntryService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateUpdateJournalEntryDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, JournalEntryDto>({
      method: 'POST',
      url: '/api/app/journal-entry',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/journal-entry/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, JournalEntryDto>({
      method: 'GET',
      url: `/api/app/journal-entry/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<JournalEntryDto>>({
      method: 'GET',
      url: '/api/app/journal-entry',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getMyJournalEntries = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, JournalEntryDto[]>({
      method: 'GET',
      url: '/api/app/journal-entry/my-journal-entries',
    },
    { apiName: this.apiName,...config });
  

  togglePin = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, JournalEntryDto>({
      method: 'POST',
      url: `/api/app/journal-entry/${id}/toggle-pin`,
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdateJournalEntryDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, JournalEntryDto>({
      method: 'PUT',
      url: `/api/app/journal-entry/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
  

  uploadVoice = (input: FormData, config?: Partial<Rest.Config>) =>
    this.restService.request<any, string>({
      method: 'POST',
      responseType: 'text',
      url: '/api/app/journal-entry/upload-voice',
      body: input,
    },
    { apiName: this.apiName,...config });
}