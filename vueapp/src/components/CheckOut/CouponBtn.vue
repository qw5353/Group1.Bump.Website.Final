<script setup>
import { ref, watch } from 'vue'
import CouponSelect from './CouponSelect.vue';
import { useCheckOutStore } from '../../stores/CheckOutStore';
import { storeToRefs } from 'pinia';

const checkoutStore = useCheckOutStore();
const { selectedDiscount } = storeToRefs(checkoutStore);

const dialog = ref(false);


</script>

<template>
    <v-text-field class="w-50" readonly>{{
        selectedDiscount?.coupon?.couponName }}</v-text-field>
    <v-btn color="amber-lighten-3" class=" ms-5 mb-6">優惠券</v-btn>
    <v-dialog v-model="dialog" activator="parent" width="auto">
        <v-card>
            <v-card-title class="text-center text-h6 font-weight-black">可使用的優惠券</v-card-title>
            <v-divider></v-divider>
            <!-- <v-card-text class="text-red">※優惠券與折扣活動僅能擇一使用</v-card-text> -->
            <v-card-text style="height: 600px;">
                <CouponSelect></CouponSelect>
            </v-card-text>
            <v-divider></v-divider>
            <v-card-actions>
                <v-btn class="dialogConfirm font-weight-black" color="white" block @click="dialog = false">確定</v-btn>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>
<style scoped>
.dialogConfirm {
    background-color: #FFB23E;
}
</style>