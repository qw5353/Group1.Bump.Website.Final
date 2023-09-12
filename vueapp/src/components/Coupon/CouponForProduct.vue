<script setup>
import { ref } from 'vue';
import CouponTemplate from './CouponTemplate.vue'
const dialog = ref(false);
const props = defineProps(['couponDiscounts']);
const couponDiscounts = props.couponDiscounts.coupons;

</script>
<template>
    <v-btn size="small" color="amber-lighten-3" class=" ms-5">適用優惠券</v-btn>
    <v-dialog v-model="dialog" activator="parent" width="auto">
        <v-card>
            <v-card-title class="text-center text-h6 font-weight-black">可使用的優惠券</v-card-title>
            <v-divider></v-divider>
            <v-card-text style="height: 600px;">
                <div v-for="coupon in couponDiscounts" :key="coupon.couponId" class="my-4">
                    <CouponTemplate :coupon="coupon"></CouponTemplate>
                </div>
            </v-card-text>
        </v-card>
    </v-dialog>
</template>
<style scoped></style>