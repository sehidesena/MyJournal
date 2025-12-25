import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PageModule } from '@abp/ng.components/page';
import { ListService, PagedResultDto, CoreModule } from '@abp/ng.core';
import { JournalEntryService } from '../proxy/services/journal-entries';
import { JournalEntryDto } from '../proxy/services/dtos/journal-entries/models';
import { EntryType } from '../proxy/enums/entry-type.enum';
import { VoiceRecorderComponent } from '../shared/voice-recorder/voice-recorder.component';

@Component({
  selector: 'app-journal-entries',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, PageModule, CoreModule, VoiceRecorderComponent],
  providers: [ListService],
  templateUrl: './journal-entries.html',
  styleUrl: './journal-entries.scss'
})
export class JournalEntries implements OnInit {
  private journalService = inject(JournalEntryService);
  private fb = inject(FormBuilder);
  public list = inject(ListService);

  journals: PagedResultDto<JournalEntryDto> = {
    items: [],
    totalCount: 0,
  };

  form!: FormGroup;
  isModalOpen = false;
  selectedJournal = {} as JournalEntryDto;
  isLoading = false;
  searchText = '';

  // Enum for Template Access
  entryTypes = EntryType;

  constructor() { }

  ngOnInit(): void {
    this.loadJournals();
  }

  loadJournals(): void {
    this.isLoading = true;
    const streamCreator = (query: any) => this.journalService.getList(query);

    this.list.hookToQuery(streamCreator).subscribe(response => {
      this.journals = response;
      this.isLoading = false;
    });
  }

  buildForm(): void {
    const now = new Date();
    const localDateTime = new Date(now.getTime() - (now.getTimezoneOffset() * 60000)).toISOString().slice(0, 16);

    this.form = this.fb.group({
      title: [this.selectedJournal.title || '', [Validators.required, Validators.maxLength(200)]],
      content: [this.selectedJournal.content || '', [Validators.required]],
      entryDate: [this.selectedJournal.entryDate ? new Date(this.selectedJournal.entryDate).toISOString().slice(0, 16) : localDateTime, [Validators.required]],
      type: [this.selectedJournal.type || EntryType.Text, [Validators.required]],
      audioUrl: [this.selectedJournal.audioUrl || ''],
      isPinned: [this.selectedJournal.isPinned || false]
    });
  }

  createJournal(): void {
    this.selectedJournal = {} as JournalEntryDto;
    this.buildForm();
    this.isModalOpen = true;
  }

  editJournal(id: string): void {
    this.journalService.get(id).subscribe((journal) => {
      this.selectedJournal = journal;
      this.buildForm();
      this.isModalOpen = true;
    });
  }

  save(): void {
    if (this.form.invalid) {
      return;
    }

    const formValue = this.form.value;

    // Convert Type to Number if standard HTML select returns string
    formValue.type = Number(formValue.type);

    const request = this.selectedJournal.id
      ? this.journalService.update(this.selectedJournal.id, formValue)
      : this.journalService.create(formValue);

    request.subscribe(() => {
      this.isModalOpen = false;
      this.form.reset();
      this.loadJournals();
    });
  }

  deleteJournal(id: string): void {
    if (confirm('Are you sure you want to delete this journal entry?')) {
      this.journalService.delete(id).subscribe(() => {
        this.loadJournals();
      });
    }
  }

  togglePin(id: string): void {
    this.journalService.togglePin(id).subscribe(() => {
      this.loadJournals();
    });
  }

  closeModal(): void {
    this.isModalOpen = false;
    this.form.reset();
  }

  onRecordingCompleted(blob: Blob) {
    const formData = new FormData();
    formData.append('input', blob, 'voice-recording.wav');

    this.isLoading = true;
    this.journalService.uploadVoice(formData).subscribe({
      next: (url) => {
        this.form.patchValue({
          audioUrl: url,
          type: EntryType.Voice,
          content: this.form.get('content')?.value || 'Ses kaydı eklendi. Analiz bekleniyor...'
        });
        this.isLoading = false;
      },
      error: (err) => {
        console.error(err);
        this.isLoading = false;
        alert('Ses yüklenirken hata oluştu.');
      }
    });
  }

  getFilteredJournals() {
    if (!this.journals.items) return [];

    let filtered = this.journals.items;

    if (this.searchText) {
      filtered = filtered.filter(j =>
        j.title?.toLowerCase().includes(this.searchText.toLowerCase()) ||
        j.content?.toLowerCase().includes(this.searchText.toLowerCase())
      );
    }

    return filtered;
  }
}
