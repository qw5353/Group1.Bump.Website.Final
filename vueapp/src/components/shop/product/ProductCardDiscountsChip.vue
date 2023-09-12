<script setup>
import { ref, onMounted } from 'vue';
import axios from 'axios';

const props = defineProps(['id'])

const activityDiscounts = ref([]);
const couponDiscounts = ref([]);
const productId = props.id;
const maxDiscountPrice = ref(0);
const discountTypeNames = ref([]);

const emit = defineEmits(['sendDiscount']);


onMounted(async () => {
    try {
        const input = {
            productId: productId,
        }
        const res = await axios.post('/api/ActivityDiscounts/GetProductDiscount', input);
        activityDiscounts.value = res.data;

        discountTypeNames.value = activityDiscounts.value.map(item => item.activityDiscount.discountTypeName);

        maxDiscountPrice.value = Math.max(...activityDiscounts.value.map(item => item.totalDiscountPrice));

        emit('sendDiscount', maxDiscountPrice.value)

        const resCoupon = await axios.post('/api/Coupons/ProductCoupons', input);
        couponDiscounts.value = resCoupon.data;

        if (couponDiscounts.value.coupons === null) return;
        if (couponDiscounts.value.coupons.length !== 0) {
            discountTypeNames.value.unshift('優惠券');
        }
    }
    catch (err) {
        console.error(err);
    }
})

</script>
<template>
    <div>
        <v-chip v-for="name in discountTypeNames" size="x-small" class="mr-1 mt-3">
            <p class="font-weight-bold">{{ name }}</p>
        </v-chip>
    </div>
</template>
<style scoped></style>