import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { ArticleService } from '../../services/article.service';
import { BlivraisonService } from '../../services/blivraison.service';
import { BreceptionService } from '../../services/breception.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatIconModule, MatProgressBarModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {
  private articleService = inject(ArticleService);
  private blService = inject(BlivraisonService);
  private brService = inject(BreceptionService);

  totalArticles = 0;
  totalBl = 0;
  totalBr = 0;
  totalVentesTtc = 0;
  totalAchatsTtc = 0;
  isLoading = true;

  ngOnInit(): void {
    this.loadStats();
  }

  loadStats(): void {
    this.isLoading = true;

    this.articleService.getArticles({ pageNumber: 1, pageSize: 1 }).subscribe(res => {
      this.totalArticles = res.totalCount;
    });

    this.blService.getBlivraisons({ pageNumber: 1, pageSize: 10 }).subscribe(res => {
      this.totalBl = res.totalCount;
      this.totalVentesTtc = res.items.reduce((sum, item) => sum + (item.totalBonLivraisonTtc || 0), 0);
    });

    this.brService.getBreceptions({ pageNumber: 1, pageSize: 10 }).subscribe(res => {
      this.totalBr = res.totalCount;
      this.totalAchatsTtc = res.items.reduce((sum, item) => sum + (item.totalBonReceptionTtc || 0), 0);
      this.isLoading = false;
    });
  }
}