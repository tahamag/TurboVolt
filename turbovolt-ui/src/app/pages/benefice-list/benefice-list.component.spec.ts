import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BeneficeListComponent } from './benefice-list.component';

describe('BeneficeListComponent', () => {
  let component: BeneficeListComponent;
  let fixture: ComponentFixture<BeneficeListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BeneficeListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BeneficeListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
