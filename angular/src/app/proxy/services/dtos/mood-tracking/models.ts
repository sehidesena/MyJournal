import type { FullAuditedEntityDto } from '@abp/ng.core';

export interface CreateMoodLogDto {
  timestamp: string;
  intensity: number;
  primaryEmotion: string;
  note?: string;
}

export interface MoodLogDto extends FullAuditedEntityDto<string> {
  userId?: string;
  timestamp?: string;
  intensity: number;
  primaryEmotion?: string;
  note?: string;
}
