import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AvoirBenificeComponent } from './avoir-benifice.component';

describe('AvoirBenificeComponent', () => {
  let component: AvoirBenificeComponent;
  let fixture: ComponentFixture<AvoirBenificeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AvoirBenificeComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AvoirBenificeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
