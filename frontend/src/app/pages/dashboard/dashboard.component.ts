import { Component, inject, OnInit } from '@angular/core';
import { ProjectService } from '../../services/project.service';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { Project } from '../../models/project.model';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
  imports: [
    FormsModule,
    RouterModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatListModule,
    MatIconModule,
  ],
})
export class DashboardComponent implements OnInit {
  public projects: Array<Project> = [];

  public newProject: Project = { id: 0, name: '', description: '' };

  private _projectService = inject(ProjectService);

  private _snackBar = inject(MatSnackBar);

  public ngOnInit(): void {
    this.loadProjects();
  }

  public loadProjects(): void {
    this._projectService.getProjects().subscribe({
      next: (data) => (this.projects = data),
      error: (err) =>
        this._snackBar.open('Failed to load projects', 'Close', {
          duration: 2000,
        }),
    });
  }

  public createProject(): void {
    this._projectService.createProject(this.newProject).subscribe({
      next: () => {
        this.loadProjects();
        this._snackBar.open('Project created successfully!', 'Close', {
          duration: 2000,
        });
        this.newProject = { id: 0, name: '', description: '' };
      },
      error: (err) =>
        this._snackBar.open('Failed to create project', 'Close', {
          duration: 2000,
        }),
    });
  }

  public deleteProject(id: number): void {
    this._projectService.deleteProject(id).subscribe({
      next: () => {
        this.loadProjects(),
          this._snackBar.open('Project deleted!', 'Close', { duration: 2000 });
      },
      error: (err) =>
        this._snackBar.open('Failed to delete project', 'Close', {
          duration: 2000,
        }),
    });
  }
}
