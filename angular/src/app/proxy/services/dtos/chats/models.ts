import type { AuditedEntityDto } from '@abp/ng.core';
import type { ChatSender } from '../../../entities/chats/chat-sender.enum';

export interface ChatMessageDto extends AuditedEntityDto<string> {
  chatSessionId?: string;
  sender?: ChatSender;
  content?: string;
  hasInlineAnalysis: boolean;
  inlineAnalysisSummary?: string;
}

export interface ChatSessionDto extends AuditedEntityDto<string> {
  userId?: string;
  title?: string;
  startedAt?: string;
  endedAt?: string;
  isActive: boolean;
  aiSummary?: string;
  lastMessageTime?: string;
}

export interface CreateUpdateChatMessageDto {
  chatSessionId: string;
  sender: ChatSender;
  content: string;
  hasInlineAnalysis: boolean;
  inlineAnalysisSummary?: string;
}

export interface CreateUpdateChatSessionDto {
  title: string;
  isActive: boolean;
}
