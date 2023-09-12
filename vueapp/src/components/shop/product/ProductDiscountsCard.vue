<script setup>
import { ref, onMounted } from 'vue';
import { useRoute } from 'vue-router';
import axios from 'axios';
import { userStateStore } from '../../../stores/UserStateStore';
import { mdiChevronRightCircle } from '@mdi/js';
import { mdiClose } from '@mdi/js';
import CouponForProduct from '../../Coupon/CouponForProduct.vue'

const route = useRoute();
const useStore = userStateStore();
const memberId = useStore.userId;
const activityDiscounts = ref([]);
const activityDiscountsDisplays = ref([]);
const couponDiscounts = ref([]);
const productId = route.params.id;
const dialog = ref(false);
const maxDiscountPrice = ref(0);

const activityDetail = ref({});
const selectedItemIndex = ref(0);

const emit = defineEmits(['sendDiscount']);

let scrollInvoked = 0
function onScroll() {
    scrollInvoked++
}

onMounted(async () => {
    try {
        const input = {
            productId: productId,
        }
        const res = await axios.post('/api/ActivityDiscounts/GetProductDiscount', input);
        activityDiscounts.value = res.data;

        const resCoupon = await axios.post('/api/Coupons/ProductCoupons', input);
        couponDiscounts.value = resCoupon.data;

        displayActivityDiscounts(activityDiscounts, couponDiscounts);

        changeActivityDetail(selectedItemIndex.value);

        maxDiscountPrice.value = Math.max(...activityDiscounts.value.map(item => item.totalDiscountPrice));
        emit('sendDiscount', maxDiscountPrice.value)
    }
    catch (err) {
        console.error(err);
    }
})

function displayActivityDiscounts(activityDiscounts, couponDiscounts) {

    activityDiscounts.value.map(item => {
        const display = {
            activityDetailName: item.activityDiscount.activityDetailName,
            discountTypeName: item.activityDiscount.discountTypeName
        }
        activityDiscountsDisplays.value.push(display);
    });
    if (couponDiscounts.value.coupons === null) return;
    if (couponDiscounts.value.coupons.length !== 0) {
        const couponDisplay = {
            activityDetailName: '',
            discountTypeName: '優惠券'
        }
        activityDiscountsDisplays.value.unshift(couponDisplay);
    }
}

function changeActivityDetail(index) {
    activityDetail.value = {
        name: activityDiscounts.value[index].activityDiscount.activityDetailName,
        startTime: formatDateTime(activityDiscounts.value[index].activityDiscount.activityDetailStartTime),
        endTime: formatDateTime(activityDiscounts.value[index].activityDiscount.activityDetailEndTime),
        priceThreshold: activityDiscounts.value[index].activityDiscount.priceThreshold
    }
    selectedItemIndex.value = index;
}

function formatDateTime(dateTimeStr) {
    const date = new Date(dateTimeStr);
    const year = String(date.getFullYear()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');

    return `${year}/${month}/${day} ${hours}:${minutes}`;
}
</script>
<template>
    <div>
        <v-card max-width='500' class="bg-grey-lighten-4 mx-3 mt-5">
            <div class="d-flex flex-column flex-md-row">
                <v-card-item class="width-left">
                    <div v-for="display in activityDiscountsDisplays.slice(0, 3)" class="mb-2 d-flex">
                        <v-chip size="small">
                            {{ display.discountTypeName }}
                        </v-chip>
                        <div v-if="display.discountTypeName === '優惠券'">
                            <CouponForProduct :couponDiscounts="couponDiscounts"></CouponForProduct>
                        </div>
                        <span v-else class="ml-3 text-body-2">{{ display.activityDetailName }}</span>
                    </div>
                </v-card-item>
                <v-card-item class="width-right align-self-md-end align-self-start pa-0 mb-4 ms-5 me-3 pe-5">
                    <div>
                        <v-btn :append-icon="mdiChevronRightCircle" variant="text" rounded="xl" class="bg-grey-darken-3"
                            size="small">看全部</v-btn>
                        <v-dialog v-model="dialog" width="40%" activator="parent">
                            <v-card>
                                <div class="mt-2 d-flex justify-space-between header">
                                    <v-card-title class="ml-3 text-h6 font-weight-black">優惠活動</v-card-title>
                                    <v-card-actions class="mr-2">
                                        <v-btn :icon="mdiClose" color="grey" @click="dialog = false"></v-btn>
                                    </v-card-actions>
                                </div>
                                <div class="d-flex">
                                    <v-card class="dialog-left" v-scroll.self="onScroll">
                                        <v-list density="compact">
                                            <v-list-item v-for="(item, index) in activityDiscounts" :key="index"
                                                :value="item" @click="changeActivityDetail(index)" color="yellow-darken-3"
                                                :active="selectedItemIndex === index">
                                                <p class="text-subtitle-1 font-weight-bold">
                                                    {{ item.activityDiscount.discountTypeName }}
                                                </p>
                                                <v-list-item-title v-text="item.activityDiscount.activityDetailName"
                                                    class="text-subtitle-2 ml-3 wrap-text"></v-list-item-title>
                                            </v-list-item>
                                        </v-list>
                                    </v-card>
                                    <v-card v-scroll.self="onScroll"
                                        class="overflow-y-auto my-5 mx-5 dialog-right bg-grey-lighten-3" height="300">
                                        <div class="mx-5 my-4">
                                            <p class="text-subtitle-1 font-weight-black">
                                                {{ activityDetail.name }}
                                            </p>
                                            <p class="text-caption">
                                                活動時間: {{ activityDetail.startTime }} - {{ activityDetail.endTime }}
                                            </p>
                                            <p class="text-caption">
                                                活動門檻: {{ activityDetail.priceThreshold }}
                                            </p>
                                            <p class="text-caption">
                                                活動注意事項：
                                            <ul class="no-marker">
                                                <li>1. 行銷活動限同一筆訂單且同一種配送方式。</li>
                                                <li>2. 第二件折扣之行銷活動，則依較低價商品為折扣商品。</li>
                                                <li>3. 優惠券可與其他優惠方案合併使用。</li>
                                                <li>4. 參與行銷活動之商品如欲退貨需全數一併退回，恕不接受單一商品辦理退貨。</li>
                                                <li>5. 加價購商品、語音接單(IVR)，及經由客服人員下單...等訂購方式，恕不列入贈品與折扣活動計算。</li>
                                            </ul>
                                            </p>
                                        </div>
                                    </v-card>
                                </div>
                            </v-card>
                        </v-dialog>
                    </div>
                </v-card-item>
            </div>
        </v-card>
    </div>
</template>
<style scoped>
.width-left {
    width: 75%;
}

.width-right {
    width: 25%;
}

.dialog-left {
    width: 40%;
}

.dialog-right {
    width: 60%;
}

.header {
    border-bottom: 1px solid rgb(206, 206, 206);
}

.no-marker {
    list-style-type: none;
}

.wrap-text {
    white-space: normal;
}
</style>