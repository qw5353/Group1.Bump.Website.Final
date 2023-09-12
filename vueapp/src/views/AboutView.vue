<script setup>

import { useTitle } from '@vueuse/core';
import DefaultLayout from '@/components/layouts/DefaultLayout.vue';
import { ref } from 'vue';

useTitle('Bump - 關於我們');

const teamWork = ref(new URL('/src/assets/images/about/全組分工.png', import.meta.url).href);
const thankYou = ref(new URL('/src/assets/images/about/THANKYOU.png', import.meta.url).href);

</script>

<template>
    <DefaultLayout>
        <main>
            <v-container style="max-width: 1200px;" class="about-container">
                <v-row>
                    <v-col cols="12">
                        <v-img src="https://qlife.com/wp-content/uploads/2021/06/best-time-climb-banner.jpg" alt="Image"
                            width="100%" class="mb-4" cover>
                            <template v-slot:placeholder>
                                <div class="d-flex align-center justify-center fill-height">
                                    <v-progress-circular color="grey-lighten-4" indeterminate></v-progress-circular>
                                </div>
                            </template>
                        </v-img>
                    </v-col>
                </v-row>
                <v-row>
                    <v-col cols="12">
                        <h2 class="headline">Bump的故事</h2>
                        <p>
                            在遙遠的2010年代初期，一位充滿熱情和對戶外活動的熱愛者，王偉仲創立了一家名為<span class="font-weight-bold"> Bump
                            </span>的小型戶外用品專賣店。當時，戶外活動在這個地區的知名度並不高，市場上也缺乏供應國外戶外品牌商品的店鋪。王偉仲深深感受到了戶外愛好者的需求，並決定以自己的熱情為起點，為這個社群打造一個獨特的聚集地。
                        </p>

                        <p>
                            於是，在2011年，<span class="font-weight-bold"> Bump
                            </span>戶外用品專賣正式成立。王偉仲在城市的一處角落租下了一個小店面，開始了他們的奮鬥之旅。最初的<span class="font-weight-bold"> Bump
                            </span>只是一家毫不起眼的戶外用品店，擁有有限的庫存和資源，但這並沒有阻礙王偉仲的決心和熱情。
                        </p>

                        <p>
                            與此同時，網際網路也開始崭露頭角，成為人們交流資訊和經驗的平台。王偉仲深切地明白到，戶外社群需要一個地方來分享技巧、行程和經驗。於是，他們在2012年推出了<span
                                class="font-weight-bold"> Bump Climbing
                            </span>，這是一個網站，提供戶外愛好者分享他們的冒險和經驗的平台。這個舉動不僅豐富了戶外社群的內容，也讓<span class="font-weight-bold"> Bump
                            </span>逐漸成為戶外愛好者交流的中心。
                        </p>

                        <p>
                            隨著時間的推移，<span class="font-weight-bold"> Bump
                            </span>不斷地吸引著更多的戶外愛好者。他們不僅提供國外優質品牌的商品，還舉辦各種戶外活動、專業訓練和講座，幫助愛好者提升技能和知識。無論是攀登、徒步、露營還是探險，每個人都能在<span
                                class="font-weight-bold"> Bump </span>找到合適的裝備和支援。
                        </p>

                        <p>
                            <span class="font-weight-bold"> Bump
                            </span>的成長不僅僅是一個商業成功的故事，更是一個充滿熱情、奮鬥和社群連結的歷程。他們的使命是為戶外愛好者提供最優質的產品，同時建立一個讓愛好者能夠互相學習、分享和成長的平台。無論是初次嘗試的新手，還是經驗豐富的戶外高手，每個人都能在<span
                                class="font-weight-bold"> Bump </span>找到一個屬於自己的家。
                        </p>
                    </v-col>
                </v-row>
                <v-row class="mt-8 mb-10">
                    <v-col cols="12">
                        <h2 class="headline">Bump團隊介紹</h2>
                        <v-img :src="teamWork" alt="Image" width="100%" class="mt-8" cover>
                        </v-img>
                    </v-col>
                </v-row>
                <v-row class="mt-10">
                    <v-col cols="12">
                        <v-img :src="thankYou" alt="Image" width="100%" class="mt-8" cover>
                        </v-img>
                    </v-col>
                </v-row>
            </v-container>
        </main>
    </DefaultLayout>
</template>


<style scoped>
p {
    margin-top: 24px;
    margin-bottom: 24px;
}

.about-container {
    min-height: calc(100vh - 66px);
}
</style>