<script setup>
import { ref } from 'vue';
const firstCarousel = ref([
    new URL('/src/assets/images/first-home-carousel/slider1.jpg', import.meta.url).href,
    new URL('/src/assets/images/first-home-carousel/slider2.jpg', import.meta.url).href
]);
</script>

<template>
    <v-container fluid class="px-0">
        <v-carousel height="80vh" hide-delimiter-background cycle interval="3000">
            <v-carousel-item v-for="link in firstCarousel" :key="link" :src="link" cover></v-carousel-item>
        </v-carousel>
    </v-container>
</template>