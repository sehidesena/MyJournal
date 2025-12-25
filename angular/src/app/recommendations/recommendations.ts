import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PageModule } from '@abp/ng.components/page';
import { CoreModule } from '@abp/ng.core';
import { RecommendationService } from '../proxy/services/recommendations';
import { RecommendationDto } from '../proxy/services/dtos/recommendations/models';
import { RecommendationType } from '../proxy/entities/recommendations';

@Component({
  selector: 'app-recommendations',
  standalone: true,
  imports: [CommonModule, PageModule, CoreModule],
  templateUrl: './recommendations.html',
  styleUrl: './recommendations.scss' // Make sure file exists or remove if not needed
})
export class Recommendations implements OnInit {
  private recService = inject(RecommendationService);

  items: RecommendationDto[] = [];
  isLoading = false;

  recTypes = RecommendationType;

  ngOnInit(): void {
    this.refresh();
  }

  refresh() {
    this.isLoading = true;
    this.recService.getMyRecommendations().subscribe(list => {
      this.items = list;
      this.isLoading = false;
    });
  }

  generateNew() {
    this.isLoading = true;
    this.recService.generateRefreshedRecommendations().subscribe(list => {
      // If update returns the new list, use it. Or reload.
      // The service returns the list.
      this.items = list; // Assuming backend returns just generated? Wait, backend app service signature: Task<List<RecommendationDto>> GenerateRefreshedRecommendationsAsync(). Yes.
      // But maybe we want to see full history? MyRecommendations gets history (limit 20).
      // Let's re-fetch my recommendations to see mix of old and new, or just assume the user wants to see what's new.
      // Let's re-fetch history to be consistent.
      this.refresh();
    });
  }

  getIcon(type: RecommendationType) {
    switch (type) {
      case RecommendationType.Music: return 'fa-music';
      case RecommendationType.Meditation: return 'fa-spa';
      case RecommendationType.Book: return 'fa-book';
      case RecommendationType.Activity: return 'fa-walking';
      case RecommendationType.Article: return 'fa-newspaper';
      default: return 'fa-star';
    }
  }

  getTypeName(type: RecommendationType): string {
    switch (type) {
      case RecommendationType.Music: return 'Müzik';
      case RecommendationType.Meditation: return 'Meditasyon';
      case RecommendationType.Book: return 'Kitap';
      case RecommendationType.Activity: return 'Aktivite';
      case RecommendationType.Article: return 'Makale';
      default: return 'Öneri';
    }
  }

  getColor(type: RecommendationType) {
    switch (type) {
      case RecommendationType.Music: return 'bg-info text-white';
      case RecommendationType.Meditation: return 'bg-success text-white';
      case RecommendationType.Book: return 'bg-warning text-dark';
      case RecommendationType.Activity: return 'bg-primary text-white';
      case RecommendationType.Article: return 'bg-secondary text-white';
      default: return 'bg-light text-dark';
    }
  }
}
