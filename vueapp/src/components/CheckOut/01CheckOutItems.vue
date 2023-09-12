<script setup>
import { useCheckOutStore } from '../../stores/CheckOutStore';
import { storeToRefs } from 'pinia';
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import _ from 'lodash';
import { mdiCheckCircle } from '@mdi/js';

const checkoutStore = useCheckOutStore();
const { items } = storeToRefs(checkoutStore);
const router = useRouter();

const showFreebies = ref([]);

onMounted(() => {
    if (items.value.length === 0) {
        router.push({ path: '/cart' });
    }
    showFreebies.value = items.value.map(item => item.freebieName !== '');
})


</script>

<template>
    <v-container fluid class="p-0">
        <v-table>
            <thead>
                <tr>
                    <th class="text-left">
                        商品
                    </th>
                    <th class="text-left">

                    </th>
                    <th class="text-left">
                        單價
                    </th>
                    <th class="text-left">
                        數量
                    </th>
                    <th class="text-left">
                        小計
                    </th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="item in items" :key="item.price">
                    <td><v-img :width="150" aspect-ratio="16/9" cover :src="`/files/images/products/${item.img}`"></v-img>
                    </td>
                    <td width="430">{{ item.name }}<br>
                        <small class="text-grey-darken-1">{{ item.style }}</small>
                        <br>
                        <br>
                        <v-chip v-for="activityDetailName in item.activityDetailNames" class="text-caption mb-1"
                            :prepend-icon=mdiCheckCircle color="green" style="white-space: normal;">
                            已符合&emsp;<p class="font-weight-bold">{{ activityDetailName }}</p>
                        </v-chip>
                    </td>
                    <td>{{ item.price }}</td>
                    <td>{{ item.quantity }}</td>
                    <td>{{ item.subTotal }}</td>
                </tr>
                <br>
            </tbody>
        </v-table>
        <v-row v-show="showFreebies[index]" v-for="(item, index) in items" :key="item.name">
            <v-col cols="1" class="d-flex justify-center align-center">
                <v-chip class="text-caption" color="green">贈品</v-chip>
            </v-col>
            <v-col cols="1" class="d-flex justify-center align-center">
                <v-img :width="150" aspect-ratio="16/9" cover :src="`${item.freebieImg}`"></v-img>
            </v-col>
            <v-col cols="2" class="d-flex align-center">
                {{ item.freebieName }}
            </v-col>
        </v-row>
        <!-- <br>
    <h2>訂單總計 : &emsp;${{ total }}</h2>
    <v-row no-gutters class="align-end d-flex">
      <v-col cols="1">
        <small v-if="items.length > 0">{{ levels[items[0].memberLevel] }}</small>
      </v-col>
      <v-col cols="9">
        <h2 v-if="items.length > 0" class="text-orange-darken-4">{{ levelred[items[0].memberLevel] }}</h2>
      </v-col>
      <v-col cols="2" class="d-flex">
        <v-btn @click="checkout" color="warning" class="w-15 h-auto text-h5 font-weight-bold ms-auto py-2">去結帳</v-btn>
      </v-col>

    </v-row> -->

    </v-container>
</template>