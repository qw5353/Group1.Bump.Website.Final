<script setup>
import { defineProps,computed } from 'vue';
import axios from 'axios';
import { ElMessage } from 'element-plus'
import { useCartStore } from '../../stores/CartStore';
import _, { assign } from 'lodash';

const cartStore = useCartStore();
const props = defineProps(['cartItems']);
async function deleteItem(id) {
    try {
        await axios.delete(`/api/cart/items/${id}`)
    } catch (err) {
        ElMessage({
            showClose: true,
            message: '喔不，購物車更改失敗!',
            type: 'error',
        })
        return;
    }
    try {
        await cartStore.updateCartItems();
        props.cartItems = cartStore.items;
    } catch (err) {
        console.error(err);
    }
    props.cartItems = props.cartItems.filter(item => item.cartDetailId !== id);
}

const total = computed(() => {
    return _.sumBy(props.cartItems, function (o) {
        return o.price * o.quantity;
    })
})

</script>

<template>
    <v-card class="mx-auto pa-2 rounded-l" max-width="800" max-height="600" color="grey-lighten-3">
        <div>
            <v-sheet v-for="item in props.cartItems" class="pa-6 mt-1" max-height="250" rounded width="500">
                <div>
                    <v-row no-gutters>
                        <v-col cols="4">
                            <a :href="`/products/${item.productId}`" target="_blank"><v-img :width="100"
                                cover :src="`/files/images/products/${item.img}`"
                                :href="`/products/${item.productId}`"></v-img></a>
                        </v-col>
                        <v-col cols="7" class="">
                            <a :href="`/products/${item.productId}`" target="_blank" class="text-decoration-none text-black">
                            {{ item.name }}</a><br>
                            <br>
                            <small class="font-weight-bold">價格: {{ item.price }}</small><br>
                            <small class="font-weight-bold">規格: {{ item.style }}</small><br>
                            <small class="font-weight-bold">數量: {{ item.quantity }}</small>
                            <!-- <small class="font-weight-bold">Id: {{ item.cartDetailId }}</small>-->
                        </v-col>
                        <v-col cols="1" class="d-flex  justify-end  align-end">
                                <v-btn class="" variant="text" color="orange-darken-3" @click="deleteItem(item.cartDetailId)">刪除</v-btn>
                        </v-col>

                    </v-row>

                    <!-- <v-btn variant="text" color="orange">Go to Login</v-btn> -->
                </div>
            </v-sheet>
        </div>
        <div>
            <div class="subTotal">
                <v-row>
                    <v-col cols="4 " class="ms-10 font-weight-bold">
                        <p>購物車小計 :</p>
                    </v-col>
                    <v-col cols="2"></v-col>
                    <v-col cols="3" class="font-weight-bold">
                        <p>$ {{ total }}</p>
                    </v-col>
                    <v-col cols="2" class="font-weight-bold">
                        <p>NTD</p>
                    </v-col>
                </v-row>
            </div>
        </div>
    </v-card>
</template>

<style scoped>
.subTotal{
    height: 80px;
    display: flex;
    align-items: center;
}
p{
    letter-spacing: 0.2em !important;
}
</style>