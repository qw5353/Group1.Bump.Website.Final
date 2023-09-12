<script setup>
import { ref, defineProps, computed } from 'vue';
import { useRouter } from 'vue-router';
import "@/assets/scss/bootstrap.scss";
import "bootstrap/dist/js/bootstrap.js"
import DailogOrderDetail from './DailogOrderDetail.vue';
import axios from 'axios';

const orderDetailData = ref()
const inquire = ref({
    orderId: 0,
    dialog: false
});

const props = defineProps(['orderData']);
const router = useRouter();

const filteredOrderData = computed(() => {
    return props.orderData.filter(item => item.status == '未付款');
})

async function checkout(orderId) {
    try {
        const res = await axios.post("/api/cart/re-checkout", { orderId });
        switch (res.data.payType) {
            case "Line Pay":
                location.href = res.data.redirectUrl;
                break;
            case "ECPay":
                router.push({ path: "/pay/ecpay/submit", query: res.data.form });
                break;
            default:
                throw new Error("沒此支付類型!");
        }
    } catch (error) {
        console.error(error);
    }
}

async function Inquire(id) {
    inquire.value.dialog = true;
    inquire.value.orderId = id;
    try {
        const res = await axios.get(`/api/orderDetail?id=${id}`);
        const { createAt } = res.data;
        const createDate = createAt.split("T")[0];
        const createTime = createAt.split("T")[1].slice(0, 5);

        orderDetailData.value = {
            ...res.data,
            formattedCreateAt: createDate + "  " + createTime
        };
    } catch (err) {
        console.error(err);
    }
}

console.log(filteredOrderData.value);

</script>
<template>
    <v-container fluid class="mx-n4">
        <table class="table custom-table-striped">
            <thead>
                <tr>
                    <th class="text-center">
                        訂單編號
                    </th>
                    <th class="text-center">
                        商品列表
                    </th>
                    <th class="text-center">
                        商品數量
                    </th>
                    <th class="text-center">
                        商品小計
                    </th>
                    <th class="text-center">
                        總價格
                    </th>
                    <th class="text-center">
                        訂單成立時間
                    </th>
                    <th class="text-center">
                        訂單狀態
                    </th>
                    <th class="text-center">
                        訂單細節
                    </th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="item in filteredOrderData" :key="item.orderId" class="">
                    <td align="center" valign="middle">{{ item.orderId }}</td>
                    <td valign="middle" class="padding1">
                        <p v-for="detail in item.details" :key="detail.unitPrice">{{ detail.productName }}</p>
                    </td>
                    <td align="center" valign="middle" class="padding1">
                        <p v-for="detail in item.details" :key="detail.unitPrice">{{ detail.quantity }}</p>
                    </td>
                    <td align="center" valign="middle" class="padding1">
                        <p v-for="detail in item.details" :key="detail.unitPrice">{{ detail.subtotal }}</p>
                    </td>
                    <td align="center" valign="middle">{{ item.totalProductsPrice }}</td>
                    <td align="center" valign="middle" style="white-space: pre;">{{ item.forrmatedCreateAt }}</td>
                    <td align="center" valign="middle">{{ item.orderStatusName }}</td>
                    <td align="center" valign="middle">
                        <v-btn variant="tonal" color="orange-darken-3" @click="Inquire(item.orderId)">查詢</v-btn>
                        <br>
                        <v-btn class="margin1" variant="tonal" color="red-accent-4" @click="checkout(item.orderId)">補結帳</v-btn>
                    </td>
                </tr>
                <br>
            </tbody>
            <DailogOrderDetail :inquire="inquire" :orderDetailData="orderDetailData" v-if="orderDetailData"
                @close="() => inquire.dialog = false">
            </DailogOrderDetail>
        </table>
    </v-container>
</template>
<style scoped>
.padding1 {
    padding-top: 20px !important;
}

.margin1 {
    margin-top: 10px !important;
}

th {
    background-color: #dc9511 !important;
    color: rgb(255, 255, 255) !important;
}

.custom-table-striped tbody tr:nth-child(odd) td {
    background-color: #fef9f3;
}

p {
    margin-bottom: 1rem;
}
</style>