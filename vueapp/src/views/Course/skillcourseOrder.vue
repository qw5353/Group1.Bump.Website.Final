<script setup>
import { ref, computed, onMounted } from 'vue';
import DefaultLayout from '../../components/layouts/DefaultLayout.vue';
import { useTitle } from '@vueuse/core';
import { userStateStore } from '../../stores/UserStateStore';
import { useRoute, useRouter } from 'vue-router'
import axios from 'axios';

useTitle('Bump - 課程');

const title = ref(new URL('/src/assets/images/course-index-carousel/title.jpg', import.meta.url).href);
const router = useRouter();

//抓取會員資料
const useStore = userStateStore()

const name = useStore.userName
const phoneNumber = ref('0912345678')
const email = useStore.userEmail
const MemberId = useStore.userId

const people = ref(1)
const peopleOptions = ref([1, 2, 3, 4])

const calculateSum = computed(() => {
    return people.value * course2.value.price;
});


const tab = ref(1)

function next() {
    if (tab.value == 1) {
        tab.value = 2;
    }
}

const opened = ref(['aio']);

const route = useRoute()
const SkillcurriculumsDetial = ref([]);
const course2 = ref([]);

onMounted(async () => {
    try {
        const res = await fetch(`/api/Skillcurriculums/${route.params.id}`)
        SkillcurriculumsDetial.value = await res.json();
        course2.value = SkillcurriculumsDetial.value;
    } catch (err) {
        console.log(err)
    }
})

//送出表單
const todata = computed(() => {
    if (course2.value) {
        return {
            SkillcurriculumsId: course2.value.id,
            MemberId: MemberId,
            NumberOfPeople: people.value,
            amount: calculateSum.value
        };
    } else {
        return null; // 或者其他适当的默认值
    }
});
const responseData = ref(null);
const error = ref(null);
console.log(todata.value)
const postData = async (payType) => {
    try {
        const response = await axios.post('/api/SkillEnrollment', { ...todata.value, payType });
        switch (payType) {
            case "Line Pay":
                location.href = response.data;
                break;
            case "ECPay":
                router.push({ path: "/pay/ecpay/submit", query: response.data });
                break;
            default:
                break;
        }
        error.value = null;
    } catch (e) {
        error.value = e.message;
        responseData.value = null;
    }
};
</script>

<template>
    <DefaultLayout>
        <v-container>
            <v-row no-gutters>
                <v-col cols="12" class="d-flex justify-center">
                    <v-img width="75%" aspect-ratio="16/9" cover :src="title" transition="slide-x-transition"></v-img>
                </v-col>
            </v-row>
        </v-container>
        <v-container>
            <v-row>
                <v-col cols="8">
                    <v-card>
                        <v-tabs v-model="tab" color="orange-darken-4" grow align-tabs="center">
                            <v-tab :value="1">填寫資料</v-tab>
                            <v-tab :value="2">結帳</v-tab>
                        </v-tabs>
                        <v-card-text>
                            <v-window v-model="tab">
                                <v-window-item :value="1">
                                    <v-form>
                                        <h2 class="mb-2 ms-2">報名資料:</h2>
                                        <v-text-field v-model="name" label="name" readonly
                                            variant="outlined"></v-text-field>
                                        <v-text-field v-model="phoneNumber" label="phoneNumber" readonly
                                            variant="outlined"></v-text-field>
                                        <v-text-field v-model="email" label="email" readonly
                                            variant="outlined"></v-text-field>

                                        <v-select v-model="people" :items="peopleOptions" label="人數" variant="outlined"
                                            suffix="人"></v-select>
                                        <v-row class="mt-4">
                                            <v-col cols="auto" class="ml-auto">
                                                <v-btn color="warning" variant="outlined" @click="next()">下一步</v-btn>
                                            </v-col>
                                        </v-row>
                                    </v-form>
                                </v-window-item>
                                <v-window-item :value="2">


                                    <div id="row" class="my-5">
                                        <v-expansion-panels class="w-75" v-model="opened">
                                            <v-expansion-panel class="mb-3" value="aio">
                                                <v-expansion-panel-title color="#616161">
                                                    <p class="text-white text-h6 font-weight-bold ">綠界 AIO 金流支付</p>
                                                </v-expansion-panel-title>
                                                <v-expansion-panel-text>
                                                    <div class="logo">
                                                        <img src="../../assets/images/pay/ecpay_logo.svg" alt="綠界"
                                                            style="width: 80px;">
                                                    </div>
                                                    <p id="font" class="mt-6">使用綠界 AIO 金流支付 <br>

                                                        可使用 信用卡付款、網路 ATM、ATM 櫃員機、超商條碼、超商代碼 付款<br>

                                                        點選確認配合銀行
                                                    </p>

                                                    <div id="btn">
                                                        <v-spacer></v-spacer>
                                                        <v-btn class="text-white text-none mb-3 text-subtitle-1"
                                                            color="warning" rounded="1" @click="postData('ECPay')">
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
                                                        <img src="../../assets/images/pay/LINE_Pay_logo_(2019).png"
                                                            alt="LINE Pay" style="width: 80px;">
                                                    </div>
                                                    <p id="font" class="mt-6">使用LINE Pay 行動支付 <br>

                                                        只需使用行動裝置掃碼付款即可完成交易
                                                    </p>

                                                    <div id="btn">
                                                        <v-spacer></v-spacer>
                                                        <v-btn class="text-white text-none mb-3 text-subtitle-1"
                                                            color="warning" rounded="1" @click="postData('Line Pay')">
                                                            結帳
                                                        </v-btn>
                                                    </div>
                                                </v-expansion-panel-text>
                                            </v-expansion-panel>
                                        </v-expansion-panels>
                                    </div>


                                </v-window-item>
                            </v-window>
                        </v-card-text>
                    </v-card>
                </v-col>
                <v-col cols="4">
                    <v-card class="mx-auto mt-6" max-width="400" v-if="course2">
                        <v-img class="align-end text-white" height="200"
                            :src="`/files/images/SkillCourses/${course2.thumbnail}`" cover>
                            <v-card-title>{{ course2.skillCoursesName }}</v-card-title>
                        </v-img>

                        <v-card-subtitle class="pt-4">{{ course2.name }}</v-card-subtitle>

                        <v-card-text>
                            <div>
                                <p>NT${{ course2.price }} × {{ people }}人</p>
                            </div>

                            <div>
                                <h3>總計:{{ calculateSum }}元</h3>
                            </div>
                        </v-card-text>

                    </v-card>


                </v-col>
            </v-row>
        </v-container>
    </DefaultLayout>
</template>

<style scoped>
.test {
    margin: 0;
    padding: 0;
}

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