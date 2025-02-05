import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ProjectService {
  private apiUrl = 'https://localhost:5001/api/projects';

  private _http = inject(HttpClient);

  public getProjects(): Observable<any[]> {
    return this._http.get<any[]>(this.apiUrl);
  }

  public getProjectById(id: number): Observable<any> {
    return this._http.get<any>(`${this.apiUrl}/${id}`);
  }

  public createProject(project: any): Observable<any> {
    return this._http.post<any>(this.apiUrl, project);
  }

  public updateProject(id: number, project: any): Observable<any> {
    return this._http.put<any>(`${this.apiUrl}/${id}`, project);
  }

  public deleteProject(id: number): Observable<any> {
    return this._http.delete<any>(`${this.apiUrl}/${id}`);
  }
}
