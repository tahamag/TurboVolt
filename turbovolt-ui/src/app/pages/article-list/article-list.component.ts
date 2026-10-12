import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatAutocompleteModule, MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { ArticleService } from '../../services/article.service';
import { LookupService } from '../../services/lookup.service';
import {
  ArticleResponseDto,
  FamilleArticleResponseDto,
  SousFamilleArticleResponseDto
} from '../../models/wms.models';

@Component({
  selector: 'app-article-list',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatTableModule,
    MatPaginatorModule,
    MatFormFieldModule,
    MatInputModule,
    MatAutocompleteModule,
    MatButtonModule,
    MatIconModule,
    MatProgressBarModule
  ],
  templateUrl: './article-list.component.html',
  styleUrl: './article-list.component.css'
})
export class ArticleListComponent implements OnInit {
  private articleService = inject(ArticleService);
  private lookupService = inject(LookupService);
  private fb = inject(FormBuilder);

  filterForm!: FormGroup;
  articles: ArticleResponseDto[] = [];
  
  familles: FamilleArticleResponseDto[] = [];
  sousFamilles: SousFamilleArticleResponseDto[] = [];

  filteredFamilles$!: Observable<FamilleArticleResponseDto[]>;
  filteredSousFamilles$!: Observable<SousFamilleArticleResponseDto[]>;

  totalCount = 0;
  pageSize = 10;
  pageIndex = 0;
  isLoading = false;

  displayedColumns: string[] = [
    'refArticle',
    'numArticle',
    'designation',
    'libelleFamille',
    'libelleSousFamille',
    'prixAchatHt',
    'prixVenteHt',
    'prixVenteTtc'
  ];

  ngOnInit(): void {
    this.filterForm = this.fb.group({
      searchTerm: [''],
      familleInput: [''],
      idFamille: [null],
      sousFamilleInput: [''],
      idSousFamille: [null]
    });

    this.loadLookups();
    this.loadArticles();

    this.filteredFamilles$ = this.filterForm.get('familleInput')!.valueChanges.pipe(
      startWith(''),
      map(value => typeof value === 'string' ? value : value?.libelleFamArticle || ''),
      map(name => name ? this._filterFamilles(name) : this.familles.slice())
    );

    this.filteredSousFamilles$ = this.filterForm.get('sousFamilleInput')!.valueChanges.pipe(
      startWith(''),
      map(value => typeof value === 'string' ? value : value?.libelleSousFamille || ''),
      map(name => name ? this._filterSousFamilles(name) : this.sousFamilles.slice())
    );
  }

  loadLookups(): void {
    this.lookupService.getFamilles().subscribe(res => this.familles = res);
    this.lookupService.getSousFamilles().subscribe(res => this.sousFamilles = res);
  }

  loadArticles(): void {
    this.isLoading = true;
    const formValue = this.filterForm.value;

    this.articleService.getArticles({
      searchTerm: formValue.searchTerm || undefined,
      idFamille: formValue.idFamille || undefined,
      idSousFamille: formValue.idSousFamille || undefined,
      pageNumber: this.pageIndex + 1,
      pageSize: this.pageSize
    }).subscribe({
      next: (res) => {
        this.articles = res.items;
        this.totalCount = res.totalCount;
        this.isLoading = false;
      },
      error: () => this.isLoading = false
    });
  }

  private _filterFamilles(name: string): FamilleArticleResponseDto[] {
    const filterValue = name.toLowerCase();
    return this.familles.filter(f => f.libelleFamArticle?.toLowerCase().includes(filterValue));
  }

  private _filterSousFamilles(name: string): SousFamilleArticleResponseDto[] {
    const filterValue = name.toLowerCase();
    return this.sousFamilles.filter(sf => sf.libelleSousFamille?.toLowerCase().includes(filterValue));
  }

  displayFamilleFn(famille?: FamilleArticleResponseDto): string {
    return famille && famille.libelleFamArticle ? famille.libelleFamArticle : '';
  }

  displaySousFamilleFn(sousFamille?: SousFamilleArticleResponseDto): string {
    return sousFamille && sousFamille.libelleSousFamille ? sousFamille.libelleSousFamille : '';
  }

  onFamilleSelected(event: MatAutocompleteSelectedEvent): void {
    const selected: FamilleArticleResponseDto = event.option.value;
    if (selected) {
      this.filterForm.patchValue({ idFamille: selected.idFamilleArticle });
      this.lookupService.getSousFamilles(selected.idFamilleArticle).subscribe(res => {
        this.sousFamilles = res;
        this.filterForm.patchValue({ sousFamilleInput: '', idSousFamille: null });
      });
    }
  }

  onSousFamilleSelected(event: MatAutocompleteSelectedEvent): void {
    const selected: SousFamilleArticleResponseDto = event.option.value;
    if (selected) {
      this.filterForm.patchValue({ idSousFamille: selected.idSousFamille });
    }
  }

  onSearch(): void {
    this.pageIndex = 0;
    this.loadArticles();
  }

  onReset(): void {
    this.filterForm.reset();
    this.lookupService.getSousFamilles().subscribe(res => this.sousFamilles = res);
    this.pageIndex = 0;
    this.loadArticles();
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.loadArticles();
  }
}