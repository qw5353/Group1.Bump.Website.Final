<script setup>
import { ref } from 'vue';

const firstCarousel = ref([
    {
        src: new URL('/src/assets/images/shop/index-first-carousel/slider1.jpg', import.meta.url).href,
        link: 'https://www.youtube.com/watch?v=M3etkEoTH54&ab_channel=TransChance%E5%AF%8C%E8%81%96%E5%9C%8B%E9%9A%9B'
    },
    {
        src: new URL('/src/assets/images/shop/index-first-carousel/slider2.jpg', import.meta.url).href,
    },
    {
        src: new URL('/src/assets/images/shop/index-first-carousel/slider3.jpg', import.meta.url).href,
    },
    {
        src: new URL('/src/assets/images/shop/index-first-carousel/slider4.jpg', import.meta.url).href,
    },
    {
        src: new URL('/src/assets/images/shop/index-first-carousel/slider5.jpg', import.meta.url).href,
    }
]);


</script>

<template>
    <v-container fluid class="px-0 py-0">
        <v-carousel height="80vh" hide-delimiter-background cycle interval="3000" class="first-carousel">
            <v-carousel-item v-for="item in firstCarousel " :key="item.src">
                <a v-if="item.link" :href="item.link" target="__blank">
                    <v-img class="h-100" aspect-ratio="16/9" cover :src="item.src"></v-img>
                </a>
                <v-img v-else class="h-100" aspect-ratio="16/9" cover :src="item.src"></v-img>
            </v-carousel-item>
        </v-carousel>
    </v-container>
</template>

<style  scoped></style>