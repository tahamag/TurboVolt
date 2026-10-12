import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { BlivraisonService } from '../../services/blivraison.service';
import { LookupService } from '../../services/lookup.service';
import {
  BlivraisonResponseDto,
  ClientResponseDto,
  FamilleArticleResponseDto,
  UserResponseDto
} from '../../models/wms.models';

@Component({
  selector: 'app-blivraison-list',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatTableModule,
    MatPaginatorModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatButtonModule,
    MatIconModule,
    MatProgressBarModule
  ],
  templateUrl: './blivraison-list.component.html',
  styleUrl: './blivraison-list.component.css'
})
export class BlivraisonListComponent implements OnInit {
  private blService = inject(BlivraisonService);
  private lookupService = inject(LookupService);
  private fb = inject(FormBuilder);

  filterForm!: FormGroup;
  blList: BlivraisonResponseDto[] = [];
  clients: ClientResponseDto[] = [];
  users: UserResponseDto[] = [];
  familles: FamilleArticleResponseDto[] = [];

  expandedBlId: number | null = null;
  totalCount = 0;
  pageSize = 10;
  pageIndex = 0;
  isLoading = false;

  displayedColumns: string[] = [
    'noBonLivraison',
    'dateLivraison',
    'nomClient',
    'totalBonLivraisonHt',
    'totalBonLivraisonTtc',
    'resteAPayer',
    'agentCreateur',
    'actions'
  ];

  articleColumns: string[] = [
    'refArticle',
    'designation',
    'quantiteLivree',
    'prixBonLivraison',
    'tauxTva',
    'montantHt',
    'montantTtc'
  ];

  ngOnInit(): void {
    this.filterForm = this.fb.group({
      dateDebut: [null],
      dateFin: [null],
      idClient: [null],
      idUser: [null],
      refArticle: [''],
      idFamille: [null]
    });

    this.loadLookups();
    this.loadBl();
  }

  loadLookups(): void {
    this.lookupService.getClients().subscribe(res => this.clients = res);
    this.lookupService.getUsers().subscribe(res => this.users = res);
    this.lookupService.getFamilles().subscribe(res => this.familles = res);
  }

  loadBl(): void {
    this.isLoading = true;
    const formValue = this.filterForm.value;

    this.blService.getBlivraisons({
      dateDebut: formValue.dateDebut ? new Date(formValue.dateDebut).toISOString() : undefined,
      dateFin: formValue.dateFin ? new Date(formValue.dateFin).toISOString() : undefined,
      idClient: formValue.idClient || undefined,
      idUser: formValue.idUser || undefined,
      refArticle: formValue.refArticle || undefined,
      idFamille: formValue.idFamille || undefined,
      pageNumber: this.pageIndex + 1,
      pageSize: this.pageSize
    }).subscribe({
      next: (res) => {
        this.blList = res.items;
        this.totalCount = res.totalCount;
        this.isLoading = false;
      },
      error: () => this.isLoading = false
    });
  }

  toggleExpand(id: number): void {
    this.expandedBlId = this.expandedBlId === id ? null : id;
  }

  onSearch(): void {
    this.pageIndex = 0;
    this.loadBl();
  }

  onReset(): void {
    this.filterForm.reset();
    this.pageIndex = 0;
    this.loadBl();
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.loadBl();
  }
}