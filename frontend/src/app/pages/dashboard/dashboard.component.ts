import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
})
export class DashboardComponent implements OnInit {
  projects: any[] = [];

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.loadProjects();
  }

  loadProjects(): void {
    this.http.get<any[]>('https://localhost:5001/api/projects').subscribe({
      next: (data) => (this.projects = data),
      error: (err) => console.error('Fehler beim Laden der Projekte:', err),
    });
  }
}
