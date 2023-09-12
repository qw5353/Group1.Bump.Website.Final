<script setup>
import { ref, onMounted } from 'vue'
import OrderDetail01 from './01OrderDetail.vue'
import OrderDetail02 from './02OrderDetail.vue'
import OrderDetail03 from './03OrderDetail.vue'
import OrderDetail04 from './04OrderDetail.vue'
import OrderDetail05 from './05OrderDetail.vue'
import OrderDetail06 from './06OrderDetail.vue'
import axios from 'axios'
import { userStateStore } from '../../../stores/UserStateStore';

const tab = ref(1);
const titles = ref([
    { t: "全部" },
    { t: "待付款" },
    { t: "處理中" },
    { t: "已完成" }
])

const orderData = ref([]);
const userStates = userStateStore();

onMounted(async () => {
    try {
        const res = await axios.get(`/api/order?id=${userStates.userId}`);
        orderData.value = res.data.map(row => {
            const { createAt } = row;
            const createDate = createAt.split("T")[0];
            const createTime = createAt.split("T")[1].slice(0, 5);

            return {
                ...row,
                forrmatedCreateAt: createDate + "  " + createTime
            }
        });
    } catch (err) {
        console.error(err);
    }
})

</script>
<template>
    <v-tabs v-model="tab" color="orange-darken-4" grow align-tabs="center">
        <v-tab v-for="(title, index) in titles" :key="title" :value="index + 1"
            class="w-15 h-auto text-h6 font-weight-bold spacing">
            {{ title.t }}
        </v-tab>
    </v-tabs>
    <v-window v-model="tab">
        <v-window-item :key="1" :value="1">
            <v-container fluid>
                <OrderDetail01 :orderData="orderData"></OrderDetail01>
            </v-container>
        </v-window-item>
        <v-window-item :key="2" :value="2">
            <v-container fluid>
                <OrderDetail02 :orderData="orderData"></OrderDetail02>
            </v-container>
        </v-window-item>
        <v-window-item :key="3" :value="3">
            <v-container fluid>
                <OrderDetail03 :orderData="orderData"></OrderDetail03>
            </v-container>
        </v-window-item>
        <v-window-item :key="4" :value="4">
            <v-container fluid>
                <OrderDetail04 :orderData="orderData"></OrderDetail04>
            </v-container>
        </v-window-item>
    </v-window>
</template>
<style scoped></style>