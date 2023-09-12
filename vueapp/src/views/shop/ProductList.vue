<script setup>
import { useTitle } from '@vueuse/core';
import DefaultLayout from '../../components/layouts/DefaultLayout.vue';
import ShopNavbar from '../../components/shop/layouts/ShopNavbar.vue';
import ProductCard from '../../components/shop/product/ProductCard.vue';
import ScrollToTop from '../../components/shop/layouts/ScrollToTop.vue';
import Breadcrumbs from '../../components/shop/layouts/Breadcrumbs.vue';
import ProductListFilter from '../../components/shop/product/ProductListFilter.vue';
import { ref, onMounted, watch } from 'vue';
import { useRoute } from 'vue-router';
import axios from 'axios';
import { ElSelect, ElOption } from 'element-plus';
import 'element-plus/es/components/skeleton-item/style/css'
import 'element-plus/es/components/skeleton/style/css'
import { mdiConsoleNetworkOutline } from '@mdi/js';
import { toggleRowStatus } from 'element-plus/es/components/table/src/util';

useTitle('Bump - 商品列表');

const productList = ref(0);
const page = ref(1);
const totalPage = ref(1);
const route = useRoute();
const showSpinner = ref(false);
const selectedProductsOrderKey = ref();
const productsOrderItem = ref([{ key: 1, text: '日期由新到舊' }, { key: 2, text: '日期由舊到新' }, { key: 3, text: '價格由低到高' }, { key: 4, text: '價格由高到低' }])

const breadcrumbs = ref([{
    title: '商品列表',
    disabled: false,
    href: 'productlist',
}]);

const filter = ref({
    keyword: '',
    page: 1,
    pageSize: 20,
    firstCategory: null,
    secondCategories: [],
    thirdCategories: [],
    brandName: [],
    productStyle: [],
    minPrice: null,
    maxPrice: null,
    orderKey: 0
});

async function loadProducts(data) {
    data.orderKey = selectedProductsOrderKey;
    try {
        const getProductList = await axios.post(`/api/Shop/ProductList`, data);
        productList.value = getProductList.data.products;
        totalPage.value = getProductList.data.totalPage;
        setTimeout(scrollToTop, 150);
    } catch (err) {
        productList.value = 0;
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
    if (route.query["navbarFilter"]) {
        filter.value.firstCategory = route.query["navbarFilter"] ?? null;
        breadcrumbs.value = ([
            {
                title: '商品列表',
                disabled: false,
                href: 'productlist',
            },
            {
                title: route.query["navbarFilter"],
                disabled: true,
                href: 'productlist?navbarFilter=' + route.query["navbarFilter"]
            }
        ]);
        if (route.query["mid"]) {
            filter.value.secondCategories.push(route.query["mid"]);
            breadcrumbs.value[1].disabled = false;
            breadcrumbs.value.push(
                {
                    title: route.query["mid"],
                    disabled: true,
                    href: 'productlist?navbarFilter=' + route.query["navbarFilter"] + '&mid=' + route.query["mid"]
                });
        }
        if (route.query["sub"]) {
            filter.value.thirdCategories.push(route.query["sub"]);
            breadcrumbs.value[2].disabled = false;
            breadcrumbs.value.push(
                {
                    title: route.query["sub"],
                    disabled: true,
                    href: 'productlist?navbarFilter=' + route.query["navbarFilter"] + '&mid=' + route.query["mid"] + '&sub=' + route.query["sub"]
                });
        }
    }
    filter.value.keyword = route.query["keyword"] ?? null;
    if (route.query.hasOwnProperty("keyword")) {
        breadcrumbs.value = ([
            {
                title: '商品列表',
                disabled: false,
                href: '/productlist',
            },
            {
                title: '搜尋',
                disabled: false,
                href: route.fullPath,
            },
            {
                title: route.query["keyword"],
                disabled: true,
                href: route.fullPath,
            }
        ]);
    }
    setTimeout(() => {
        showSpinner.value = true;
    }, 300);

    loadProducts(filter.value);
});


watch(() => route.query["keyword"], (newKeyword) => {
    filter.value = {
        keyword: '',
        page: 1,
        pageSize: 20,
        firstCategory: null,
        secondCategories: [],
        thirdCategories: [],
        brandName: [],
        productStyle: [],
        minPrice: null,
        maxPrice: null,
        orderKey: 0
    };
    if (newKeyword) {
        filter.value.keyword = route.query["keyword"];
        page.value = 1;
        loadProducts(filter.value);
    }
    breadcrumbs.value = ([
        {
            title: '商品列表',
            disabled: false,
            href: 'productlist',
        },
        {
            title: '搜尋',
            disabled: false,
            href: route.fullPath,
        },
        {
            title: route.query["keyword"],
            disabled: true,
            href: route.fullPath,
        }
    ]);
});

watch(page, async () => {
    filter.value.page = page.value;
    loadProducts(filter.value);
});
const getfilterSelected = (data) => {
    filter.value.brandName = data.brandSelected ? data.brandSelected : null;

    filter.value.secondCategories = route.query["mid"] ? [route.query["mid"]] : [];
    data.secondCategorySelected ? filter.value.secondCategories.push(...data.secondCategorySelected) : null;

    filter.value.thirdCategories = route.query["sub"] ? [route.query["sub"]] : [];
    data.thirdCategorySelected ? filter.value.thirdCategories.push(...data.thirdCategorySelected) : null;

    filter.value.minPrice = data.minPrice ? data.minPrice : null;
    filter.value.maxPrice = data.maxPrice ? data.maxPrice : null;

    filter.value.page = 1;
    loadProducts(filter.value);
};

function handleProductsOrderChange(orderKey) {
    filter.value.orderKey = orderKey;
    loadProducts(filter.value);
}
</script>

<template>
    <DefaultLayout>
        <ShopNavbar></ShopNavbar>
        <main>
            <v-container class="mw-1300px">
                <v-row no-gutters>
                    <Breadcrumbs :items="breadcrumbs" />
                </v-row>
                <v-row no-gutters>
                    <ProductListFilter @filterSelected="getfilterSelected" />
                    <div v-if="productList != null && productList != 0" class="w-100 d-flex justify-end mt-5 mb-n10 px-9">
                        <div>
                            <el-select v-model="selectedProductsOrderKey"
                                @change="handleProductsOrderChange(selectedProductsOrderKey)" class="m-2"
                                placeholder="請選擇排序">
                                <el-option v-for="order in productsOrderItem" :key="order.key" :label="order.text"
                                    :value="order.key" />
                            </el-select>
                        </div>
                    </div>
                </v-row>
                <v-row class="px-10 pb-5" v-if="productList != null && productList != 0">
                    <v-col cols="12" lg="3" md="4" sm="6" xs="12" v-for="  product   in   productList  " :key="product.id">
                        <ProductCard :id="product.id" :name="product.name" :brandName="product.brandName"
                            :price="product.price" :thumbnail="product.thumbnail" class="mb-0">
                        </ProductCard>
                    </v-col>
                </v-row>
                <v-row v-else-if="productList == null">
                    <v-col class="d-flex justify-center my-10" :style="{ height: 'calc(100vh - 628px)' }">
                        <h2 class="mx-auto">查無商品 !</h2>
                    </v-col>
                </v-row>
                <v-row no-gutters v-else-if="productList == 0">
                    <v-col class="d-flex justify-center my-10" :style="{ height: 'calc(100vh - 628px)' }">
                        <v-progress-circular v-if="showSpinner" indeterminate color="amber" :size="57"
                            :width="8"></v-progress-circular>
                    </v-col>
                </v-row>
                <v-row justify="center">
                    <v-col cols="8">
                        <v-pagination v-model="page" class="my-4" :length="totalPage"></v-pagination>
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

.panel.panel-default {
    border-color: rgba(0, 0, 0, .2);
}
</style>