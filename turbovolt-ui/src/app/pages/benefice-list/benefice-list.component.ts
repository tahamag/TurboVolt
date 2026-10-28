import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MAT_DATE_LOCALE, MatNativeDateModule, provideNativeDateAdapter } from '@angular/material/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { BeneficeService } from '../../services/benefice.service';
import { LookupService } from '../../services/lookup.service';
import { BeneficeResponseDto, UserResponseDto } from '../../models/wms.models';

@Component({
  selector: 'app-benefice-list',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatTableModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatButtonModule,
    MatIconModule,
    MatProgressBarModule
  ],
  templateUrl: './benefice-list.component.html',
  styleUrl: './benefice-list.component.css',
  providers: [
    provideNativeDateAdapter(),
    { provide: MAT_DATE_LOCALE, useValue: 'fr-FR' } // Force la locale en Français
  ]
})
export class BeneficeListComponent implements OnInit {
  private beneficeService = inject(BeneficeService);
  private lookupService = inject(LookupService);
  private fb = inject(FormBuilder);

  filterForm!: FormGroup;
  benefices: BeneficeResponseDto[] = [];
  users: UserResponseDto[] = [];
  isLoading = false;

  // Totaux
  totalVenteHt = 0;
  totalAchatHt = 0;
  totalBeneficeHt = 0;

  displayedColumns: string[] = [
    'refBonLivraison',
    'dateHeureVente',
    'nom',
    'refArticle',
    'designationArticle',
    'quantiteLivree',
    'prixVenteUnitaireHt',
    'montantTotalVenteHt',
    'prixAchatUnitaireHt',
    'refBonReceptionInterne',
    'beneficeHt'
  ];

  ngOnInit(): void {
    this.filterForm = this.fb.group({
      idUser: [null],
      dateDebut: [null],
      dateFin: [null]
    });

    this.loadUsers();
    this.loadBenefices();
  }

  loadUsers(): void {
    this.lookupService.getUsers().subscribe(res => this.users = res);
  }

  loadBenefices(): void {
    this.isLoading = true;
    const formValue = this.filterForm.value;

    const dateDebutStr = formValue.dateDebut ? new Date(formValue.dateDebut).toISOString() : undefined;
    const dateFinStr = formValue.dateFin ? new Date(formValue.dateFin).toISOString() : undefined;

    this.beneficeService.getBenefices(formValue.idUser || undefined, dateDebutStr, dateFinStr).subscribe({
      next: (data) => {
        this.benefices = data;
        this.calculateTotals();
        this.isLoading = false;
      },
      error: () => this.isLoading = false
    });
  }

  calculateTotals(): void {
    this.totalVenteHt = this.benefices.reduce((acc, curr) => acc + (curr.montantTotalVenteHt || 0), 0);
    this.totalAchatHt = this.benefices.reduce((acc, curr) => acc + ((curr.prixAchatUnitaireHt || 0) * (curr.quantiteLivree || 0)), 0);
    this.totalBeneficeHt = this.benefices.reduce((acc, curr) => acc + (curr.beneficeHt || 0), 0);
  }

  onSearch(): void {
    this.loadBenefices();
  }

  onReset(): void {
    this.filterForm.reset();
    this.loadBenefices();
  }
}