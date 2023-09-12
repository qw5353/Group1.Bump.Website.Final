<script setup>
import { ref, defineProps } from 'vue';
import { ElMessage } from 'element-plus'
import axios from 'axios';
import { useRouter } from 'vue-router';

const opened = ref(['aio']);
const router = useRouter();

const props = defineProps(['orderInfo3', 'checkoutData']);

async function greenBTN() {
    const receiverIsValid = validReceiver();

    if (!receiverIsValid) {
        ElMessage({
            showClose: true,
            message: '喔不，有欄位格式錯誤',
            type: 'error',
        })
    } else {
        try {
            const res = await axios.post('/api/Cart/checkout', { ...props.checkoutData, payType: "ECPay" });
            router.push({ path: "/pay/ecpay/submit", query: res.data });
        } catch (err) {
            ElMessage({
                showClose: true,
                message: '喔不，結帳失敗!',
                type: 'error',
            })
        }
    }
}

async function lineBTN() {
    const receiverIsValid = validReceiver();

    if (!receiverIsValid) {
        ElMessage({
            showClose: true,
            message: '喔不，有欄位格式錯誤',
            type: 'error',
        })
    } else {
        try {
            console.log(props.checkoutData);
            const payUrl = await axios.post('/api/Cart/checkout', { ...props.checkoutData, payType: "Line Pay" });
            location.href = payUrl.data;
        } catch (err) {
            ElMessage({
                showClose: true,
                message: '喔不，結帳失敗!',
                type: 'error',
            })
        }
    }

}

function validReceiver() {
    for (const key in props.orderInfo3) {
        if (key === 'note') continue;
        if (!props.orderInfo3[key]) {
            return false;
        }
    }
    return true;
}
</script>

<template>
    <div id="row" class="my-5">
        <v-expansion-panels class="w-75" v-model="opened">
            <v-expansion-panel class="mb-3" value="aio">
                <v-expansion-panel-title color="#616161">
                    <p class="text-white text-h6 font-weight-bold ">綠界 AIO 金流支付</p>
                </v-expansion-panel-title>
                <v-expansion-panel-text>
                    <div class="logo">
                        <img src="../../assets/images/pay/ecpay_logo.svg" alt="綠界" style="width: 80px;">
                    </div>
                    <p id="font" class="mt-6">使用綠界 AIO 金流支付 <br>

                        可使用 信用卡付款、網路 ATM、ATM 櫃員機、超商條碼、超商代碼 付款<br>

                        點選確認配合銀行
                    </p>

                    <div id="btn">
                        <v-spacer></v-spacer>
                        <v-btn class="text-white text-none mb-3 text-subtitle-1" color="warning" rounded="1"
                            @click="greenBTN">
                            結帳
                        </v-btn>
                    </div>
                </v-expansion-panel-text>
            </v-expansion-panel>

            <v-expansion-panel>
                <v-expansion-panel-title color="#616161">
                    <p class="text-white text-h6 font-weight-bold ">LINE Pay 行動支付</p>
                </v-expansion-panel-title>
                <v-expansion-panel-text>
                    <div class="logo">
                        <img src="../../assets/images/pay/LINE_Pay_logo_(2019).png" alt="LINE Pay" style="width: 80px;">
                    </div>
                    <p id="font" class="mt-6">使用LINE Pay 行動支付 <br>

                        只需使用行動裝置掃碼付款即可完成交易
                    </p>

                    <div id="btn">
                        <v-spacer></v-spacer>
                        <v-btn class="text-white text-none mb-3 text-subtitle-1" color="warning" rounded="1"
                            @click="lineBTN">
                            結帳
                        </v-btn>

                    </div>
                </v-expansion-panel-text>
            </v-expansion-panel>
        </v-expansion-panels>
    </div>
</template>

<style scoped>
#btn {
    display: flex;
    justify-content: flex-end;
    justify-content: space-between;
    align-items: end;
}

#row {
    display: flex;
    justify-content: center;
}

#font {
    letter-spacing: 0.2em !important;
    line-height: 2em;
}

.logo {
    float: right;
}
</style>