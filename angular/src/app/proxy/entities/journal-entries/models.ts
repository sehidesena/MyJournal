import type { FullAuditedAggregateRoot } from '../../volo/abp/domain/entities/auditing/models';
import type { EntryType } from '../../enums/entry-type.enum';
import type { IdentityUser } from '../../volo/abp/identity/models';
import type { EmotionalAnalysisResult } from '../analysis/models';

export interface JournalEntry extends FullAuditedAggregateRoot<string> {
  userId?: string;
  title?: string;
  content?: string;
  entryDate?: string;
  type?: EntryType;
  audioUrl?: string;
  durationSeconds?: number;
  isPinned?: boolean;
  hasAiAnalysis: boolean;
  user: IdentityUser;
  analysisResult: EmotionalAnalysisResult;
}
