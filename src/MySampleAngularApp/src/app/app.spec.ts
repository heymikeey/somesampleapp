import { TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { AppComponent } from './app.component'; // Corrected import path and component name

describe('AppComponent', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        RouterTestingModule,
        AppComponent // NEW: Import AppComponent as it is now standalone
      ]
      // declarations: [ // REMOVED: Standalone components are imported, not declared
      //   AppComponent
      // ],
    }).compileComponents();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });

  it(`should have as title 'MySampleAngularApp'`, () => {
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.componentInstance;
    expect(app.title).toEqual('MySampleAngularApp');
  });

  it('should render title', () => {
    const fixture = TestBed.createComponent(AppComponent);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    // Note: The default Angular CLI template for new projects might have changed this text.
    // If your template is different, adjust this expectation accordingly.
    expect(compiled.querySelector('h1')?.textContent).toContain('MySampleAngularApp');
  });
});