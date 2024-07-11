import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

type Pokemon = {
  id: number;
  name: string;
  type: string;
  wins: number;
  losses: number;
  ties: number;
};

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent implements OnInit {
  public pokemons: Pokemon[] = [];
  sortBy: 'wins' | 'losses' | 'ties' | 'name' | 'id' = 'wins';
  sortDirection: 'asc' | 'desc' = 'desc';

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.getPokemons();
  }

  getPokemons() {
    this.http
      .get<Pokemon[]>(
        `/pokemon?sortBy=${this.sortBy}&sortDirection=${this.sortDirection}`
      )
      .subscribe(
        (result) => {
          this.pokemons = result;
        },
        (error) => {
          console.error(error);
        }
      );
  }

  title = 'pokemon.client';
}
