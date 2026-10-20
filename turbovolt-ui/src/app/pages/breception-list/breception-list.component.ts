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
import { BreceptionService } from '../../services/breception.service';
import { LookupService } from '../../services/lookup.service';
import {
  BreceptionResponseDto,
  FamilleArticleResponseDto,
  UserResponseDto
} from '../../models/wms.models';

@Component({
  selector: 'app-breception-list',
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
  templateUrl: './breception-list.component.html',
  styleUrl: './breception-list.component.css'
})
export class BreceptionListComponent implements OnInit {
  private brService = inject(BreceptionService);
  private lookupService = inject(LookupService);
  private fb = inject(FormBuilder);

  filterForm!: FormGroup;
  brList: BreceptionResponseDto[] = [];
  users: UserResponseDto[] = [];
  familles: FamilleArticleResponseDto[] = [];

  expandedBrId: number | null = null;
  totalCount = 0;
  pageSize = 10;
  pageIndex = 0;
  isLoading = false;

  displayedColumns: string[] = [
    'noBl',
    'dateCreation',
    'nomFournisseur',
    'totalBonReceptionHt',
    'totalBonReceptionTtc',
    'resteADonner',
    'agentCreateur',
    'actions'
  ];

  articleColumns: string[] = [
    'refArticle',
    'designation',
    'quantiteReceve',
    'prixBonReception',
    'tauxTva',
    'montantHt',
    'montantTtc'
  ];

  ngOnInit(): void {
    this.filterForm = this.fb.group({
      dateDebut: [null],
      dateFin: [null],
      idFournisseur: [null],
      idDepot: [null],
      idUser: [null],
      refArticle: [''],
      idFamille: [null]
    });

    this.loadLookups();
    this.loadBr();
  }

  loadLookups(): void {
    this.lookupService.getUsers().subscribe(res => this.users = res);
    this.lookupService.getFamilles().subscribe(res => this.familles = res);
  }

  loadBr(): void {
    this.isLoading = true;
    const formValue = this.filterForm.value;

    this.brService.getBreceptions({
      dateDebut: formValue.dateDebut ? new Date(formValue.dateDebut).toISOString() : undefined,
      dateFin: formValue.dateFin ? new Date(formValue.dateFin).toISOString() : undefined,
      idFournisseur: formValue.idFournisseur || undefined,
      idDepot: formValue.idDepot || undefined,
      idUser: formValue.idUser || undefined,
      refArticle: formValue.refArticle || undefined,
      idFamille: formValue.idFamille || undefined,
      pageNumber: this.pageIndex + 1,
      pageSize: this.pageSize
    }).subscribe({
      next: (res) => {
        this.brList = res.items;
        this.totalCount = res.totalCount;
        this.isLoading = false;
      },
      error: () => this.isLoading = false
    });
  }

  toggleExpand(id: number): void {
    this.expandedBrId = this.expandedBrId === id ? null : id;
  }

  onSearch(): void {
    this.pageIndex = 0;
    this.loadBr();
  }

  onReset(): void {
    this.filterForm.reset();
    this.pageIndex = 0;
    this.loadBr();
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.loadBr();
  }
}