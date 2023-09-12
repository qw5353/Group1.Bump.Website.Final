<script setup>
import { mdiFireCircle } from '@mdi/js';
import { ref } from 'vue';
import { VSkeletonLoader } from 'vuetify/lib/labs/components.mjs';
import 'element-plus/es/components/skeleton-item/style/css'
import 'element-plus/es/components/skeleton/style/css'
import { ElSkeletonItem, ElSkeleton } from 'element-plus';
import ProductCardDiscountsChip from './ProductCardDiscountsChip.vue';
import ProductCardDiscountsBadge from './ProductCardDiscountsBadge.vue';
import ProductCardNewBadge from './ProductCardNewBadge.vue';
import ProductCardSoldOutBadge from './ProductCardSoldOutBadge.vue';

defineProps(["id", "name", "brandName", "thumbnail", "price"])

const totalDiscount = ref(0)
const getDataFromComponent = (data) => {
    totalDiscount.value = data
};

</script>

<template>
    <v-card class="mx-auto my-12" width="262" style="box-shadow: none;" :href="`/products/${id}`">
        <div v-if="id && price" class="d-flex">
            <ProductCardDiscountsBadge :id="id" :price="price" class="me-auto"></ProductCardDiscountsBadge>
            <ProductCardNewBadge :id="id"></ProductCardNewBadge>
        </div>
        <div class="d-flex justify-center">
            <v-img :src="`/files/images/products/${thumbnail}`" alt="404" height="210px" width="210px" contains>
                <template #placeholder>
                    <el-skeleton style="width: 210px; margin: 0 auto 0 auto;" animated>
                        <template #template>
                            <el-skeleton-item variant="image" style="width: 210px; height: 210px" />
                        </template>
                    </el-skeleton>
                </template>
            </v-img>
            <ProductCardSoldOutBadge :id="id" />
        </div>
        <v-skeleton-loader v-if="name.length < 1 || price.length < 1" class="mx-auto" max-width="300" type="article">
        </v-skeleton-loader>
        <div v-else>
            <v-card-item>
                <v-card-title class="text-subtitle-1 title_style">{{ name }}</v-card-title>
                <v-card-subtitle>
                    <span class="me-1">{{ brandName }}</span>
                </v-card-subtitle>
            </v-card-item>
            <v-card-text>
                <div v-if="totalDiscount === 0" class="my-1 text-subtitle-1 price_style">
                    $ {{ price }}
                </div>
                <div v-else class="d-flex">
                    <div class="my-1 text-subtitle-1 price_style mr-3 text-decoration-line-through">
                        $ {{ price }}
                    </div>
                    <div style="color: #D41010;" class="my-1 text-subtitle-1 price_style">
                        $ {{ price - totalDiscount }}
                    </div>
                </div>
                <div>
                    <ProductCardDiscountsChip :id="id" @send-Discount="getDataFromComponent">
                    </ProductCardDiscountsChip>
                </div>
            </v-card-text>
        </div>
    </v-card>
</template>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Inria+Sans:wght@300&display=swap');
@import url('https://fonts.googleapis.com/css2?family=Inria+Sans:wght@300&display=swap');

.v-card-title {
    white-space: unset !important;
}

.title_style {
    font-family: 'Inria Sans', sans-serif !important;
    line-height: 25px;
}

.price_style {
    display: flex;
    font-family: 'Inria Sans', sans-serif !important;
    font-weight: 600;
    font-size: 17px !important;
    text-align: center;
    color: #656565;
}

.v-card:hover {
    border: 2px solid rgb(255, 178, 62);
    border-radius: 5%;
}
</style>

