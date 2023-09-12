<script setup>
import { defineProps } from 'vue';
const props = defineProps(['coupon'])
const coupon = props.coupon;

const promotionProductTypeName = "全館商品";

function formatDateTime(dateTimeStr) {
    const date = new Date(dateTimeStr);

    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');

    return `${month}/${day} ${hours}:${minutes}`;
}
</script>
<template>
    <v-card class="mx-auto h-100" max-width="344" color="#FAEDCD">
        <div class="d-flex flex-no-wrap justify-space-between h-100">
            <v-card-item class="bg-left width-left">
                <div class="text-body-2 mb-1">
                    {{ formatDateTime(coupon.startTime) }} - {{ formatDateTime(coupon.endTime) }}
                </div>
                <div class="text-h6 mb-1 font-weight-bold">
                    {{ coupon.couponName }}
                </div>
            </v-card-item>
            <v-card-item class="bg-right width-right">
                <div class="text-h4 text-center font-weight-bold text-brown-darken-1" v-if="coupon.couponTypeName === '打折'">
                    {{ coupon.amount * 100 }}折</div>
                <div class="text-h4 text-center font-weight-bold text-brown-darken-1" v-if="coupon.couponTypeName === '贈品'">
                    贈品</div>
                <div class="text-h4 text-center font-weight-bold text-brown-darken-1" v-if="coupon.couponTypeName === '折抵'">
                    ${{ coupon.amount
                    }}
                </div>
                <div class="text-caption text-brown-darken-1"
                    v-if="coupon.promotionProductTypeName === promotionProductTypeName">全館商品<span
                        v-if="coupon.priceThreshold !== 0">滿{{ coupon.priceThreshold }}</span>
                    <span v-else>不限金額</span>使用
                </div>
                <div class="text-caption text-brown-darken-1" v-else>指定商品<span v-if="coupon.priceThreshold !== 0">滿{{
                    coupon.priceThreshold }}</span>
                    <span v-else>不限金額</span>使用
                </div>
            </v-card-item>
        </div>
    </v-card>
</template>
<style scoped>
.bg-left {
    background-color: #FAEDCD;
    border-right: 2px dashed white;
}

.bg-right {
    background-color: #CCD5AE;
    border-left: 2px dashed white;
}

.width-left {
    width: 65%;
}

.width-right {
    width: 35%;
}
</style>