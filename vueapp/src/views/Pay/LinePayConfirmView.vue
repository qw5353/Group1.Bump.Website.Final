<script setup>
import { onMounted } from 'vue';
import axios from 'axios';
import { useRoute, useRouter } from 'vue-router';

const route = useRoute();
const router = useRouter();

onMounted(async () => {
    const { transactionId, orderId } = route.query;

    const amountRes = await axios.get('/api/pay/line/order/' + orderId);
    const amount = amountRes.data;

    if (transactionId && orderId) {
        try {
            await axios.post('/api/pay/line/confirm', {
                transactionId,
                amount,
                lineOrderId: orderId
            });

            // 結帳完成跳轉
            router.push('/Member/Orders');

        } catch (err) {
            console.log(err);
        }

    }
});

</script>

<template>
    <div></div>
</template>

<style scoped></style>