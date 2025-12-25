import { Component, inject, OnInit } from '@angular/core';
import { AuthService } from '@abp/ng.core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MoodTrackingService } from '../proxy/services/mood-tracking';
import { RecommendationService } from '../proxy/services/recommendations';
import { JournalEntryService } from '../proxy/services/journal-entries';
import { PageModule } from '@abp/ng.components/page';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
  standalone: true,
  imports: [CommonModule, RouterLink, PageModule],
})
export class HomeComponent implements OnInit {
  private moodService = inject(MoodTrackingService);
  private recService = inject(RecommendationService);
  private journalService = inject(JournalEntryService);
  private authService = inject(AuthService);

  get hasLoggedIn(): boolean {
    return this.authService.isAuthenticated;
  }

  // Dashboard Data
  latestMood: any = null;
  latestRec: any = null;
  journalCount = 0;

  constructor() { }

  ngOnInit() {
    if (this.hasLoggedIn) {
      this.loadDashboardData();
    }
  }

  loadDashboardData() {
    // Load latest mood (Need to get history and take first, or we need a GetLatest endpoint which we simulated in backend Logic but not exposed as specific endpoint. We can use GetHistory with expected range or just GetHistory).
    // Let's use GetHistory for last 7 days.
    const end = new Date();
    const start = new Date();
    start.setDate(end.getDate() - 7);

    this.moodService.getHistory(start.toISOString(), end.toISOString()).subscribe(logs => {
      if (logs && logs.length > 0) {
        // Logs are ordered by Timestamp Ascending in backend?
        // Backend: .OrderBy(x => x.Timestamp)
        // So last item is latest.
        this.latestMood = logs[logs.length - 1];
      }
    });

    // Load journal count
    this.journalService.getList({ maxResultCount: 1 }).subscribe(res => {
      this.journalCount = res.totalCount || 0;
    });

    // Load latest rec
    this.recService.getMyRecommendations().subscribe(recs => {
      if (recs && recs.length > 0) {
        this.latestRec = recs[0]; // Ordered by Descending Time in backend
      }
    });
  }

  login() {
    this.authService.navigateToLogin();
  }
}
