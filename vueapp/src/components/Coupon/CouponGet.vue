<script setup>
import { ref } from 'vue';
import axios from 'axios';
import { defineProps } from 'vue';

const code = ref("");
const props = defineProps({
    initialize: Function
})

const showSuccess = ref(false);
const showError = ref(false);
const errMsg = ref("");

async function createCouponSend() {
    try {
        const input = {
            code: code.value,
        }
        const res = await axios.post(`/api/Coupons/CreateCouponSend`, input);

        props.initialize();
        showSuccess.value = true;
        showError.value = false;

        setTimeout(() => {
            showSuccess.value = false;
        }, 5000);
    } catch (err) {
        console.error(err.response.data);

        showSuccess.value = false;
        showError.value = true;
        errMsg.value = err.response.data;

        setTimeout(() => {
            showError.value = false;
        }, 5000);
    }
}


</script>
<template>
    <div class="allDiv mx-auto">
        <div class="mt-5 bg-grey-lighten-4 rounded-lg py-6">
            <v-row class="input-label mx-auto">
                <p class="text-h6 ml-3">優惠券歸戶</p>
            </v-row>
            <v-row class="v-row-btn-input mx-auto justify-space-between">
                <v-col cols="10">
                    <v-text-field class="bg-white btn-input-height input" label="請輸入8碼優惠券序號" single-line variant="outlined"
                        v-model="code"></v-text-field>
                </v-col>
                <v-col cols="2">
                    <v-btn class="btn btn-input-height text-subtitle-1" @click="createCouponSend">歸戶</v-btn>
                </v-col>
            </v-row>
        </div>
        <transition name="fade" mode="out-in">
            <v-alert class="mt-3" v-if="showSuccess" type="success" variant="tonal">
                優惠券歸戶成功！ 請留意優惠券的使用期限，以免錯過專屬優惠
            </v-alert>
        </transition>

        <transition name="fade" mode="out-in">
            <v-alert class="mt-3" v-if="showError" type="error" variant="tonal">
                {{ errMsg }}
            </v-alert>
        </transition>
    </div>
</template>
<style scoped>
.input-label {
    height: 30px;
    width: 100%;
}

.v-row-btn-input {
    height: 80px;
    width: 100%;
}

.input {
    width: 100%;
}

.btn-input-height {
    height: 50px;
}

.btn {
    width: 100%;
    background-color: #FFB23E;
}

.allDiv {
    width: 100%;
}

.fade-enter-active,
.fade-leave-active {
    transition: opacity 0.15s;
}

.fade-enter,
.fade-leave-to

/* .fade-leave-active in <2.1.8 */
    {
    opacity: 0;
}
</style>