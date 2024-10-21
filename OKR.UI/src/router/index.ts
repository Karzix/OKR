import { createRouter, createWebHistory } from 'vue-router'
//@ts-ignore
import Cookies from "js-cookie";
import layout1 from '@/components/Layout/layout1.vue'
import LayoutBlank from '@/components/Layout/LayoutBlank.vue';

import HomePageView from '@/views/HomeView.vue'
import LoginVue from '@/views/Auth/Login.vue';
import UserVue from '@/views/User/Index.vue'
import BranchVue from '@/views/Department/Branch.vue'
import TeamVue from '@/views/Department/Team.vue'
import { LoginResult } from '@/Models/LoginResult';
import UserInfor from '@/views/OKR/UserInfor.vue'
import SeeObjectives from '@/views/OKR/SeeObjectives.vue';
import layout2 from '@/components/Layout/layout2.vue';
import Layout2 from '@/components/Layout/layout2.vue';

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: "/",
      component: Layout2,
      meta: { requiresAuth: true },
      children: [
        {
          path: "",
          component: HomePageView,
          meta: { requiresAuth: true},
        },
        {
          path: "User",
          component: UserVue,
          meta: { requiresAuth: true, roles: ["Admin", "superadmin"]},
        },
        {
          path: "Branch",
          component: BranchVue,
          meta: { requiresAuth: true, roles: ["Admin", "superadmin"]},
        },
        {
          path: "Team",
          component: TeamVue,
          meta: { requiresAuth: true, roles: ["BranchManagement"]},
        },
      ],
    },
    {
      path: "/login",
      component: LayoutBlank,
      meta: { requiresAuth: false },
      children: [
        {
          path: "",
          component: LoginVue,
        },
      ],
    },
    {
      path: "/",
      component: Layout2,
      meta: { requiresAuth: false },
      children: [
        {
          path: "UserName=:UserName",
          component: UserInfor,
          meta: { requiresAuth: true},
        },
        {
          path: "Objectives=:EntityObjectiveId&:targetTpye",
          component: SeeObjectives,
          meta: { requiresAuth: true},
        },
      
      ],
    },
    
  ]
})

router.beforeEach((to, from, next) => {
  const isAuthenticated: boolean = !!Cookies.get('accessToken');
  const userRoles: string[] = getRolesFromToken() ??[];

  if (to.meta.requiresAuth && isAuthenticated == false) {
    next('/login'); 
  } else if (to.meta.roles && !hasPermission(userRoles, to.meta.roles as string[])) {
    next('/'); 
  } else {
    next();
  }
});

function hasPermission(userRoles: string[], requiredRoles: string[]): boolean {
  for (const requiredRole of requiredRoles) {
    if (userRoles.includes(requiredRole)) {
      return true;
    }
  }
  return false;
}
function getRolesFromToken(): string[] | null {
  try {
    // var token = Cookies.get('accessToken')?.toString() ?? "";
    const decodedToken = new LoginResult();
    var jsonString = Cookies.get('Roles')?.toString() ?? '';
    var jsonObject = JSON.parse(jsonString);
    var arrayFromString = Object.values(jsonObject);
    decodedToken.roles = arrayFromString as string[];
    console.log(decodedToken);
    return decodedToken.roles || [];
  } catch (error) {
    // console.error(error);
    return null;
  }
}
interface TokenPayload {
  [x: string]: never[];
}
export default router
