<script setup>
import { ref, onMounted, computed } from 'vue'
import MemberLevelCard from './MemberLevelCard.vue'
import { ElCarousel, ElCarouselItem } from 'element-plus'
import { mdiChartBubble } from '@mdi/js';
import axios from 'axios';
import { max } from 'lodash';

const currentOrderSum = ref(0)
const currentOrderCount = ref(0)
const currentSumPercent = ref(0)
const currentOrderPercent = ref(0)

const cards = ref([{
    "memberCardId": 1,
    "levelOrder": 1,
    "levelName": "一般會員",
    "name": "腕力",
    "upgradePrice": 0,
    "upgradeOrderCount": 0,
    "timePeriod": 0,
    "gainPointRate": 0.02,
    "description": "會員等級LV1(預設) 無升級條件 永久會員 回饋點數2%",
    'color': 'amber-accent-4'
}])

const userCard = ref({
    "memberId": 1,
    "memberCardId": 3,
    "levelOrder": 3,
    "levelName": "高級會員",
    "name": "怪力",
    "upgradePrice": 15000,
    "upgradeOrderCount": 10,
    "leftTimePeriod": "295.08:03",
    "gainPointRate": 0.08,
    "description": "會員等級LV3 升級條件$15000&&滿10筆訂單 會員期限12個月 回饋點數8%",
    "startDate": "2023-06-22",
    "endDate": "2024-06-22"
})

const nextCard = ref({
    "memberCardId": 2,
    "levelOrder": 2,
    "levelName": "進階會員",
    "name": "豪力",
    "upgradePrice": 6000,
    "upgradeOrderCount": 6,
    "timePeriod": 12,
    "gainPointRate": 0.05,
    "description": "會員等級LV2 升級條件$6000&&滿6筆訂單 會員期限12個月 回饋點數5%"
})

async function getCards() {
    const res = await axios.get('/api/MemberLevel/MemberCards')
    cards.value = res.data
}

// const index = cards.value.findIndex(card => card.memberCardId === userCard.value.memberCardId)

async function theCard() {
    const res = await axios.get('/api/MemberLevel/TheMemberCard')
    userCard.value = res.data

    userCard.value.startDate = userCard.value.startDate.split('T')[0]
    userCard.value.endDate = userCard.value.endDate.split('T')[0]

    const index = cards.value.findIndex(card => card.memberCardId === userCard.value.memberCardId)
    if (index > -1) {
        cards.value[index] = { ...userCard.value, 'color': 'amber-darken-4' }
    }
}

async function theNextCard() {
    const res = await axios.get('/api/MemberLevel/TheNextCard')
    nextCard.value = res.data
    // console.log(nextCard.value)
}

async function currentSum() {
    const res = await axios.get('/api/MemberLevel/CurrentOrderSum')
    currentOrderSum.value = res.data

    currentSumPercent.value = (currentOrderSum.value / nextCard.value.upgradePrice) >= 1 ?
        100 : (currentOrderSum.value / nextCard.value.upgradePrice) * 100
}

async function currentCount() {
    const res = await axios.get('/api/MemberLevel/CurrentOrderCount')
    currentOrderCount.value = res.data

    currentOrderPercent.value = (currentOrderCount.value / nextCard.value.upgradeOrderCount) >= 1 ?
        100 : (currentOrderCount.value / nextCard.value.upgradeOrderCount) * 100
}

const carouselRef = ref(null)

onMounted(async () => {
    await getCards()
    await theCard()
    await theNextCard()
    await currentSum()
    await currentCount()
})

</script>
<template>
    <v-card variant="flat">
        <v-row class="mt-10">
            <v-col>
                <el-carousel ref="carousel" type="card" height="280px" :autoplay="false" arrow="never"
                    :initial-index="(userCard.memberCardId - 1)">
                    <el-carousel-item v-for="card in cards" :key="card.memberCardId">
                        <h3 text="2xl" justify="center">
                            <MemberLevelCard :card="card"></MemberLevelCard>
                        </h3>
                    </el-carousel-item>
                </el-carousel>
            </v-col>
        </v-row>
        <v-row>
            <v-col class="text-body-1 d-flex align-center justify-center">
                <p>距離進化為</p>
                <v-chip class="ma-2" color="warning" size="large" label>
                    {{ nextCard.name }}
                </v-chip>
                <p>還剩</p>
                <p class="text-overline font-weight-bold">&emsp;{{ userCard.leftTimePeriod.split('.')[0] }}&emsp;</p>
                <p>天</p>
                <v-icon :icon="mdiChartBubble" class="mx-2"></v-icon>
                <p>完成度:</p>
            </v-col>
        </v-row>
        <v-row class="justify-center">
            <v-col cols="10">
                <v-progress-linear :model-value="((currentSumPercent + currentOrderPercent) / 2)" color="amber-darken-4"
                    height="25">
                    <strong>{{ Math.ceil((currentSumPercent + currentOrderPercent) / 2) }}%</strong>
                </v-progress-linear>
            </v-col>
        </v-row>
        <v-row class="mt-12 justify-center">
            <v-col cols="auto"></v-col>
            <v-col cols="3" class="text-center text-subtitle-1 font-weight-black">消費金額</v-col>
            <v-col cols="3" class="text-center text-subtitle-1 font-weight-black">訂單筆數</v-col>
            <v-col cols="auto"></v-col>
        </v-row>
        <v-row class="justify-center">
            <v-col cols="auto"></v-col>
            <v-col cols="3" class="text-center"><v-progress-circular v-if="nextCard" :rotate="360" :size="200" :width="15"
                    :model-value="currentSumPercent" color="warning">
                    <p class="text-h5 font-weight-bold">{{ currentOrderSum }}</p>
                    <p class="align-self-end text-button">&nbsp;/ {{ nextCard.upgradePrice }}</p>
                </v-progress-circular>
            </v-col>
            <v-col cols="3" class="text-center"><v-progress-circular v-if="nextCard" :rotate="360" :size="200" :width="15"
                    :model-value="currentOrderPercent" color="warning">
                    <p class="text-h5 font-weight-bold">{{ currentOrderCount }}</p>
                    <p class="align-self-end text-button">&nbsp;/ {{ nextCard.upgradeOrderCount }}</p>
                </v-progress-circular>
            </v-col>
            <v-col cols="auto"></v-col>
        </v-row>
    </v-card>
</template>
