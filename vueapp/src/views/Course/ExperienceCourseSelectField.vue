<script setup>
import { ref } from 'vue';
import FieldCard from '../../components/Course/ExpFieldCard.vue';
import DefaultLayout from '../../components/layouts/DefaultLayout.vue';
import { useTitle } from '@vueuse/core';

useTitle('Bump - 技巧課程');


const fields = ref([])
const lordFields = async () => {
    const response = await fetch("/api/Fields")
    const datas = await response.json();

    fields.value = datas;
    console.log(fields.value);
}
lordFields();
const skillpage1 = ref(new URL('/src/assets/images/course-index-carousel/skillpage1.jpg', import.meta.url).href);
const skillpage2 = ref(new URL('/src/assets/images/course-index-carousel/skillpage2.jpg', import.meta.url).href);
const skillpage3 = ref(new URL('/src/assets/images/course-index-carousel/skillpage3.jpg', import.meta.url).href);
const title = ref(new URL('/src/assets/images/course-index-carousel/title.jpg', import.meta.url).href);
</script>


<template>
    <DefaultLayout>
        <v-container class="mw-xl">
            <v-row no-gutters>
                <v-col cols="12" class="d-flex justify-center">
                    <v-img width="75%" aspect-ratio="16/9" cover :src="title" transition="slide-x-transition"></v-img>
                </v-col>
            </v-row>
            <v-row no-gutters>
                <v-col cols="4"><v-img width="100%" aspect-ratio="16/9" cover :src=skillpage1></v-img></v-col>
                <v-col cols="4"><v-img height="100%" aspect-ratio="16/9" cover :src=skillpage2></v-img></v-col>
                <v-col cols="4"><v-img width="100%" aspect-ratio="16/9" cover :src=skillpage3></v-img></v-col>
            </v-row>
            <v-row class="d-flex justify-center">
                <h2>選擇想去的場館</h2>
            </v-row>
            <p>合作夥伴:</p>
            <v-row>
                <v-col cols="3" v-for="field in fields" :key="field.id">
                    <FieldCard :title="field.name" :subtitle="field.address" :image="field.thumbnail"
                        :description="field.shortDescription" :id="field.id"></FieldCard>
                </v-col>
            </v-row>
        </v-container>
    </DefaultLayout>
</template>
    
<style scoped>
.text-center {
    text-align: center;
}

.text-end {
    text-align: end;
}

.mw-xl {
    max-width: 1440px;
}
</style>