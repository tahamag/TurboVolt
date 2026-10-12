import { TestBed } from '@angular/core/testing';

import { BreceptionService } from './breception.service';

describe('BreceptionService', () => {
  let service: BreceptionService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(BreceptionService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
