import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BreceptionListComponent } from './breception-list.component';

describe('BreceptionListComponent', () => {
  let component: BreceptionListComponent;
  let fixture: ComponentFixture<BreceptionListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BreceptionListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BreceptionListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
