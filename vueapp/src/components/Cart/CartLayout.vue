<script setup>
import { ref, onMounted } from 'vue';
import CartList from './CartList.vue';
import CartEmpty from './CartEmpty.vue';
import ProductCardGroup from '../shop/product/ProductCardGroup.vue'
import axios from 'axios';
const itemsData = ref([]);


const sellsWellProductGroup = ref([
  {
    name: "", brandName: "", price: "", thumbnail: "", totalPage: ""
  },
])
onMounted(async () => {
  try {
    const res2 = await axios.get('/api/shop/SellsWellProductGroup');
    sellsWellProductGroup.value = res2.data;
  } catch (err) {
    console.error(err);
  }
})

</script>

<template>
  <v-container fluid class="p-0">
    <div class="content-container">
      <p class="ms-10 mt-6" style="font-size:2em;">購物車</p>
      <br>
      <hr>
      <div class="center">
        <CartList @cart-data="(items) => itemsData = items" :class="{ 'd-none': !itemsData }" />
        <CartEmpty :class="{ 'd-none': itemsData }" />
      </div>
      <hr>
      <div class="center">
        <ProductCardGroup title=" 你可能會喜歡 " :products="sellsWellProductGroup" />
      </div>
    </div>
  </v-container>
</template>
  
<style scoped>
.content-container {
  width: 1000px;
  margin: 0 auto;
}

.center {
  display: flex;
  justify-content: center;
  /* 水平居中 */
  align-items: center;
  /* 垂直居中（可选） */
}
</style>