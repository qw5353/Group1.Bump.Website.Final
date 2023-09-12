import { ref, computed } from 'vue'
import { defineStore } from 'pinia'
import axios from 'axios';

export const useCheckOutStore = defineStore('checkout', () => {
    const items = ref([]);
    const selectedDiscount = ref({});
    const selectedCouponId = ref(0);
    const memberCoupons = ref([]);

    return { items, selectedDiscount, selectedCouponId, memberCoupons }
})
