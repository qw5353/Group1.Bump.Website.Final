<script setup>
import { ref } from 'vue';
import ProductCardDiscountsBadge from './ProductCardDiscountsBadge.vue';
import { mdiPanRight } from '@mdi/js';

const activityDetailName = ref("");
const productTagName = ref("");

const link = ref("");

const getDataFromComponent = (data1, data2) => {
    activityDetailName.value = data1;
    productTagName.value = data2;
    link.value = productTagName.value == null ? 'https://localhost:5002/Shop' : 'https://localhost:5002/Shop/OnSale?actName=' + productTagName.value;
};
const props = defineProps(['id', 'price']);
const id = props.id;
const price = props.price;

</script>
<template>
    <div>
        <a :href="link">
            <div style="width:100%" class="border d-flex">
                <ProductCardDiscountsBadge @send-Name="getDataFromComponent" :id="id" :price="price">
                </ProductCardDiscountsBadge>
                <p class="mx-auto">{{ activityDetailName }}</p>
                <v-icon :icon="mdiPanRight" class="mr-3"></v-icon>
            </div>
        </a>

    </div>
</template>

<style scoped>
a {
    text-decoration: none;
}

a:link {
    color: black;
}

a:visited {
    color: black;
}

a:hover {
    color: rgb(165, 165, 165);
}
</style>

