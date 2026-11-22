import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BenificeGlobaleComponent } from './benifice-globale.component';

describe('BenificeGlobaleComponent', () => {
  let component: BenificeGlobaleComponent;
  let fixture: ComponentFixture<BenificeGlobaleComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BenificeGlobaleComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BenificeGlobaleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
