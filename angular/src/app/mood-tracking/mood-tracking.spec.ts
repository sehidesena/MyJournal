import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MoodTracking } from './mood-tracking';

describe('MoodTracking', () => {
  let component: MoodTracking;
  let fixture: ComponentFixture<MoodTracking>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MoodTracking]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MoodTracking);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
