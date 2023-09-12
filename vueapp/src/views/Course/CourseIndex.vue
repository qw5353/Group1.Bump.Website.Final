<script setup>
import { ref } from 'vue';
import DefaultLayout from '../../components/layouts/DefaultLayout.vue';
import { useTitle } from '@vueuse/core';

useTitle('Bump - 課程');

const firstCarousel = ref([
    new URL('/src/assets/images/course-index-carousel/slider1.jpg', import.meta.url).href,
    new URL('/src/assets/images/course-index-carousel/slider2.jpg', import.meta.url).href,
    new URL('/src/assets/images/course-index-carousel/slider3.jpg', import.meta.url).href
]);
const skillcourse = ref(new URL('/src/assets/images/course-index-carousel/skillcourse.jpg', import.meta.url).href);
const experencecourse = ref(new URL('/src/assets/images/course-index-carousel/experencecourse.jpg', import.meta.url).href);

</script>

<template>
    <DefaultLayout>
        <div width="1000">
            <v-carousel height="60vh" hide-delimiter-background cycle interval="3000">
                <v-carousel-item class="carousel-item" v-for="link in firstCarousel" :key="link" :src="link"
                    cover></v-carousel-item>
            </v-carousel>
        </div>

        <v-container fluid class="p-0 bgc">
            <v-row no-gutters>
                <v-col cols="6" class="align-center">
                    <v-img width="75%" aspect-ratio="16/9" cover :src="skillcourse" class="ms-auto my-auto"
                        transition="slide-x-transition"></v-img>
                </v-col>
                <v-col cols="4" class="centered-container p-12 justify-center">
                    <div class="w-75 d-flex justify-center flex-column ms-8">
                        <h2 style="letter-spacing:2px;">我們的技巧課</h2>
                        <p style="letter-spacing:2px; text-align: justify;" class="my-10">
                            &emsp;&emsp;攀岩是一項需要登高離地的運動項目，因此不管是新手還是老手來說，做好安全措施絕對是最重要的一環；除了帶好相對應的護具、頭盔，確保安全帶及扣環是有扣緊的，並且新手們必須要有專業且合格的攀岩教練在旁協助，確保進行攀岩運動時的安全。
                        </p>

                        <v-btn to="/SkillSelectField" color="warning" class="w-25">報名去</v-btn>
                    </div>
                </v-col>
            </v-row>
        </v-container>

        <v-container fluid class="p-0 bgc2">
            <v-row no-gutters>
                <v-col cols="4" offset="2" class="centered-container p-12">
                    <div class="w-75 d-flex justify-center flex-column mx-auto ms-8">
                        <h2 style="letter-spacing:2px;" class="ms-auto">我們的體驗課</h2>
                        <p style="letter-spacing:2px; text-align: justify;" class="my-10">
                            &emsp;&emsp;開始時教練會講解一些基本的知識，教你怎麼用安全姿勢的落地，再來教練會帶爬幾條抱石新手的 VB ~ V1 路線，就是教練示範一次後，再換體驗者爬。
                            體驗結束後的自由活動時間，如果自己有不會爬的路線，也可請教練協助示範。
                        </p>

                        <v-btn to="/ExperienceCourseSelectField" color="warning" class="w-25 ms-auto">報名去</v-btn>
                    </div>
                </v-col>
                <v-col cols="6" class="align-center">
                    <v-img width="75%" aspect-ratio="16/9" cover :src="experencecourse" class="my-auto"
                        transition="slide-x-transition"></v-img>
                </v-col>
            </v-row>
        </v-container>
    </DefaultLayout>
</template>
    
<style scoped>
.bgc {
    background: linear-gradient(90deg, #FCBD9C 40%, #FEE5D7);
}

.bgc2 {
    background: linear-gradient(90deg, #FEEDD7 0%, #FCCC8D);
}

.centered-container {
    display: flex;
    /* align-items: center; */
}

.full-width {
    width: 100%;
    margin: 0;
    padding: 0;
}
</style>