<script setup>
import { ref } from 'vue';
import DefaultLayout from '../../components/layouts/DefaultLayout.vue';
import { useTitle } from '@vueuse/core';
const title = ref(new URL('/src/assets/images/course-index-carousel/title.jpg', import.meta.url).href);
useTitle('Bump - 體驗課程');

const latestEnrollment = ref([])
const loadEnrollment = async () => {
    const response = await fetch("/api/ExperienceEnrollments")
    const data = await response.json();
    latestEnrollment.value = data;
}
loadEnrollment();

</script>

<template>
    <DefaultLayout>
        <v-container>
            <v-row no-gutters>
                <v-col cols="12" class="d-flex justify-center">
                    <v-img width="75%" aspect-ratio="16/9" cover :src="title" transition="slide-x-transition"></v-img>
                </v-col>
            </v-row>
            <v-row class="ms-20 me-20 d-flex justify-center">
                <v-col cols="6">
                    <v-card>
                        <v-img class="align-end text-white" height="200"
                            src="https://cdn.vuetifyjs.com/images/cards/docks.jpg" cover></v-img>
                        <v-card-title>
                            <v-container>
                                <v-row>
                                    <v-col cols="12" class="text-center">
                                        <h2>課程報名成功</h2>
                                    </v-col>
                                </v-row>
                            </v-container>
                        </v-card-title>
                        <v-card-subtitle>
                            <v-container>
                                <v-row no-gutters><v-col cols="4"></v-col>
                                    <v-col cols="4">
                                        <div>
                                            <p>您已成功報名體驗課，請記得於活動當天出席，以下為活動資訊:</p>
                                            <p>體驗人:{{ latestEnrollment.memberName }}</p>
                                            <p>體驗課時間:{{ latestEnrollment.startTime }}至{{ latestEnrollment.endTime }}</p>
                                            <p>體驗課地點:{{ latestEnrollment.fieldName }}</p>
                                            <p>報名成立時間:{{ latestEnrollment.createdAt }}</p>
                                            <br>
                                        </div>
                                    </v-col><v-col cols="4"></v-col>
                                </v-row>
                            </v-container>
                        </v-card-subtitle>
                        <v-card-text>
                            <v-container>
                                <v-row no-gutters>
                                    <v-col cols="4"></v-col>
                                    <v-col cols="4">
                                        <h2>注意事項:</h2>
                                        <p>請自備水壺或水杯（岩館提供飲水設備）<br>
                                            穿著舒適、適合運動之服裝<br>
                                            請事先修剪指甲<br>
                                            建議穿著或攜帶襪子</p>
                                    </v-col>
                                    <v-col cols="4"></v-col>
                                </v-row>
                            </v-container>
                        </v-card-text>
                        <v-card-actions>
                            <v-container>
                                <v-row>
                                    <v-col cols="12" class="text-end">
                                        <RouterLink to="/">
                                            <v-btn variant="outlined">
                                                返回首頁
                                            </v-btn>
                                        </RouterLink>
                                    </v-col>
                                </v-row>
                            </v-container>
                        </v-card-actions>
                    </v-card>
                </v-col>
            </v-row>
        </v-container>
    </DefaultLayout>
</template>

<style scoped>
.bgc2 {
    background: linear-gradient(90deg, #FEEDD7 0%, #FCCC8D);
}
</style>