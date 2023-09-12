<script setup>
import { useTitle } from '@vueuse/core';
import DefaultLayout from '../../components/layouts/DefaultLayout.vue';
import FirstCarousel from '../../components/shop/FirstCarousel.vue';
import ShopNavbar from '../../components/shop/layouts/ShopNavbar.vue';
import ProductCardGroup from '../../components/shop/product/ProductCardGroup.vue';
import ScrollToTop from '../../components/shop/layouts/ScrollToTop.vue';
import { ref, onMounted } from 'vue';
import axios from 'axios';
import NewIcon from '@/assets/images/shop/newicon.png';

useTitle('Bump - 商品首頁');

const newProductGroup = ref([
    {
        name: "", brandName: "", price: "", thumbnail: "", totalPage: ""
    },
])
const sellsWellProductGroup = ref([
    {
        name: "", brandName: "", price: "", thumbnail: "", totalPage: ""
    },
])
const onSaleProductGroup = ref([
    {
        name: "", brandName: "", price: "", thumbnail: "", totalPage: ""
    },
])
onMounted(async () => {
    try {
        const getNewProductGroup = await axios.get('/api/shop/NewProductGroup');
        newProductGroup.value = getNewProductGroup.data;

        const getSellsWellProductGroup = await axios.get('/api/shop/SellsWellProductGroup');
        sellsWellProductGroup.value = getSellsWellProductGroup.data;

        const getOnSaleProductGroup = await axios.get('/api/ActivityDiscounts/OnSaleProductGroup');
        onSaleProductGroup.value = getOnSaleProductGroup.data;
    } catch (err) {
        console.error(err);
    }
})
</script>

<template>
    <DefaultLayout>
        <ShopNavbar></ShopNavbar>
        <main>
            <FirstCarousel></FirstCarousel>
            <!-- 一、icon用相對路徑: appendIcon、prependIcon  二、icon用html語法:傳入slot-->
            <ProductCardGroup title="新上市" :products="newProductGroup" link="/productlist" :appendIcon="NewIcon">
            </ProductCardGroup>

            <ProductCardGroup title="促銷中，趕緊手刀下單 " :products="onSaleProductGroup" link="https://localhost:5002/Shop/OnSale">
                <template #icon-element-append>
                    <i class="fa-solid fa-exclamation fa-beat fa-sm" style="color: black;"></i>
                </template>
            </ProductCardGroup>
            <ProductCardGroup title=" 大家愛不釋手的商品 " :products="sellsWellProductGroup" link="/productlist" class="my-16">
                <template #icon-element-append>
                    <i class="fa-regular fa-star fa-fade fa-sm" style="color: #ffc800;"></i>
                </template>
                <template #icon-element-prepend>
                    <i class="fa-regular fa-star fa-fade fa-sm" style="color: #ffc800;"></i>
                </template>
            </ProductCardGroup>
            <ScrollToTop />
        </main>
    </DefaultLayout>
</template>
    
<style scoped></style>