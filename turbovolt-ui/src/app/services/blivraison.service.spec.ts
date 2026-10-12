import { TestBed } from '@angular/core/testing';

import { BlivraisonService } from './blivraison.service';

describe('BlivraisonService', () => {
  let service: BlivraisonService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(BlivraisonService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
