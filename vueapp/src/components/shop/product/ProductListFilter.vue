<script setup>
import { ref, watchEffect, watch } from 'vue';
import { useRoute } from 'vue-router';
import axios from 'axios';

const route = useRoute();
const filterEmits = ref({});
const emit = defineEmits(['filterSelected']);

const params = ref({ keyword: null, FirstCategory: null, secondCategories: null, thridCategories: null });
const isFilterOpen = ref(false);
const brandSelected = ref([]);
const categorySelected = ref([]);
const priceRange = ref([0, 0]);
const filterCategoryTitle = ref('');
const filterCategories = ref([]);
const filterBrands = ref([]);
const filterPriceRange = ref({});


watchEffect(async () => {
    filterCategoryTitle.value = null;
    brandSelected.value = [];
    categorySelected.value = [];
    if (route.query["navbarFilter"] && !route.query["mid"]) {
        filterCategoryTitle.value = '商品中分類'
        try {
            const getCategories = await axios.get(`/api/Shop/ProductSecondCategories?firstCategory=${route.query["navbarFilter"]}`);
            filterCategories.value = getCategories.data;
        } catch (err) {
            console.error(err);
        }
    }
    else if (route.query["mid"] && !route.query["sub"]) {
        filterCategoryTitle.value = '商品小分類'
        try {
            const getCategories = await axios.get(`/api/Shop/ProductThirdCategories?firstCategory=${route.query["navbarFilter"]}&secoondCategory=${route.query["mid"]}`);
            filterCategories.value = getCategories.data;
        } catch (err) {
            console.error(err);
        }
    }
    try {
        params.value.keyword = route.query["keyword"];
        params.value.FirstCategory = route.query["navbarFilter"];
        params.value.secondCategories = route.query["mid"] ? [route.query["mid"]] : null;
        params.value.thirdCategories = route.query["sub"] ? [route.query["sub"]] : null;
        const getBrands = await axios.post(`/api/Shop/ProductListBrands`, params.value);
        filterBrands.value = getBrands.data;

        const getPriceRange = await axios.post(`/api/Shop/ProductListPriceRange`, params.value);
        filterPriceRange.value = getPriceRange.data;
        priceRange.value[0] = filterPriceRange.value.minPrice;
        priceRange.value[1] = filterPriceRange.value.maxPrice;
    } catch (err) {
        console.error(err);
    }
});
watch(brandSelected, () => {
    filterEmits.value.brandSelected = brandSelected.value;
    emit('filterSelected', filterEmits.value);
});
watch(categorySelected, () => {
    filterEmits.value.thirdCategorySelected = [];
    filterEmits.value.secondCategorySelected = [];
    if (filterCategoryTitle.value == '商品中分類') {
        filterEmits.value.secondCategorySelected = categorySelected.value;
    }
    else if (filterCategoryTitle.value == '商品小分類') {
        filterEmits.value.thirdCategorySelected = categorySelected.value;
    }
    emit('filterSelected', filterEmits.value);
});

const clickFilterPriceRange = () => {
    filterEmits.value.minPrice = priceRange.value[0];
    filterEmits.value.maxPrice = priceRange.value[1];
    emit('filterSelected', filterEmits.value);
};

const clickFilterOpen = async () => {
    isFilterOpen.value = !isFilterOpen.value;
};

function removeBrand(brand) {
    brandSelected.value = brandSelected.value.filter(item => item !== brand);
};
function removeCategory(category) {
    categorySelected.value = categorySelected.value.filter(item => item !== category);
};
</script>
<template>
    <v-container>
        <v-row class="px-4 pt-4">
            <a href="javascript:void(0)" class="px-5 sidebarFilter" @click="clickFilterOpen"><i
                    class="fa fa-align-left mr-5"></i>
                Filter</a>
            <div class="px-4 pt-4" style="width:100%">
                <v-divider></v-divider>
            </div>
        </v-row>
        <v-row v-if="isFilterOpen" class="px-8 d-flex align-center">
            <v-col cols="12">
                <span v-if="brandSelected.length > 0">品牌:</span>
                <v-chip v-for="brand in brandSelected" :key="brand" class="mx-1" @click:close="removeBrand(brand)" closable
                    color="blue">
                    {{ brand }}
                </v-chip>
                <span v-if="brandSelected.length > 0 && categorySelected.length > 0"> 　</span>
                <span v-if="categorySelected.length > 0">分類:</span>
                <v-chip v-for="category in categorySelected" :key="category" class="mx-1"
                    @click:close="removeCategory(category)" closable color="green">
                    {{ category }}
                </v-chip>
            </v-col>
            <v-col cols="12" xl="4" lg="4" md="6" sm="6" class="h170px rounded-lg">
                <div class="d-flex justify-center align-center mb-auto rounded-t-lg"
                    style="width: 100%; height: 40px; background-color: rgb(221, 221, 221);">
                    品牌</div>
                <div class="px-1 overflow-auto border" style="height: 107px;"><v-checkbox v-for="brand in filterBrands"
                        v-model="brandSelected" :label="brand" :value="brand" class="mb-n10 mt-n3"></v-checkbox></div>
            </v-col>
            <v-col cols="12" xl="4" lg="4" md="6" sm="6" v-if="filterCategoryTitle" class="h170px rounded-lg">
                <div class="d-flex justify-center align-center mb-auto rounded-t-lg"
                    style="width: 100%; height: 40px; background-color: rgb(221, 221, 221);">
                    {{ filterCategoryTitle }}</div>
                <div class="px-1 overflow-auto border" style="height: 107px;"><v-checkbox
                        v-for="category in filterCategories" v-model="categorySelected" :label="category" :value="category"
                        class="mb-n10 mt-n3"></v-checkbox>
                </div>
            </v-col>
            <v-col cols="12" xl="4" lg="4" md="6" sm="6" class="h170px rounded-lg d-flex flex-column">
                <div class="d-flex justify-center align-center mb-auto rounded-t-lg"
                    style="width: 100%; height: 40px; background-color: rgb(221, 221, 221);">
                    金額範圍</div>
                <div class="px-5 border h-75 d-flex justify-center flex-column">
                    <div class="w-100 pt-10 h-75"><v-range-slider v-model="priceRange" step="1"
                            :min="filterPriceRange.minPrice" :max="filterPriceRange.maxPrice"
                            thumb-label="always"></v-range-slider></div>
                    <div class="mb-3 d-flex justify-center h-25"><v-btn variant="outlined" size="small"
                            @click="clickFilterPriceRange">
                            價格篩選
                        </v-btn></div>
                </div>
            </v-col>

            <div class="pt-4" style="width:100%">
                <v-divider></v-divider>
            </div>
        </v-row>
    </v-container>
</template>
    
<style scoped>
.sidebarFilter {
    list-style: none;
    color: #706f6f;
    text-decoration: none;
}

.sidebarFilter:hover {
    color: #E6A947 !important;
}

.h170px {
    height: 170px;
}
</style>