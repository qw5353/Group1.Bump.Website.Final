<script setup>
import { ref, onMounted } from 'vue';
import axios from 'axios';

const props = defineProps(['id', 'price'])

const activityDiscounts = ref([]);
const productId = props.id;
const productPrice = props.price;
const maxDiscountPrice = ref(0);
const maxDiscountType = ref("");
const maxDiscountAmount = ref("");
const maxDiscountDisplay = ref("");
const maxDiscountShow = ref(false);
const maxDiscountADName = ref("");
const maxDiscountPTName = ref("");

const emit = defineEmits(['sendName']);

onMounted(async () => {
    try {
        const input = {
            productId: productId,
        }
        const res = await axios.post('/api/ActivityDiscounts/GetProductDiscount', input);
        activityDiscounts.value = res.data;

        maxDiscountPrice.value = Math.max(...activityDiscounts.value.map(item => item.totalDiscountPrice));
        const index = activityDiscounts.value.map(item => item.totalDiscountPrice).indexOf(maxDiscountPrice.value);
        maxDiscountAmount.value = activityDiscounts.value[index].activityDiscount.amount;
        maxDiscountType.value = activityDiscounts.value[index].activityDiscount.discountTypeName;
        maxDiscountDisplay.value = amountDisplay(maxDiscountAmount.value, maxDiscountType.value, productPrice, maxDiscountPrice.value);
        if (maxDiscountDisplay.value !== null) {
            maxDiscountShow.value = true;
        }
        maxDiscountADName.value = activityDiscounts.value[index].activityDiscount.activityDetailName;
        maxDiscountPTName.value = activityDiscounts.value[index].activityDiscount.productTagName;

        emit('sendName', maxDiscountADName.value, maxDiscountPTName.value)
    }
    catch (err) {
        console.error(err);
    }
})

function amountDisplay(amount, discountType, price, dicountPrice) {
    if (dicountPrice === 0) return null;
    if (discountType == '打折') return amount * 100;
    if (discountType == '折抵') return Math.ceil((price - amount) / price * 100);
}

</script>
<template>
    <div class="container">
        <div v-if="maxDiscountDisplay !== null" v-show="maxDiscountShow">
            <svg class="svg-shape" xmlns="http://www.w3.org/2000/svg" width="73" height="23" viewBox="0 0 73 23"
                fill="none">
                <path d="M71 1H1V22H51.5556L71 1Z" fill="black" stroke="black" />
            </svg>
            <p class="overlay-text text-white text-body-2 font-weight-bold">{{ maxDiscountDisplay }}%</p>
        </div>
    </div>
</template>
<style scoped>
.container {
    display: flex;
    align-items: top;
    justify-content: left;
    position: relative;
    width: 50px;
    height: 25px;
}

.svg-shape {
    position: absolute;
}

.overlay-text {
    position: absolute;
    font-family: 'Inria Sans', sans-serif !important;
    left: 35%;
}
</style>