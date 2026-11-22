import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  trigger,
  transition,
  style,
  animate,
  query,
  stagger,
} from '@angular/animations';

// Angular Material Modules
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MAT_DATE_LOCALE, MatNativeDateModule, provideNativeDateAdapter } from '@angular/material/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatTooltipModule } from '@angular/material/tooltip';

import { AvoirBenificeService } from '../../services/avoir-benifice.service';
import { UserService } from '../../services/user.service';
import {
  AvoirBenificeFilter,
  BenificeDifferenceByUser,
} from '../../models/wms.models';

@Component({
  selector: 'app-benifice-globale',
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
  templateUrl: './benifice-globale.component.html',
  styleUrls: ['./benifice-globale.component.css'],
  animations: [
    trigger('fadeSlideUp', [
      transition(':enter', [
        style({ opacity: 0, transform: 'translateY(14px)' }),
        animate(
          '360ms cubic-bezier(0.22, 1, 0.36, 1)',
          style({ opacity: 1, transform: 'translateY(0)' })
        ),
      ]),
    ]),
    trigger('staggerList', [
      transition('* => *', [
        query(
          ':enter',
          [
            style({ opacity: 0, transform: 'translateY(10px)' }),
            stagger('55ms', [
              animate(
                '280ms cubic-bezier(0.22, 1, 0.36, 1)',
                style({ opacity: 1, transform: 'translateY(0)' })
              ),
            ]),
          ],
          { optional: true }
        ),
      ]),
    ]),
  ],
  providers: [
    provideNativeDateAdapter(),
    { provide: MAT_DATE_LOCALE, useValue: 'fr-FR' } // Force la locale en Français
  ]
})

export class BenificeGlobaleComponent implements OnInit {
  users: any[] = [];
  items: BenificeDifferenceByUser[] = [];

  startDateObj: Date | null = null;
  endDateObj: Date | null = null;

  filter: AvoirBenificeFilter = {
    userId: null,
    articleId: null,
    articleSearch: '',
    dateDebut: '',
    dateFin: '',
  };

  displayedColumns: string[] = [
    'userName',
    'globalBlBenifice',
    'globalAvoirBenifice',
    'impactRatio',
    'differenceBenifice',
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
    this.userService.getUsers().subscribe({
      next: (data: any[]) => {
        this.users = data;
      },
      error: (err) => {
        console.error('Erreur chargement utilisateurs:', err);
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

    this.avoirBenificeService.getDifferenceByUser(activeFilter).subscribe({
      next: (data) => {
        this.items = data;
        this.loading = false;
      },
      error: (err) => {
        console.error(err);
        this.errorMessage =
          'Impossible de charger le bilan des bénéfices globaux.';
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

  get totalBl(): number {
    return this.items.reduce((acc, x) => acc + (x.globalBlBenifice || 0), 0);
  }

  get totalAvoir(): number {
    return this.items.reduce(
      (acc, x) => acc + (x.globalAvoirBenifice || 0),
      0
    );
  }

  get totalDifference(): number {
    return this.items.reduce((acc, x) => acc + (x.differenceBenifice || 0), 0);
  }

  getAvoirSharePercent(row: BenificeDifferenceByUser): number {
    if (!row.globalBlBenifice || row.globalBlBenifice <= 0) return 0;
    const pct = (row.globalAvoirBenifice / row.globalBlBenifice) * 100;
    return Math.max(0, Math.min(100, pct));
  }
}