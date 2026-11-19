import { TestBed } from '@angular/core/testing';

import { AvoirBenificeService } from './avoir-benifice.service';

describe('AvoirBenificeService', () => {
  let service: AvoirBenificeService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AvoirBenificeService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
