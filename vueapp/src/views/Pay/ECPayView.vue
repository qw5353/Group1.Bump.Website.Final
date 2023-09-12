<script setup>
import { onMounted, ref } from 'vue';
import { useRoute } from 'vue-router';

const route = useRoute();

// 特店編號(MerchantID)：3002607
// 特店後台登入帳號：stagetest3
// 特店後台登入密碼：test1234
// 特店統一編號：00000000
// 串接金鑰HashKey：pwFHCqoQZGmho4w6
// 串接金鑰HashIV：EkRm7iFT261dpevs

const form = ref(null);

onMounted(() => {

    const obj = {...route.query};
    

    for (const key in obj) {
        const input = document.createElement('input');
        input.value = obj[key];
        input.name = key;
        form.value.append(input);
    }

    form.value.submit();
})

</script>

<template>
    <form ref="form" action="https://payment-stage.ecpay.com.tw/Cashier/AioCheckOut/V5" method="post">

    </form>
</template>

<style scoped>

</style>
