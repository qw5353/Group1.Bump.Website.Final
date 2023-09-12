<script setup>
import { ref, onMounted } from 'vue';
import axios from 'axios';
import CouponGet from './CouponGet.vue';
import CouponTransfer from './CouponTransfer.vue';

const coupons = ref([]);
const expiringCoupons = ref([]);
const promotionProductTypeName = ["全館商品", "商品標籤"];
const showExpiringCoupons = ref(false);
const numInPage = 6;
const page = ref(1);

onMounted(async () => {
    try {
        await initialize();
    } catch (err) {
        console.error(err);
    }
})

async function initialize() {
    const res = await axios.get('/api/Coupons/MemberCoupons');
    coupons.value = res.data;
    coupons.value.map((item) => {
        const productTagName = item.productTagNames[0];
        if (item.promotionProductTypeName === promotionProductTypeName[0]) {
            item.goRouter = "/shop";
        }
        if (item.promotionProductTypeName === promotionProductTypeName[1]) {
            item.goRouter = "/shop/OnSale?actName=" + productTagName;
        }
    })

    expiringCoupons.value = expiringCoupon(coupons.value);
    expiringCoupons.value.map((item) => {
        if (item.promotionProductTypeName === promotionProductTypeName[0]) {
            item.goRouter = "/shop";
        }
        if (item.promotionProductTypeName === promotionProductTypeName[1]) {
            item.goRouter = "/shop/OnSale?actName=" + productTagName;
        }
    })
}
function expiringCoupon(coupons) {
    var now = new Date();
    var expiring = now.setDate(now.getDate() + 7);

    return coupons.filter(c => new Date(c.endTime) < expiring);
}

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
    <CouponGet :initialize="initialize"></CouponGet>

    <div class="all-width mx-auto">
        <v-row class="mt-3 mx-auto">
            <v-btn class="rounded-pill bg-white mr-3" @click="showExpiringCoupons = false" :active="!showExpiringCoupons">全部
                ({{
                    coupons.length
                }})</v-btn>
            <v-btn class="rounded-pill bg-white" @click="showExpiringCoupons = true" :active="showExpiringCoupons">即將到期
                ({{
                    expiringCoupons.length }})</v-btn>
        </v-row>
        <v-row v-if="!showExpiringCoupons">
            <v-col xs="12" sm="12" md="12" lg="12" xl="6"
                v-for="(coupon, index) in coupons.slice(numInPage * (page - 1), numInPage * page)" :key="coupon.couponId">
                <v-card class="mx-auto mb-3 mt-8 card" max-width="400" color="#FAEDCD">
                    <div class="d-flex flex-no-wrap justify-space-between h-100">
                        <v-card-item class="bg-left width-left">
                            <div class="text-body-2 mb-1">
                                {{ formatDateTime(coupon.startTime) }} - {{ formatDateTime(coupon.endTime) }}
                            </div>
                            <div class="text-h6 mb-1 font-weight-bold">
                                {{ coupon.couponName }}
                            </div>
                            <div class="d-flex mb-1">
                                <a :href="coupon.goRouter">
                                    <v-btn v-if="coupon.promotionProductTypeName !== ''"
                                        class="text-caption btn-product rounded-pill bg-white mr-2" variant="outlined"
                                        @click="print(index)">適用商品</v-btn>
                                    <v-btn disabled v-else class="text-caption btn-product rounded-pill bg-white mr-2"
                                        variant="outlined" @click="print(index)">適用商品</v-btn>
                                </a>
                                <CouponTransfer :coupon="coupon" :initialize="initialize"></CouponTransfer>
                            </div>
                        </v-card-item>
                        <v-card-item class="bg-right width-right">
                            <div class="text-h4 text-center font-weight-bold text-brown-darken-1"
                                v-if="coupon.couponTypeName === '打折'">{{ coupon.amount * 100 }}折</div>
                            <div class="text-h4 text-center font-weight-bold text-brown-darken-1"
                                v-if="coupon.couponTypeName === '贈品'">贈品</div>
                            <div class="text-h4 text-center font-weight-bold text-brown-darken-1"
                                v-if="coupon.couponTypeName === '折抵'">${{ coupon.amount
                                }}
                            </div>
                            <div class="text-caption text-brown-darken-1"
                                v-if="coupon.promotionProductTypeName === promotionProductTypeName[0]">全館商品<span
                                    v-if="coupon.priceThreshold !== 0">滿{{ coupon.priceThreshold }}</span>
                                <span v-else>不限金額</span>
                                使用
                            </div>
                            <div class="text-caption text-brown-darken-1"
                                v-if="coupon.promotionProductTypeName === promotionProductTypeName[1]">指定商品<span
                                    v-if="coupon.priceThreshold !== 0">滿{{ coupon.priceThreshold }}</span>
                                <span v-else>不限金額</span>使用
                            </div>
                        </v-card-item>
                    </div>
                </v-card>
            </v-col>
        </v-row>
        <v-row v-if="showExpiringCoupons">
            <v-col xs="12" sm="12" md="12" lg="12" xl="6"
                v-for="(coupon, index) in expiringCoupons.slice(numInPage * (page - 1), numInPage * page)"
                :key="coupon.couponId">
                <v-card class="mx-auto mb-3 mt-8 card" max-width="400" color="#FAEDCD">
                    <div class="d-flex flex-no-wrap justify-space-between h-100">
                        <v-card-item class="bg-left width-left">
                            <div class="text-body-2 mb-1">
                                {{ formatDateTime(coupon.startTime) }} - {{ formatDateTime(coupon.endTime) }}
                            </div>
                            <div class="text-h6 mb-1 font-weight-bold">
                                {{ coupon.couponName }}
                            </div>
                            <div class="d-flex mb-1">
                                <router-link :to="coupon.goRouter">
                                    <v-btn v-if="coupon.promotionProductTypeName !== ''"
                                        class="text-caption btn-product rounded-pill bg-white mr-2" variant="outlined"
                                        @click="print(index)">適用商品</v-btn>
                                    <v-btn disabled v-else class="text-caption btn-product rounded-pill bg-white mr-2"
                                        variant="outlined" @click="print(index)">適用商品</v-btn>
                                </router-link>
                                <CouponTransfer :coupon="coupon" :initialize="initialize"></CouponTransfer>
                            </div>
                        </v-card-item>
                        <v-card-item class="bg-right width-right">
                            <div class="text-h4 text-center font-weight-bold text-brown-darken-1"
                                v-if="coupon.couponTypeName === '打折'">{{ coupon.amount * 100 }}折</div>
                            <div class="text-h4 text-center font-weight-bold text-brown-darken-1"
                                v-if="coupon.couponTypeName === '贈品'">贈品</div>
                            <div class="text-h4 text-center font-weight-bold text-brown-darken-1"
                                v-if="coupon.couponTypeName === '折抵'">${{ coupon.amount
                                }}
                            </div>
                            <div class="text-caption text-brown-darken-1"
                                v-if="coupon.promotionProductTypeName === promotionProductTypeName[0]">全館商品<span
                                    v-if="coupon.priceThreshold !== 0">滿{{ coupon.priceThreshold }}</span>
                                <span v-else>不限金額</span>
                                使用
                            </div>
                            <div class="text-caption text-brown-darken-1"
                                v-if="coupon.promotionProductTypeName === promotionProductTypeName[1]">指定商品<span
                                    v-if="coupon.priceThreshold !== 0">滿{{ coupon.priceThreshold }}</span>
                                <span v-else>不限金額</span>使用
                            </div>
                        </v-card-item>
                    </div>
                </v-card>
            </v-col>
        </v-row>
        <v-pagination class="my-5" v-if="!showExpiringCoupons" :length="Math.ceil(coupons.length / numInPage)"
            v-model="page"></v-pagination>
        <v-pagination class="my-5" v-if="showExpiringCoupons" :length="Math.ceil(expiringCoupons.length / numInPage)"
            v-model="page"></v-pagination>
    </div>
</template>
<style scoped>
.card {
    height: 150px;
}

.btn-product {
    width: 10%
}

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

.all-width {
    width: 100%
}
</style>