import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';
import type { EmotionalAnalysisResult } from '../../entities/analysis/models';
import type { JournalEntry } from '../../entities/journal-entries/models';

@Injectable({
  providedIn: 'root',
})
export class AnalysisService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  analyzeJournalEntry = (entry: JournalEntry, config?: Partial<Rest.Config>) =>
    this.restService.request<any, EmotionalAnalysisResult>({
      method: 'POST',
      url: '/api/app/analysis/analyze-journal-entry',
      body: entry,
    },
    { apiName: this.apiName,...config });
}