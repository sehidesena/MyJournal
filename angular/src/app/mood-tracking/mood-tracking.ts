import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PageModule } from '@abp/ng.components/page';
import { ListService, CoreModule } from '@abp/ng.core';
import { MoodTrackingService } from '../proxy/services/mood-tracking';
import { MoodLogDto, CreateMoodLogDto } from '../proxy/services/dtos/mood-tracking/models';

@Component({
  selector: 'app-mood-tracking',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, PageModule, CoreModule],
  providers: [ListService],
  templateUrl: './mood-tracking.html',
  styleUrl: './mood-tracking.scss'
})
export class MoodTracking implements OnInit {
  private moodService = inject(MoodTrackingService);
  private fb = inject(FormBuilder);

  moodLogs: MoodLogDto[] = [];
  form!: FormGroup;
  isModalOpen = false;

  stats: any = {}; // { 'Happy': 5, 'Sad': 2 }

  moodEmojis = [
    { value: 1, emoji: '😢', label: 'Berbat' },
    { value: 2, emoji: '😟', label: 'Kötü' },
    { value: 3, emoji: '😕', label: 'Pek İyi Değil' },
    { value: 4, emoji: '😐', label: 'İdare Eder' },
    { value: 5, emoji: '🙂', label: 'İyi' },
    { value: 6, emoji: '😊', label: 'Güzel' },
    { value: 7, emoji: '😄', label: 'Mutlu' },
    { value: 8, emoji: '😁', label: 'Harika' },
    { value: 9, emoji: '🤗', label: 'Mükemmel' },
    { value: 10, emoji: '🥳', label: 'Fantastik' }
  ];

  commonEmotions = ['Mutlu', 'Üzgün', 'Endişeli', 'Heyecanlı', 'Stresli', 'Sakin', 'Sıkılmış', 'Öfkeli', 'Minnettar', 'Umutlu'];

  ngOnInit(): void {
    this.loadHistory();
  }

  loadHistory(): void {
    const end = new Date();
    const start = new Date();
    start.setDate(end.getDate() - 30); // Last 30 days

    this.moodService.getHistory(start.toISOString(), end.toISOString()).subscribe(logs => {
      this.moodLogs = logs;
      this.calculateStats();
    });
  }

  calculateStats() {
    // Mock stats from logs
    // In real world, we might call getEmotionStats endpoint
    const end = new Date();
    const start = new Date();
    start.setDate(end.getDate() - 30);

    this.moodService.getEmotionStats(start.toISOString(), end.toISOString()).subscribe(stats => {
      this.stats = stats;
    });
  }

  buildForm(): void {
    this.form = this.fb.group({
      intensity: [5, [Validators.required, Validators.min(1), Validators.max(10)]],
      primaryEmotion: ['', [Validators.required]],
      note: [''],
      timestamp: [new Date().toISOString().slice(0, 16), [Validators.required]]
    });
  }

  openLogModal(): void {
    this.buildForm();
    this.isModalOpen = true;
  }

  save(): void {
    if (this.form.invalid) return;

    const input = this.form.value as CreateMoodLogDto;

    // Ensure timestamp is ISO
    input.timestamp = new Date(input.timestamp).toISOString();

    this.moodService.create(input).subscribe(() => {
      this.isModalOpen = false;
      this.loadHistory();
    });
  }

  closeModal(): void {
    this.isModalOpen = false;
  }

  selectIntensity(val: number) {
    this.form.patchValue({ intensity: val });
  }

  selectEmotion(emotion: string) {
    this.form.patchValue({ primaryEmotion: emotion });
  }

  getEmoji(intensity: number) {
    const found = this.moodEmojis.find(e => e.value === intensity);
    return found ? found.emoji : '😐';
  }
}
