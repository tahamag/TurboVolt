import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';

// Angular Material Modules
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatTooltipModule } from '@angular/material/tooltip';

import { AvoirBenificeService } from '../../services/avoir-benifice.service';
// Adaptez le chemin vers votre service utilisateur existant
import { UserService } from '../../services/user.service';
import {
  AvoirBenificeByUser,
  AvoirBenificeDetail,
  AvoirBenificeFilter,
} from '../../models/wms.models';

@Component({
  selector: 'app-avoir-benifice',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    MatProgressBarModule,
    MatTooltipModule,
  ],
  templateUrl: './avoir-benifice.component.html',
  styleUrls: ['./avoir-benifice.component.css'],
})
export class AvoirBenificeComponent implements OnInit {
  users: any[] = [];

  startDateObj: Date | null = null;
  endDateObj: Date | null = null;

  filter: AvoirBenificeFilter = {
    userId: null,
    articleId: null,
    articleSearch: '',
    dateDebut: '',
    dateFin: '',
  };

  summaryByUser: AvoirBenificeByUser[] = [];
  details: AvoirBenificeDetail[] = [];

  summaryColumns: string[] = [
    'userName',
    'nombreAvoirs',
    'nombreLignes',
    'lignesCalculeesParBL',
    'lignesCalculeesParPMP',
    'totalBenificeAvoirCalculated',
  ];

  detailColumns: string[] = [
    'codeAvoir',
    'dateCreation',
    'nomClient',
    'userName',
    'referenceArticle',
    'quantiteRetournee',
    'prixAvoir',
    'benificeAvoirCalculated',
    'methodeCalcul',
  ];

  loading = false;
  errorMessage = '';

  constructor(
    private avoirBenificeService: AvoirBenificeService,
    private userService: UserService
  ) {}

  ngOnInit(): void {
    this.loadUsers();
    this.search();
  }

  loadUsers(): void {
    // Adaptez le nom de la méthode selon votre UserService (ex: getAll(), getUsers())
    this.userService.getUsers().subscribe({
      next: (data: any[]) => {
        this.users = data;
      },
      error: (err) => {
        console.error('Erreur lors du chargement des utilisateurs:', err);
      },
    });
  }

  private formatDateToIso(date: Date | null): string | undefined {
    if (!date) return undefined;
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  search(): void {
    this.loading = true;
    this.errorMessage = '';

    const activeFilter: AvoirBenificeFilter = {
      ...this.filter,
      dateDebut: this.formatDateToIso(this.startDateObj),
      dateFin: this.formatDateToIso(this.endDateObj),
    };

    forkJoin({
      byUser: this.avoirBenificeService.getByUser(activeFilter),
      details: this.avoirBenificeService.getDetails(activeFilter),
    }).subscribe({
      next: ({ byUser, details }) => {
        this.summaryByUser = byUser;
        this.details = details;
        this.loading = false;
      },
      error: (err) => {
        console.error(err);
        this.errorMessage = 'Impossible de charger les données de bénéfice sur avoirs.';
        this.loading = false;
      },
    });
  }

  resetFilters(): void {
    this.startDateObj = null;
    this.endDateObj = null;
    this.filter = {
      userId: null,
      articleId: null,
      articleSearch: '',
      dateDebut: '',
      dateFin: '',
    };
    this.search();
  }

  get totalGlobalBenificeAvoir(): number {
    return this.summaryByUser.reduce(
      (sum, item) => sum + (item.totalBenificeAvoirCalculated || 0),
      0
    );
  }
}