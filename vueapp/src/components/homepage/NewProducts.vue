<script setup>
import { ref, onMounted } from 'vue';
import ProductCard from '../shop/product/ProductCard.vue';
import axios from 'axios';
const newProductGroup = ref([
    {
        name: "", brandName: "", price: "", thumbnail: "", totalPage: ""
    },
])
onMounted(async () => {
    try {
        const getNewProductGroup = await axios.get('/api/shop/NewProductGroup');
        newProductGroup.value = getNewProductGroup.data;
    } catch (err) {
        console.error(err);
    }
})
</script>
<template>
    <div>
        <div class="d-flex flex-column align-center mt-16">
            <v-row>
                <h2 style="align-self: center;">
                    最新商品
                </h2>
            </v-row>
            <div class="title_style mt-5">
            </div>
        </div>
        <v-container class="d-flex justify-center">
            <v-row class="px-13" style="max-width:1300px" v-if="newProductGroup" no-gutters>
                <v-col cols="12" lg="3" md="4" sm="6" xs="12" v-for="product in newProductGroup" :key="product.id"
                    style="margin-top: -7px;">
                    <ProductCard :id="product.id" :name="product.name" :brandName="product.brandName" :price="product.price"
                        :thumbnail="product.thumbnail">
                    </ProductCard>
                </v-col>
            </v-row>
        </v-container>
    </div>
</template>
    
    
<style scoped>
.title_style {
    margin-top: 0.75rem;
    border-top: 3px solid rgb(255, 178, 62);
    width: 24px;
    border-radius: 12px;
}
</style>