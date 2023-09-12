import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import { userStateStore } from '../stores/UserStateStore'

function authorizeIsMember(to, from, next) {
  const userStore = userStateStore()
  if (to.name !== 'login' && !userStore.authenticate) {
    next({ name: 'login' })
  } else {
    next()
  }
}

function noShowAfterLogIn(to, from) {
  const userStore = userStateStore();
  if (userStore.authenticate) {
    return from;
  }
}

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView,
      meta: {
        title: "Bump"
      }
    },
    {
      path: '/about',
      name: 'about',
      component: () => import('../views/AboutView.vue')
    },
    {
      path: '/faq',
      name: 'faq',
      component: () => import('../views/CustomerService/FAQView.vue')
    },
    {
      path: '/privacy',
      name: 'privacy',
      component: () => import('../views/CustomerService/PrivacyView.vue'),
    },
    {
      path: '/terms',
      name: 'terms',
      component: () => import('../views/CustomerService/TermsView.vue'),
    },
    {
      path: '/course',
      name: 'course',
      component: () => import('../views/Course/CourseIndex.vue')
    },
    {
      path: '/SkillCourseDetial/:id',
      name: 'SkillCourseDetial',
      component: () => import('../views/Course/SkillCourseDetial.vue')
    },
    {
      path: '/SkillSelectField',
      name: '/SkillSelectField',
      component: () => import('../views/course/SkillSelectField.vue')
    },
    {
      path: '/SkillSelectTime/:id',
      name: '/SkillSelectTime',
      component: () => import('../views/course/SkillSelectTime.vue')
    },
    {
      path: '/skillcourseOrder/:id',
      name: '/skillcourseOrder',
      component: () => import('../views/course/skillcourseOrder.vue')
    },
    {
      path: '/ExperienceCourseSelectField',
      name: '/ExperienceCourseSelectField',
      component: () => import('../views/course/ExperienceCourseSelectField.vue'),
      beforeEnter: authorizeIsMember
    },
    {
      path: '/ExperienceCourseOrder/:id',
      name: '/ExperienceCourseOrder',
      component: () => import('../views/course/ExperienceCourseOrder.vue')
    },
    {
      path: '/ExperienceCourseFinish',
      name: '/ExperienceCourseFinish',
      component: () => import('../views/course/ExperienceCourseFinish.vue')
    },
    {
      path: '/Cart',
      name: '/Cart',
      component: () => import('../views/Cart/Cart.vue')
    },
    {
      path: '/contact-us',
      name: 'contactus',
      component: () => import('@/views/CustomerService/ContactUsView.vue')
    },
    {
      path: '/CheckOut',
      name: 'CheckOut',
      component: () => import('@/views/CheckOut/CheckOut.vue')
    },
    {
      path: '/:pathMatch(.*)*',
      name: 'NotFound',
      component: () => import('../views/NotFound.vue')
    },
    {
      path: '/shop',
      name: 'shop',
      component: () => import('../views/Shop/ShopIndex.vue'),
    },
    {

      path: '/ProductList',
      name: 'ProductList',
      component: () => import('../views/Shop/ProductList.vue'),
    },
    {

      path: '/products/:id',
      name: 'ProductDetails',
      component: () => import('../views/Shop/ProductDetails.vue'),
    },
    {
      path: '/Login',
      name: 'login',
      component: () => import('../views/Account/LoginView.vue'),
      beforeEnter: noShowAfterLogIn
    },
    {
      path: '/Register',
      name: 'register',
      component: () => import('../views/Account/RegisterView.vue'),
      beforeEnter: noShowAfterLogIn
    },
    {
      path: '/Account/OAuthRegister',
      name: 'OAuthRegister',
      component: () => import('../views/Account/OAuthRegisterView.vue')
    },
    {
      path: '/VerifyRegister',
      name: 'verifyRegister',
      component: () => import('../views/Account/VerifyRegisterView.vue')
    },
    {
      path: '/ForgetPwd',
      name: 'forgetPwd',
      component: () => import('../views/Account/ForgetPwdView.vue')
    },
    {
      path: '/ChangePwd',
      name: 'ChangePwd',
      component: () => import('../views/Account/ChangePwdView.vue')
    },
    {
      path: '/VerifyGoogle',
      name: 'VerifyGoogle',
      component: () => import('../views/Account/VerifyGoogle.vue')
    },
    {
      path: '/Member',
      component: () => import('../views/Member/MemberIndexView.vue'),
      beforeEnter: authorizeIsMember,
      children: [
        {
          path: 'Index',
          name: 'index',
        },
        {
          path: 'Orders',
          name: 'orders',
          component: () => import('../views/Member/MemberOrderView.vue')
        },
        {
          path: 'Courses',
          name: 'courses',
          component: () => import('../views/Member/MemberCourseView.vue')
        },
        {
          path: 'Coupons',
          name: 'coupons',
          component: () => import('../views/Member/MemberCouponView.vue')
        }
      ],
    },
    {
      path: '/pay/line',
      name: "LinePayRequest",
      component: () => import('@/views/Pay/LinePayView.vue'),
    },
    {
      path: '/pay/line/confirm',
      name: "LinePayRequestConfirm",
      component: () => import('@/views/Pay/LinePayConfirmView.vue'),
    },
    {
      path: '/admin/reply',
      name: "Demo reply",
      component: () => import('@/views/CustomerService/RealtimeReplyView.vue')
    },
    {
      path: "/Activity/:id",
      name: "activity",
      component: () => import("../views/Activity/ActivityDetailsView.vue"),
    },
    {
      path: "/pay/ecpay/submit",
      component: () => import("@/views/Pay/ECPayView.vue"),
    },
    {
      path: "/Shop/OnSale",
      name: "onSale",
      component: () => import("../views/Activity/OnSaleProductList.vue"),
    },
    {
      path: "/BrandsIndex",
      name: "brandsIndex",
      component: () => import("../views/Shop/BrandsIndex.vue"),
    }
  ],
});

router.beforeEach(() => {
  const userState = userStateStore();
  if (userState.authenticate) {
    localStorage.removeItem('userStore');
    userState.GetUserInfo();
  }
});

export default router;
