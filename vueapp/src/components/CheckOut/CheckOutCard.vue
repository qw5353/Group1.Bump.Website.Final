<script setup>
import { ref, computed, onMounted, watch } from 'vue';
import OrderContent from "./01CheckOutItems.vue";
import PersonalInfo from "./02CheckOutItems.vue";
import CheckOut from "./03CheckOutItems.vue";
import CouponBtn from './CouponBtn.vue';
import { useCheckOutStore } from '../../stores/CheckOutStore';
import { userStateStore } from '../../stores/UserStateStore.js';
import { storeToRefs } from 'pinia';
import _ from 'lodash';
import axios from 'axios';
import { ElMessage } from 'element-plus'

const userStore = userStateStore();
const checkoutStore = useCheckOutStore();
const { items, selectedDiscount } = storeToRefs(checkoutStore);
const selectedCouponId = storeToRefs(checkoutStore);

const total = computed(() => {
    return _.sumBy(items.value, function (o) {
        return o.subTotal;
    })
})
const levelred = ref({
    1: 0.02,
    2: 0.05,
    3: 0.08
})

const tab = ref(1);
const titles = ref([
    { t: "1.訂單內容" },
    { t: "2.填寫資料" },
    { t: "3.結帳" }
])


const requiredRules = ref([
    value => (!value || (value && value >= 0) || '不得輸入負數'),
    value => (!value || (value && value <= memeberPoints.value.points) || '不得超過上限')
])

const memeberPoints = ref();

const usePoint = ref(0);
const showPoint = ref(0);


onMounted(async () => {
    try {
        const res = await axios.get(`/api/cart/memberPoint?id=${userStore.userId}`);
        memeberPoints.value = res.data;
        console.log(memeberPoints.value);
    } catch (err) {
        console.error(err);
    }
});


const freightPrice = computed(() => total.value > 3000 ? 0 : 60);
const orderTotal = computed(() => total.value + freightPrice.value - (showPoint.value ?? 0
) - (selectedDiscount.value.discountPrice ?? 0));
const getRedpoint = computed(() => parseInt(levelred.value[items.value[0].memberLevel] * orderTotal.value))
const redPiontPrice = computed(() => showPoint.value);

function next() {
    if (tab.value == 2) {
        tab.value = 3;
    } else if (tab.value == 1) {
        tab.value = 2;
    }
}

function clickUsePoint() {
    if (/^[1-9]\d*$/.test(usePoint.value)) {
        showPoint.value = usePoint.value;
      } else {
        showPoint.value = 0;
        ElMessage({
            showClose: true,
            message: '只能使用整數!',
            type: 'warning',
        })
      }
}


const orderInfo = ref();

const checkoutData = ref({
    MemberId: userStore.userId,
    RecipientName: '',
    RecipientPhone: '',
    RecipientEmail: '',
    RecipientAddress: '',
    OrderStatusId: 1,
    Note: '',
    TotalProductsPrice: orderTotal,
    DeliverPrice: freightPrice,
    DiscountPrice: 0,
    UsedPoint: redPiontPrice,
    CouponId: selectedCouponId.selectedCouponId,
    GetRedPoint: getRedpoint,
    CheckoutItems: computed(() => items.value.map(item => {
        return {
            ProductId: item.productId,
            Quantity: item.quantity,
            UnitPrice: item.price,
            ProductName: item.name,
            Subtotal: item.subTotal,
            ProductStyleId: item.productStylesId,
            CartDetailId: item.cartDetailId
        }
    }))
});

watch(orderInfo, function () {
    checkoutData.value.RecipientName = orderInfo.value.name
    checkoutData.value.RecipientPhone = orderInfo.value.phone
    checkoutData.value.RecipientEmail = orderInfo.value.email
    checkoutData.value.RecipientAddress = orderInfo.value.address
    checkoutData.value.Note = orderInfo.value.note

}, {
    deep: true
});

watch(selectedDiscount, function () {
    checkoutData.value.DiscountPrice = selectedDiscount.value.discountPrice
})



// const checkoutItemsData = computed(() => items.value.map(item => {
//     return {
//         ProductId: item.productId,
//         Quantity: item.quantity,
//         UnitPrice: item.price,
//         ProductName: item.name,
//         Subtotal: item.quantity * item.price,
//     }
// }));

// orderTotal = total - (freightPrice.value + usePoint.value ? usePoint.value : 0 + selectedDiscount.discountPrice ? selectedDiscount.discountPrice : 0);
</script>

<template>
    <v-card>
        <v-tabs v-model="tab" color="orange-darken-4" grow align-tabs="center">
            <v-tab v-for="(title, index) in titles" :key="title" :value="index + 1"
                class="w-15 h-auto text-h6 font-weight-bold spacing">
                {{ title.t }}
            </v-tab>
        </v-tabs>
        <v-window v-model="tab">
            <v-window-item :key="1" :value="1">
                <v-container fluid>
                    <OrderContent />
                </v-container>
            </v-window-item>
            <v-window-item :key="2" :value="2">
                <v-container fluid>
                    <PersonalInfo @order-info="(info) => orderInfo = info" />
                </v-container>
            </v-window-item>
            <v-window-item :key="3" :value="3">
                <v-container fluid>
                    <CheckOut :orderInfo3="orderInfo" :checkoutData="checkoutData" />
                </v-container>
            </v-window-item>
        </v-window>
    </v-card>
    <div class="my-10">
        <div class="mb-6" style="align-content: center;">
            <div id="div1" style="display: inline-block !important;">
                <h2 style="width: 300px;">訂單總計 : &emsp;${{ orderTotal }}</h2>

                <h2 style="width: 300px;" v-if="items.length > 0" class="text-orange-darken-4">紅利累積 : &emsp;{{
                    getRedpoint }}</h2>
            </div>

            <div id="div2" style="display: inline-block !important;">
                <p>訂單小計 :</p>
                <p>運費(滿3000免運) :</p>
                <p>紅利折抵 :</p>
                <p>優惠券折抵 :</p>
            </div>
            <div id="div3" style="display: inline-block !important;">
                <p>$ {{ total }}</p>
                <p>$ {{ freightPrice }}</p>
                <p>$ {{ showPoint }}</p>
                <p v-if="selectedDiscount">$ {{ selectedDiscount?.discountPrice ?? 0 }}</p>
            </div>
            <div id="div4" style="display: inline-block !important;" class="">
                <p v-if="memeberPoints?.points">剩餘紅利 : {{ memeberPoints.points }}</p>

                <div class="redPiont">
                    <v-text-field :rules="requiredRules" label="使用紅利" class="w-50" v-model.number="usePoint"></v-text-field>
                    <v-btn color="amber-lighten-3 ms-5 mb-6 px-6" @click="clickUsePoint">折抵</v-btn>
                </div>
                <div class="redPiont">
                    <CouponBtn></CouponBtn>
                </div>
            </div>
        </div>
        <v-row no-gutters class="align-end d-flex">
            <v-col cols="10"></v-col>
            <v-col cols="2" class="d-flex">
                <!-- <v-btn v-if="tab === 3" @click="checkout" color="warning"
                    class="w-15 h-auto text-h5 font-weight-bold ms-auto py-2">去結帳</v-btn> -->
                <v-btn v-if="tab === 1 || tab === 2" @click="next()" color="warning"
                    class="w-15 h-auto text-h5 font-weight-bold ms-auto py-2">下一步</v-btn>
            </v-col>
        </v-row>
    </div>
</template>

<style scoped>
.spacing {
    letter-spacing: 0.2em !important;
}

#div1 {
    width: 350px !important;
    line-height: 60px;
}

#div2 {
    width: 150px !important;
    line-height: 40px;
}

#div3 {
    width: 150px !important;
    line-height: 40px;
}

#div4 {
    width: 350px !important;
    line-height: 40px;
}

.redPiont {
    display: flex;
    justify-content: flex-end;
    justify-content: space-between;
    align-items: end;
}
</style>