import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class VoiceProcessingService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  transcribeAudio = (audioUrlOrPath: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, string>({
      method: 'POST',
      responseType: 'text',
      url: '/api/app/voice-processing/transcribe-audio',
      params: { audioUrlOrPath },
    },
    { apiName: this.apiName,...config });
}