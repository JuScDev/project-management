import{a as T}from"./chunk-7DU6S6KI.js";import{a as W}from"./chunk-VOE6KC63.js";import"./chunk-Z3PJB6SC.js";import{a as v,b as w,c as D,d as k,e as B,f as E,g as I}from"./chunk-UZTVE72L.js";import{da as N,fa as F,h as M,j as _,m as C,n as P,q as b,r as j,s as h,t as O,u as y,w as S,x}from"./chunk-3KCAOOFU.js";import{Db as l,Pb as a,Sb as d,Tb as s,Ua as p,Ub as u,ba as c,eb as f,xb as t,yb as o}from"./chunk-7H4R236B.js";var R=class g{project={id:0,name:"",description:""};router=c(_);_route=c(M);_projectService=c(T);_snackBar=c(W);ngOnInit(){let r=Number(this._route.snapshot.paramMap.get("id"));this.loadProject(r)}loadProject(r){this._projectService.getProjectById(r).subscribe({next:n=>this.project=n,error:n=>this._snackBar.open("Error while loading project","Close",{duration:2e3})})}updateProject(){this._projectService.updateProject(this.project.id,this.project).subscribe({next:()=>this.router.navigate(["/dashboard"]),error:r=>this._snackBar.open("Error while updating project","Close",{duration:2e3})})}static \u0275fac=function(n){return new(n||g)};static \u0275cmp=f({type:g,selectors:[["app-project-detail"]],decls:17,vars:2,consts:[[3,"submit"],["appearance","outline"],["matInput","","name","name","required","",3,"ngModelChange","ngModel"],["matInput","","name","description","required","",3,"ngModelChange","ngModel"],[1,"button-group"],["mat-flat-button","",3,"click"],["mat-flat-button","","color","primary","type","submit"]],template:function(n,e){n&1&&(t(0,"mat-card")(1,"h2"),a(2,"Edit Project"),o(),t(3,"form",0),l("submit",function(){return e.updateProject()}),t(4,"mat-form-field",1)(5,"mat-label"),a(6,"Project Name"),o(),t(7,"input",2),u("ngModelChange",function(i){return s(e.project.name,i)||(e.project.name=i),i}),o()(),t(8,"mat-form-field",1)(9,"mat-label"),a(10,"Description"),o(),t(11,"input",3),u("ngModelChange",function(i){return s(e.project.description,i)||(e.project.description=i),i}),o()(),t(12,"div",4)(13,"button",5),l("click",function(){return e.router.navigate(["/dashboard"])}),a(14," Back "),o(),t(15,"button",6),a(16," Save Changes "),o()()()()),n&2&&(p(7),d("ngModel",e.project.name),p(4),d("ngModel",e.project.description))},dependencies:[x,y,P,b,j,S,O,h,C,w,v,B,k,D,I,E,F,N],styles:["[_nghost-%COMP%]{display:block;padding:20px}[_nghost-%COMP%]   mat-card[_ngcontent-%COMP%]{padding:20px;margin:0 auto;max-width:500px;width:100%;box-shadow:0 4px 8px #0000001a}[_nghost-%COMP%]   mat-card[_ngcontent-%COMP%]   h2[_ngcontent-%COMP%]{text-align:center;margin-bottom:20px}[_nghost-%COMP%]   mat-card[_ngcontent-%COMP%]   form[_ngcontent-%COMP%]{display:flex;flex-direction:column;align-items:center}[_nghost-%COMP%]   mat-card[_ngcontent-%COMP%]   form[_ngcontent-%COMP%]   mat-form-field[_ngcontent-%COMP%]{width:100%;margin-bottom:16px}[_nghost-%COMP%]   mat-card[_ngcontent-%COMP%]   form[_ngcontent-%COMP%]   button[type=submit][_ngcontent-%COMP%]{align-self:center}[_nghost-%COMP%]   mat-card[_ngcontent-%COMP%]   .button-group[_ngcontent-%COMP%]{display:flex;justify-content:center;align-items:center;gap:16px}[_nghost-%COMP%]   mat-card[_ngcontent-%COMP%]   .button-group[_ngcontent-%COMP%]   button[_ngcontent-%COMP%]:first-child{background-color:#fff;color:#3f51b5;border:1px solid #3f51b5}"]})};export{R as ProjectDetailComponent};
