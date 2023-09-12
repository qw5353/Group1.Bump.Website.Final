<script setup>
import DefaultLayout from '../../components/layouts/DefaultLayout.vue'
import { ref, onMounted } from 'vue';
import { useRoute } from 'vue-router';
import axios from 'axios';
import { mdiShopping } from '@mdi/js';
import { useTitle } from '@vueuse/core';

useTitle('Bump - 活動');

const route = useRoute();
const activitys = ref({});
const activityName = ref("");
const activityDetails = ref([]);
const activityImg = ref('');
const activitySchedule = "/files/images/activity/週年慶活動總覽.png";
const colorSet = ["#FFC15B", "#FFA30D", "#78B8FF", "#305CA0", "#233C6F", "#FFC15B", "#FFA30D", "#78B8FF", "#305CA0", "#233C6F"];
const promotionProductTypeName = ["全館商品", "商品標籤"];

onMounted(async () => {
    const avtivityId = route.params.id;
    try {
        await initialize(avtivityId);
    } catch (err) {
        console.error(err);
    }
});

async function initialize(id) {
    const res = await axios.get(`/api/OnActivityDetails?id=${id}`);
    activitys.value = res.data;
    activityImg.value = "/files/images/second-home-carousel/" + activitys.value[0].thumbnail;
    activityDetails.value = activitys.value[0].activityDetails;
    activityName.value = activitys.value[0].name;

    activityDetails.value.map((item, index) => {
        item.thumbnail = "/files/images/activity/" + item.thumbnail;
        item.headerColor = { backgroundColor: colorSet[index] };
        item.borderColor = colorSet[index];
        if (item.activityDiscounts[0].promotionProductTypeName === promotionProductTypeName[0]) {
            item.goRouter = "/Shop";
        }
        else if (item.activityDiscounts[0].promotionProductTypeName === promotionProductTypeName[1]) {
            const productTagName = item.activityDiscounts[0].productTags[0].name;
            item.goRouter = "/Shop/OnSale?actName=" + productTagName;
        }
        else {
            item.goRouter = `/activity/${id}`
        }
    })
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
    <DefaultLayout>
        <main>
            <v-parallax height="600px" cover :src="`${activityImg}`"></v-parallax>
            <v-img v-if="activityName === '10周年周年慶'" class="my-10 mx-auto" width="80%" cover
                :src="`${activitySchedule}`"></v-img>
            <v-container class="mb-10">
                <div class="detail-div mx-auto mb-8" v-for="detail in activityDetails"
                    :style="{ 'border-color': detail.borderColor }">
                    <div :style="detail.headerColor" class="detail-div-header">
                        <h3 class="text-white text-center">{{ detail.name }}</h3>
                    </div>
                    <v-row class="my-3 mx-10">
                        <v-col cols="4">
                            <v-img :width="600" aspect-ratio="40/9" class="mx-auto" cover
                                :src="`${detail.thumbnail}`"></v-img>
                        </v-col>
                        <v-col cols="8">
                            <div class="mx-5">
                                <div>
                                    <span class="text-subtitle-1 font-weight-bold">活動時間: </span>
                                    <span class="text-red">
                                        <span class="text-subtitle-1">{{ formatDateTime(detail.startTime) }}</span>
                                        <span> ~ </span>
                                        <span class="text-subtitle-1">{{ formatDateTime(detail.endTime) }}</span>
                                    </span>
                                </div>
                                <div>
                                    <span class="text-subtitle-1 font-weight-bold">活動說明: </span>
                                    <span class="text-subtitle-1">{{ detail.description }}</span>
                                </div>
                            </div>
                            <div class="d-flex justify-end mt-3">
                                <router-link :to="detail.goRouter">
                                    <v-btn :prepend-icon="mdiShopping" rounded="xl" variant="text" class="bg-red-darken-4">
                                        <p class="text-h6 font-weight-bold">GO!</p>
                                    </v-btn>
                                </router-link>
                            </div>
                        </v-col>
                    </v-row>
                </div>
            </v-container>
        </main>
    </DefaultLayout>
</template>
<style scoped>
.detail-div {
    width: 80%;
    border: 4px solid black;
    border-radius: 20px;
}

.detail-div-header {
    border-radius: 15px 15px 0px 0px;
}
</style>