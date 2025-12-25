import { Component, OnInit, inject, ViewChild, ElementRef, AfterViewChecked } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ChatService } from '../proxy/services/chats/chat.service';
import { ChatSessionDto, ChatMessageDto } from '../proxy/services/dtos/chats/models';
import { ChatSender } from '../proxy/entities/chats/chat-sender.enum';
import { ConfirmationService, Confirmation } from '@abp/ng.theme.shared';
import { LocalizationPipe, LocalizationService } from '@abp/ng.core';
import { MarkdownModule } from 'ngx-markdown';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [CommonModule, FormsModule, MarkdownModule, LocalizationPipe],
  templateUrl: './chat.html',
  styleUrls: ['./chat.scss']
})
export class Chat implements OnInit, AfterViewChecked {
  private chatService = inject(ChatService);
  private confirmation = inject(ConfirmationService);
  private localization = inject(LocalizationService);

  sessions: ChatSessionDto[] = [];
  messages: ChatMessageDto[] = [];
  currentSession: ChatSessionDto | null = null;
  newMessage = '';
  isLoading = false;
  chatSender = ChatSender; // For template access

  @ViewChild('scrollContainer') private scrollContainer: ElementRef;
  @ViewChild('messageInput') private messageInput: ElementRef;

  ngOnInit() {
    this.loadSessions();
  }

  ngAfterViewChecked() {
    this.scrollToBottom();
  }

  scrollToBottom(): void {
    try {
      if (this.scrollContainer) {
        this.scrollContainer.nativeElement.scrollTop = this.scrollContainer.nativeElement.scrollHeight;
      }
    } catch (err) { }
  }

  autoResize(textarea: any) {
    textarea.style.height = 'auto';
    textarea.style.height = textarea.scrollHeight + 'px';
  }

  onEnter(event: any) {
    if (!event.shiftKey) {
      event.preventDefault();
      this.sendMessage();
    }
  }

  loadSessions() {
    this.chatService.getList({ maxResultCount: 100 }).subscribe(res => {
      this.sessions = res.items;
      if (this.sessions.length > 0 && !this.currentSession) {
        this.selectSession(this.sessions[0]);
      }
    });
  }

  createNewSession() {
    const baseTitle = this.localization.instant('::Chat:NewSessionDefault');
    const title = `${baseTitle} ${new Date().toLocaleString()}`;
    this.chatService.create({ title, isActive: true }).subscribe(session => {
      this.sessions.unshift(session);
      this.selectSession(session);
    });
  }

  selectSession(session: ChatSessionDto) {
    this.currentSession = session;
    this.messages = [];
    this.chatService.getMessages(session.id, { maxResultCount: 100 }).subscribe(res => {
      this.messages = res.items;
      this.scrollToBottom();
    });
  }

  sendMessage() {
    if (!this.newMessage.trim() || !this.currentSession || this.isLoading) return;

    const content = this.newMessage;
    this.newMessage = '';

    // Reset textarea height
    if (this.messageInput) {
      this.messageInput.nativeElement.style.height = 'auto';
    }

    this.isLoading = true;

    // Optimistic update
    const tempMessage: ChatMessageDto = {
      id: 'temp',
      chatSessionId: this.currentSession.id,
      sender: ChatSender.User,
      content: content,
      creationTime: new Date().toISOString(),
      hasInlineAnalysis: false,
      inlineAnalysisSummary: ''
    };
    this.messages.push(tempMessage);
    this.scrollToBottom();

    this.chatService.askAi({
      chatSessionId: this.currentSession.id,
      sender: ChatSender.User,
      content: content,
      hasInlineAnalysis: false
    }).subscribe({
      next: (aiMessage) => {
        // Reload to get correct states
        this.chatService.getMessages(this.currentSession!.id, { maxResultCount: 100 }).subscribe(res => {
          this.messages = res.items;
          this.isLoading = false;
          this.scrollToBottom();
        });
      },
      error: () => {
        this.isLoading = false;
        this.messages.pop(); // Remove temp on error
      }
    });
  }

  deleteSession(e: Event, id: string) {
    e.stopPropagation();
    this.confirmation.warn('::AreYouSureToDelete', '::AreYouSure').subscribe((status) => {
      if (status === Confirmation.Status.confirm) {
        this.chatService.delete(id).subscribe(() => {
          this.sessions = this.sessions.filter(s => s.id !== id);
          if (this.currentSession?.id === id) {
            this.currentSession = null;
            this.messages = [];
            if (this.sessions.length > 0) this.selectSession(this.sessions[0]);
          }
        });
      }
    });
  }
}
