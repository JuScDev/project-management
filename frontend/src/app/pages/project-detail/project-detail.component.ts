import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ProjectService } from '../../services/project.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-project-detail',
  templateUrl: './project-detail.component.html',
  styleUrls: ['./project-detail.component.scss'],
  imports: [FormsModule, RouterModule],
})
export class ProjectDetailComponent implements OnInit {
  public project: any = { name: '', description: '' };
  public projectId!: number;

  public router = inject(Router);

  public _route = inject(ActivatedRoute);

  public _projectService = inject(ProjectService);

  public ngOnInit(): void {
    this.projectId = Number(this._route.snapshot.paramMap.get('id'));
    this.loadProject();
  }

  public loadProject(): void {
    this._projectService.getProjectById(this.projectId).subscribe({
      next: (data) => (this.project = data),
      error: (err) => console.error('Error while loading project:', err),
    });
  }

  public updateProject(): void {
    this._projectService.updateProject(this.projectId, this.project).subscribe({
      next: () => this.router.navigate(['/dashboard']),
      error: (err) => console.error('Error while updating project:', err),
    });
  }
}
