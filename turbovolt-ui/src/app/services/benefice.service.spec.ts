import { TestBed } from '@angular/core/testing';

import { BeneficeService } from './benefice.service';

describe('BeneficeService', () => {
  let service: BeneficeService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(BeneficeService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
