import type { FullAuditedEntity } from '../../volo/abp/domain/entities/auditing/models';
import type { JournalEntry } from '../journal-entries/models';

export interface EmotionalAnalysisResult extends FullAuditedEntity<string> {
  journalEntryId?: string;
  sentimentScore: number;
  dominantEmotion?: string;
  analysisSummary?: string;
  emotionProbabilities?: string;
  clinicalFlags?: string;
  journalEntry: JournalEntry;
}
