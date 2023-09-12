<script setup>
import DefaultLayout from '../../components/layouts/DefaultLayout.vue';
import ShopNavbar from '../../components/shop/layouts/ShopNavbar.vue';
import ProductCard from '../../components/shop/product/ProductCard.vue';
import ScrollToTop from '../../components/shop/layouts/ScrollToTop.vue';
import { ref, onMounted, watch } from 'vue';
import { useRoute } from 'vue-router';
import axios from 'axios';
import ActivityCarousel from '../../components/Activity/ActivityCarousel.vue';
import { useTitle } from '@vueuse/core';

useTitle('Bump - 優惠商品');

const onSaleProductList = ref([]);
const page = ref(1);
const totalPage = ref(1);
const route = useRoute();
const headerImg = new URL('/src/assets/images/activity/onsale.jpeg', import.meta.url).href
const onSaleLink = 'https://localhost:5002/Shop/OnSale';
const pageLink = decodeURIComponent(window.location.href);

const filter = ref({
    page: 1,
    pageSize: 20,
    productTagNames: [],
    firstCategory: null,
    secondCategories: [],
    thirdCategories: [],
    brandName: [],
    productStyle: [],
    minPrice: null,
    maxPrice: null
});

async function loadProducts(data) {
    try {
        const res = await axios.post(`/api/OnSale/OnSaleProductList`, data);
        onSaleProductList.value = res.data.products;
        totalPage.value = res.data.totalPage;
        setTimeout(scrollToTop, 150);
    } catch (err) {
        onSaleProductList.value = null;
        console.error(err);
    }
}
function scrollToTop() {
    window.scrollTo({
        top: 0,
        behavior: "smooth",
    });
}
onMounted(async () => {
    if (route.query["actName"]) {
        filter.value.productTagNames.push(route.query["actName"]);
    }
    loadProducts(filter.value);
});

watch(page, async () => {
    filter.value.page = page.value,

        // firstCategory: clickNavbar.value,
        // secondCategories: null,
        // thirdCategories: null,
        // brandName: null,
        // productStyle: null,
        // minPrice: null,
        // maxPrice: null

        loadProducts(filter.value);
});

</script>

<template>
    <DefaultLayout>
        <ShopNavbar></ShopNavbar>
        <v-img v-if="pageLink === onSaleLink" class="bg-white" width="100%" height="300" :aspect-ratio="1" :src="headerImg"
            cover>
            <v-row class="fill-height d-flex align-center justify-center">
                <v-col cols="12" class="text-center">
                    <v-card-title class="text-white text-h2  font-weight-black">優惠商品</v-card-title>
                </v-col>
            </v-row>
        </v-img>
        <v-container v-else class="mw-1300px mt-5">
            <h1>{{ filter.productTagNames[0] }}</h1>
        </v-container>
        <main>
            <v-container class="mw-1300px">
                <ActivityCarousel></ActivityCarousel>
                <v-row class="pa-10" v-if="onSaleProductList">
                    <v-col cols="12" lg="3" md="4" sm="6" xs="12" v-for="product in onSaleProductList" :key="product.id">
                        <ProductCard :id="product.id" :name="product.name" :brandName="product.brandName"
                            :price="product.price" :thumbnail="product.thumbnail" class="mb-0">
                        </ProductCard>
                    </v-col>
                </v-row>
                <v-row v-else>
                    <v-col class="d-flex justify-center my-10">
                        <h2 class="mx-auto">查無商品 !</h2>
                    </v-col>
                </v-row>
                <v-row justify="center">
                    <v-col cols="8">
                        <v-container class="max-width">
                            <v-pagination v-model="page" class="my-4" :length="totalPage"></v-pagination>
                        </v-container>
                    </v-col>
                </v-row>
            </v-container>
            <ScrollToTop />
        </main>
    </DefaultLayout>
</template>
    
<style scoped>
@media (min-width: 1920px) {
    .mw-1300px {
        max-width: 1300px;
    }
}
</style>