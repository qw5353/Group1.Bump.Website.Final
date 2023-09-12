<script setup>
import { ref, onMounted, watch } from 'vue';
import axios from 'axios';
import { useCheckOutStore } from '../../stores/CheckOutStore';
import { storeToRefs } from 'pinia';
import { mdiAlertCircleOutline } from '@mdi/js';
import CouponTemplate from '../Coupon/CouponTemplate.vue'

const checkoutStore = useCheckOutStore();
const { items, selectedDiscount, selectedCouponId } = storeToRefs(checkoutStore);
const coupons = ref([]);
const discountPrices = ref([]);
const defaultCoupon = {
    id: 0,
    memberName: '',
    account: '',
    startTime: '',
    endTime: '',
    sendingTime: '',
    usage: false,
    couponId: 0,
    couponName: '',
    code: '',
    eventTypeName: '',
    couponStartTime: '',
    couponEndTime: '',
    targetTypeName: '',
    promotionProductTypeName: '',
    status: '',
    priceThreshold: 0,
    couponTypeName: '',
    discountQty: null,
    amount: 0,
    freebieName: null,
    quantity: null,
    extraSalesUsage: false,
    memberId: 0,
    productTagNames: []
};

watch(selectedCouponId, function (newId) {
    const findCoupon = coupons.value.find(c => c.couponId === newId);
    if (findCoupon) {
        const index = coupons.value.findIndex(c => c.couponId === newId);
        selectedDiscount.value = {
            coupon: findCoupon,
            discountPrice: discountPrices.value[index]
        };
    }

    else {
        selectedDiscount.value = {};
        selectedCouponId.value = 0;
    }
});

onMounted(async () => {
    try {
        localStorage.setItem('myCart', JSON.stringify(items.value))
        const carts = JSON.parse(localStorage.getItem('myCart')).map(cart => {
            return {
                ...cart,
            };
        });
        const res = await axios.post('/api/Coupons/CartCoupons', carts);
        coupons.value = res.data.coupons;
        console.log(coupons.value)
        discountPrices.value = res.data.discountPrice;
        console.log(selectedCouponId.value)
    } catch (err) {
        console.error(err);
    }
})

</script>

<template>
    <v-row class="mt-10" justify="center" v-if="coupons == null">
        <div>
            <v-icon :icon="mdiAlertCircleOutline" color="grey" class="noCouponImage"></v-icon>
        </div>
    </v-row>
    <v-row class="mt-10" justify="center" v-if="coupons == null">
        <div class="text-h6 noCoupon">
            這裡什麼優惠券都沒有喔
        </div>
    </v-row>
    <v-row class="align-center mb-3 card-height" v-for="(coupon, index) in coupons" :key="coupon.couponId">
        <v-checkbox :value="coupon.couponId" v-model="selectedCouponId"></v-checkbox>
        <CouponTemplate :coupon="coupon"></CouponTemplate>
    </v-row>
</template>

<style scoped>
.card-height {
    height: 130px;
}

.noCoupon {
    color: grey;
}

.noCouponImage {
    height: 100px;
    width: 100px;
}
</style>