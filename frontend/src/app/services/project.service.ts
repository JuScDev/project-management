import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Project } from '../models/project.model';

@Injectable({
  providedIn: 'root',
})
export class ProjectService {
  private apiUrl = 'https://localhost:5001/api/projects';

  private _http = inject(HttpClient);

  public getProjects(): Observable<Array<Project>> {
    return this._http.get<Array<Project>>(this.apiUrl);
  }

  public getProjectById(id: number): Observable<Project> {
    return this._http.get<Project>(`${this.apiUrl}/${id}`);
  }

  public createProject(project: Project): Observable<Project> {
    return this._http.post<Project>(this.apiUrl, project);
  }

  public updateProject(id: number, project: Project): Observable<Project> {
    return this._http.put<Project>(`${this.apiUrl}/${id}`, project);
  }

  public deleteProject(id: number): Observable<void> {
    return this._http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
