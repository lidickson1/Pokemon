import {
  HttpClientTestingModule,
  HttpTestingController,
} from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AppComponent } from './app.component';
import { FormsModule } from '@angular/forms';
import { By } from '@angular/platform-browser';

describe('AppComponent', () => {
  let component: AppComponent;
  let fixture: ComponentFixture<AppComponent>;
  let httpMock: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AppComponent],
      imports: [HttpClientTestingModule, FormsModule],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AppComponent);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    // httpMock.verify();
  });

  it('should create the app', () => {
    expect(component).toBeTruthy();
  });

  it('should retrieve pokemons from the server', () => {
    const mockData = [
      { id: 12, name: 'butterfree', type: 'bug', wins: 7, losses: 0, ties: 0 },
      { id: 91, name: 'cloyster', type: 'water', wins: 6, losses: 1, ties: 0 },
      {
        id: 122,
        name: 'mr-mime',
        type: 'psychic',
        wins: 5,
        losses: 2,
        ties: 0,
      },
      {
        id: 83,
        name: 'farfetchd',
        type: 'normal',
        wins: 4,
        losses: 3,
        ties: 0,
      },
      { id: 33, name: 'nidorino', type: 'poison', wins: 3, losses: 4, ties: 0 },
      { id: 109, name: 'koffing', type: 'poison', wins: 2, losses: 5, ties: 0 },
      { id: 98, name: 'krabby', type: 'water', wins: 0, losses: 6, ties: 1 },
      { id: 88, name: 'grimer', type: 'poison', wins: 0, losses: 6, ties: 1 },
    ];

    component.ngOnInit();

    const req = httpMock.expectOne(
      '/pokemon/tournament/statistics?sortBy=wins&sortDirection=desc'
    );
    expect(req.request.method).toEqual('GET');
    req.flush(mockData);

    expect(component.pokemons).toEqual(mockData);
  });

  it('should update the select values', () => {
    fixture.detectChanges();

    const selectElements = fixture.debugElement.queryAll(By.css('select'));
    const sortBy: HTMLSelectElement = selectElements[0].nativeElement;
    const sortDirection = selectElements[1].nativeElement;

    sortBy.value = 'id';
    sortBy.dispatchEvent(new Event('change'));
    fixture.detectChanges();
    expect(component.sortBy).toBe('id');

    sortDirection.value = 'asc';
    sortDirection.dispatchEvent(new Event('change'));
    fixture.detectChanges();
    expect(component.sortDirection).toBe('asc');
  });

  it('should make an HTTP request', () => {
    const button: HTMLButtonElement = fixture.debugElement.query(
      By.css('button')
    ).nativeElement;
    button.click();
    const req = httpMock.expectOne(
      `/pokemon/tournament/statistics?sortBy=${component.sortBy}&sortDirection=${component.sortDirection}`
    );
    expect(req.request.method).toBe('GET');
    req.flush(null);
  });
});
