<script setup>
import { onMounted } from 'vue';
import axios from 'axios';

onMounted(async () => {
    try {
        const res = await axios.post('/api/pay/line/request', {
            amount: 1200,
            confirmURL: "https://localhost:5002/pay/line/confirm",
            productName: "BUMP 戶外用品"
        });
        
        location.href = res.data;
    } catch (error) {
        console.log(error);
    }
});

</script>

<template>
    <form @submit.prevent ref="form" action="/api/pay/line/request" method="POST"></form>
</template>

<style scoped></style>