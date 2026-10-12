import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BlivraisonListComponent } from './blivraison-list.component';

describe('BlivraisonListComponent', () => {
  let component: BlivraisonListComponent;
  let fixture: ComponentFixture<BlivraisonListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BlivraisonListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BlivraisonListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
