import { Routes } from '@angular/router';
import { LoginComponent } from './pages/login/login.component';
import { MainLayoutComponent } from './layout/main-layout/main-layout.component';
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { ArticleListComponent } from './pages/article-list/article-list.component';
import { BlivraisonListComponent } from './pages/blivraison-list/blivraison-list.component';
import { BreceptionListComponent } from './pages/breception-list/breception-list.component';
import { BeneficeListComponent } from './pages/benefice-list/benefice-list.component';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
    { path: 'login', component: LoginComponent },
    {
        path: '',
        component: MainLayoutComponent,
        canActivate: [authGuard],
        children: [
            { path: 'dashboard', component: DashboardComponent },
            { path: 'articles', component: ArticleListComponent },
            { path: 'blivraisons', component: BlivraisonListComponent },
            { path: 'breceptions', component: BreceptionListComponent },
            { path: 'benefices', component: BeneficeListComponent },
            { path: '', redirectTo: 'dashboard', pathMatch: 'full' }
        ]
    },
    { path: '**', redirectTo: 'login' }
];