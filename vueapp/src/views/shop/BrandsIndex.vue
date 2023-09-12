<script setup>
import { useTitle } from '@vueuse/core';
import DefaultLayout from '../../components/layouts/DefaultLayout.vue';
import ShopNavbar from '../../components/shop/layouts/ShopNavbar.vue';
import ScrollToTop from '../../components/shop/layouts/ScrollToTop.vue';
import axios from 'axios';
import { ref, onMounted } from 'vue';
import { ElSelect, ElOption } from 'element-plus';
import 'element-plus/es/components/skeleton-item/style/css'
import 'element-plus/es/components/skeleton/style/css'

useTitle('Bump - 品牌介紹');

const brands = ref([]);
const selectedBrand = ref('MSR');
const selectedBrandDesc = ref('');

onMounted(async () => {
    try {
        const getBrands = await axios.get('/api/shop/Brands');
        brands.value = getBrands.data.map(n => { return n.brandName });
        const getBrandDesc = await axios.get(`/api/shop/BrandDesc?name=MSR`);
        selectedBrandDesc.value = getBrandDesc.data;
    } catch (err) {
        console.error(err);
    }
});
async function handleBrandChange(Brand) {
    try {
        const getBrandDesc = await axios.get(`/api/shop/BrandDesc?name=${Brand}`);
        selectedBrandDesc.value = getBrandDesc.data;
    } catch (err) {
        console.error(err);
    }
};

</script>

<template>
    <DefaultLayout>
        <ShopNavbar></ShopNavbar>
        <main>
            <v-container>
                <v-row class="my-2 mx-16 px-16">
                    <v-col><span class="text-h4">{{ selectedBrand }}</span></v-col>
                    <v-col class="d-flex justify-end">
                        <el-select v-model="selectedBrand" @change="handleBrandChange(selectedBrand)" class="m-2"
                            placeholder="請選擇品牌">
                            <el-option v-for="brand in brands" :key="brand" :label="brand" :value="brand" />
                        </el-select>
                    </v-col>
                </v-row>
                <v-row class="w-100 h-100 py-5 mb-10 d-flex justify-center" v-if="selectedBrandDesc">
                    <div class="ma-0 px-15 h-100 w-75" v-html="selectedBrandDesc"
                        style="font-Size: 1em; letter-spacing:0.6px; line-height: 30px">
                    </div>
                </v-row>
                <v-row v-else class="d-flex justify-center ma-10" :style="{ height: 'calc(100vh - 533px)' }">
                    <h2>尚無品牌資訊 🙇</h2>
                </v-row>
            </v-container>
            <ScrollToTop />
        </main>
    </DefaultLayout>
</template>
<style scoped></style>