<script setup>
import { ref, onMounted, computed } from 'vue';
import axios from 'axios';
import { mdiMenuRight } from '@mdi/js';
import { mdiMenuLeft } from '@mdi/js';

const slides = ref([]);
const activities = ref([]);
const pageLink = decodeURIComponent(window.location.href);
const onSaleLink = 'https://localhost:5002/Shop/OnSale';
const filteredSlide = ref([]);
const currentIndex = ref(0);

onMounted(async () => {
    const res = await axios.get('/api/ActivityDiscounts/ActUsedProductTags');
    activities.value = res.data;
    activities.value.map(item => {
        slides.value.push({
            name: item.name,
            startTime: formatDateTime(item.startTime),
            endTime: formatDateTime(item.endTime),
            description: item.description,
            priceThreshold: item.activityDiscounts[0].priceThreshold,
            link: 'https://localhost:5002/Shop/OnSale?actName=' + item.activityDiscounts[0].productTags[0].name
        });
    });
    filteredSlide.value = slides.value.filter(slide => slide.link === pageLink);
    console.log(slides.value[2].link, pageLink)
})

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
    <div>
        <v-carousel v-model="currentIndex" class="mt-5 rounded-card" height="140" cycle hide-delimiters
            hide-delimiter-background>
            <template v-slot:prev="{ props }">
                <v-btn v-if="filteredSlide.length !== 1" :icon="mdiMenuLeft" size="small" variant="text"
                    @click="props.onClick"></v-btn>
            </template>
            <template v-slot:next="{ props }">
                <v-btn v-if="filteredSlide.length !== 1" :icon="mdiMenuRight" class="text-none" size="small" variant="text"
                    @click="props.onClick"></v-btn>
            </template>
            <v-carousel-item v-if="pageLink == onSaleLink" v-for="(slide, i) in slides" :key="i" :value="i">
                <v-sheet height="100%">
                    <v-hover>
                        <template v-slot:default="{ isHovering, props }">
                            <a :href="slide.link">
                                <v-card class="fill-height bg-grey-lighten-4" v-bind="props"
                                    :color="isHovering ? 'grey-lighten-1' : undefined">
                                    <v-card-text class="text-center">
                                        <p class="text-h6 font-weight-black">
                                            {{ slide.name }}
                                        </p>
                                        <p class="text-subtitle-1">{{ slide.startTime }} - {{ slide.endTime }}</p>
                                        <p class="text-subtitle-1">
                                            {{ slide.description }}
                                        </p>
                                    </v-card-text>
                                </v-card>
                            </a>
                        </template>
                    </v-hover>
                </v-sheet>
            </v-carousel-item>
            <v-carousel-item v-for="(slide, j) in filteredSlide" :key="j" :value="j">
                <v-sheet height="100%">
                    <v-hover>
                        <template v-slot:default="{ isHovering, props }">
                            <v-card class="fill-height bg-grey-lighten-4" v-bind="props"
                                :color="isHovering ? 'grey-lighten-1' : undefined">
                                <v-card-text class="text-center">
                                    <p class="text-h6 font-weight-black">
                                        {{ slide.name }}
                                    </p>
                                    <p class="text-subtitle-1">{{ slide.startTime }} - {{ slide.endTime }}</p>
                                    <p class="text-subtitle-1">
                                        {{ slide.description }}
                                    </p>
                                </v-card-text>
                            </v-card>
                        </template>
                    </v-hover>
                </v-sheet>
            </v-carousel-item>
        </v-carousel>
    </div>
</template>
<style scoped>
.rounded-card {
    border-radius: 10px;
}

a {
    text-decoration: none;
}

a:link {
    color: black;
}

a:visited {
    color: black;
}

a:hover {
    color: rgb(165, 165, 165);
}
</style>