<script setup>
import { computed, ref, onMounted,watch } from 'vue';
import { mdiCartOutline, mdiAccountOutline, mdiMenu } from '@mdi/js';
import { mdiClock, mdiAccount, mdiFlag } from '@mdi/js';
import logoUrl from '../../assets/siteLOGO.png';
import { userStateStore } from '../../stores/UserStateStore';
import { useCartStore } from '../../stores/CartStore';
import AccountLogInHeader from './AccountLogInHeader.vue';
import AccountLogoutHeader from './AccountLogoutHeader.vue';
import CartHeader from './CartHeader.vue';
import { useRouter } from 'vue-router';
import { ElMessage } from 'element-plus'
import axios from 'axios';

const store = userStateStore()
const isAuthenticated = computed(() => store.authenticate)
const router = useRouter();
const items = ref([
    { text: '個人化中心', icon: mdiClock },
    { text: '會員紅利', icon: mdiAccount },
    { text: '商品訂單', icon: mdiFlag },
])

const cartStore = useCartStore();
const cartItems = ref([]);
const deleteBtn = computed(()=> cartStore.items)
// console.log(store.userId);

watch(deleteBtn, function(){
    cartItems.value = deleteBtn.value;
})


onMounted(async () => {
    try {
        if (isAuthenticated.value) {
            await cartStore.updateCartItems();
            cartItems.value = cartStore.items;
        }

    } catch (err) {
        console.error(err);
    }
})

function memberCart() {
    if (isAuthenticated.value) {
        router.push('/cart');
        return;
    }

    ElMessage({
        showClose: true,
        message: '請先登入!',
        type: 'warning',
    })

    setTimeout(() => {
        router.push('/login');
    }, 500);
}

const links = ref([
    {
        to: "/shop",
        text: "商城"
    },
    {
        to: "/course",
        text: "課程"
    },
    {
        to: "/about",
        text: "關於我們"
    },
    {
        to: "/faq",
        text: "常見問題"
    },
]);

</script>

<template>
    <div class="header-container">
        <v-container class="pa-0">
            <v-toolbar prominent>
                <v-row no-gutters>
                    <v-col cols="4" lg="4" class="d-lg-flex d-none">
                        <v-btn v-for="link in links" :to="link.to" :key="link.to"
                            class="ma-0 h-100 pa-0 px-xl-6 px-lg-3 text-white text-h6">
                            {{ link.text }}
                        </v-btn>
                    </v-col>
                    <v-col cols="4" class="d-md-flex d-lg-none">
                        <v-menu>
                            <template v-slot:activator="{ props }">
                                <v-btn :icon="mdiMenu" color="white" v-bind="props"></v-btn>
                            </template>

                            <v-list bg-color="#E6A947" color="white">
                                <v-list-item v-for="link in links" :to="link.to" :key="link.to">
                                    <v-list-item-title class="list-item-title">{{ link.text }}</v-list-item-title>
                                </v-list-item>
                            </v-list>
                        </v-menu>
                    </v-col>

                    <v-col cols="4" lg="4" class="d-flex justify-center">
                        <a href="/" class="w-100">
                            <v-img :height="48" :src="logoUrl"></v-img>
                        </a>
                    </v-col>
                    <v-col cols="4" class="d-flex justify-end">
                        <template v-if="isAuthenticated">
                            <AccountLogInHeader></AccountLogInHeader>
                        </template>
                        <template v-else>
                            <AccountLogoutHeader></AccountLogoutHeader>
                        </template>
                        <v-menu open-on-hover>
                            <template v-slot:activator="{ props }">
                                <v-btn icon class="ml-4" @click="memberCart()" v-bind="props">
                                    <v-badge v-if="isAuthenticated" :content="cartItems.length" color="amber-accent-4"
                                        text-color="white">
                                        <v-icon :icon="mdiCartOutline" color="white" size="large"></v-icon>
                                    </v-badge>
                                    <v-icon v-else :icon="mdiCartOutline" color="white" size="large"></v-icon>
                                </v-btn>
                            </template>
                            <CartHeader :cartItems="cartItems" @btnClicked="deleteBtnClicked"/>
                        </v-menu>
                    </v-col>
                </v-row>
            </v-toolbar>
        </v-container>
    </div>
</template>

<style scoped>
.header-container {
    height: 66px;
    background: linear-gradient(90deg, #E6A947 40%, #D68924);
    width: 100%;
    position: sticky;
    top: 0;
    z-index: 999;
    box-shadow: 0px 10px 10px -4px rgba(0, 0, 0, 0.12);
    -webkit-box-shadow: 0px 10px 10px -4px rgba(0, 0, 0, 0.12);
    -moz-box-shadow: 0px 10px 10px -4px rgba(0, 0, 0, 0.12);
}

a.v-btn:hover {
    background: radial-gradient(closest-side, rgba(255, 255, 255, 0.1), rgba(255, 255, 255, 0));
}

header.v-toolbar {
    background: transparent;
}

.list-item-title {
    letter-spacing: 0.25rem;
    text-align: center;
    color: white;
}
</style>
