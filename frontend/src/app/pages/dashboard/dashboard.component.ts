import { Component, inject, OnInit } from '@angular/core';
import { ProjectService } from '../../services/project.service';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { Project } from '../../models/project.model';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
  imports: [FormsModule, RouterModule],
})
export class DashboardComponent implements OnInit {
  public projects: Array<Project> = [];

  public newProject: Project = { id: 0, name: '', description: '' };

  private _projectService = inject(ProjectService);

  public ngOnInit(): void {
    this.loadProjects();
  }

  public loadProjects(): void {
    this._projectService.getProjects().subscribe({
      next: (data) => (this.projects = data),
      error: (err) => console.error('Error while loading projects:', err),
    });
  }

  public createProject(): void {
    this._projectService.createProject(this.newProject).subscribe({
      next: () => {
        this.loadProjects();
        this.newProject = { id: 0, name: '', description: '' };
      },
      error: (err) => console.error('Error while creating project:', err),
    });
  }

  public deleteProject(id: number): void {
    this._projectService.deleteProject(id).subscribe({
      next: () => this.loadProjects(),
      error: (err) => console.error('Error while deleting project:', err),
    });
  }
}
