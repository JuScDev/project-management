import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ProjectService } from '../../services/project.service';
import { FormsModule } from '@angular/forms';
import { Project } from '../../models/project.model';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-project-detail',
  templateUrl: './project-detail.component.html',
  styleUrls: ['./project-detail.component.scss'],
  imports: [
    FormsModule,
    RouterModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
  ],
})
export class ProjectDetailComponent implements OnInit {
  public project: Project = { id: 0, name: '', description: '' };

  public router = inject(Router);

  public _route = inject(ActivatedRoute);

  public _projectService = inject(ProjectService);

  private _snackBar = inject(MatSnackBar);

  public ngOnInit(): void {
    const projectId = Number(this._route.snapshot.paramMap.get('id'));
    this.loadProject(projectId);
  }

  public loadProject(projectId: number): void {
    this._projectService.getProjectById(projectId).subscribe({
      next: (data) => (this.project = data),
      error: (err) =>
        this._snackBar.open('Error while loading project', 'Close', {
          duration: 2000,
        }),
    });
  }

  public updateProject(): void {
    this._projectService
      .updateProject(this.project.id, this.project)
      .subscribe({
        next: () => this.router.navigate(['/dashboard']),
        error: (err) =>
          this._snackBar.open('Error while updating project', 'Close', {
            duration: 2000,
          }),
      });
  }
}
