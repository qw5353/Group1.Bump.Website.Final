<script setup>
import { ref, computed } from 'vue';
import axios from 'axios';
import { defineProps } from 'vue';
import { ElMessage } from 'element-plus'

const props = defineProps({
    initialize: Function,
    coupon: Object
})
const dialog = ref(false);
const sendId = ref(0);
const account = ref("");
const email = ref("");
const showError = ref(false);
const errMsg = ref("");

const accountRules = computed(() => [
    (value) => {
        if (value) return true;
        return '帳號為必填';
    },
]);

const emailRules = computed(() => [
    (value) => {
        if (!value) return 'Email為必填';
        if (!(/^\w+([.-]?\w+)*@\w+([.-]?\w+)*(\.\w{2,3})+$/.test(value))) return 'Email格式有誤'

        return true;
    },
]);

function show() {
    sendId.value = props.coupon.id
    if (dialog.value == false) {
        clearForm();
    }
}

async function transferCouponSend() {
    try {
        const input = {
            sendId: sendId.value,
            account: account.value,
            email: email.value
        }
        const res = await axios.post(`/api/Coupons/TransferCouponSend`, input);

        showError.value = false;
        ElMessage({
            showClose: true,
            message: '轉移成功!',
            type: 'success',
        });
        dialog.value = false;
        props.initialize();

    } catch (err) {
        if (err.response.data == "空值") return
        console.error(err.response.data);
        showError.value = true;
        errMsg.value = err.response.data;

        setTimeout(() => {
            showError.value = false;
        }, 5000);
    }
}

function clearForm() {
    account.value = "";
    email.value = "";
    showError.value = false;
}
</script>
<template>
    <div class="text-left div-btn">
        <v-btn class="text-caption rounded-pill bg-grey-darken-2" @click="show">優惠券移轉</v-btn>
        <v-dialog v-model="dialog" activator="parent" width="450px">
            <v-form @submit.prevent="transferCouponSend">
                <v-card>
                    <v-card-text>
                        <v-container>
                            <v-row>
                                <v-col class="text-h6 text-center" cols="12" sm="3">
                                    <p class="mt-3">帳號</p>
                                </v-col>
                                <v-col cols="12" md="9">
                                    <v-text-field label="請輸入移轉帳戶帳號*" v-model="account" :rules="accountRules"></v-text-field>
                                </v-col>
                                <v-col class="text-h6 text-center" cols="12" sm="3">
                                    <p class="mt-3">Email</p>
                                </v-col>
                                <v-col cols="12" md="9">
                                    <v-text-field label="請輸入移轉帳戶Email*" v-model="email" :rules="emailRules"></v-text-field>
                                </v-col>
                            </v-row>
                        </v-container>
                        <small class="text-red">*必填</small>
                        <transition name="fade" mode="out-in">
                            <v-alert v-if="showError" class="mt-3" type="error" variant="tonal">
                                {{ errMsg }}
                            </v-alert>
                        </transition>
                    </v-card-text>
                    <v-card-actions>
                        <v-btn type="submit" class="dialogConfirm font-weight-black" color="white" block>確定</v-btn>
                    </v-card-actions>
                </v-card>
            </v-form>
        </v-dialog>
    </div>
</template>
<style scoped>
.div-btn {
    width: 68px;
}

.dialogConfirm {
    background-color: #FFB23E;
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