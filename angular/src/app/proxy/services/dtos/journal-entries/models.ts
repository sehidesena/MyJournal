import type { EntryType } from '../../../enums/entry-type.enum';
import type { FullAuditedEntityDto } from '@abp/ng.core';

export interface CreateUpdateJournalEntryDto {
  title: string;
  content: string;
  entryDate: string;
  type?: EntryType;
  audioUrl?: string;
  durationSeconds?: number;
  isPinned?: boolean;
}

export interface JournalEntryDto extends FullAuditedEntityDto<string> {
  userId?: string;
  title?: string;
  content?: string;
  entryDate?: string;
  type?: EntryType;
  audioUrl?: string;
  durationSeconds?: number;
  isPinned?: boolean;
  hasAiAnalysis: boolean;
}
