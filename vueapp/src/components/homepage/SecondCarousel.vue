<script setup>
import { ref, onMounted } from 'vue';
import axios from 'axios';

const activities = ref([]);
const modalIndex = ref(0);

onMounted(async () => {
    try {
        await initialize();
    } catch (err) {
        console.error(err);
    }
})

async function initialize() {
    const res = await axios.get(`/api/OnActivities`);
    activities.value = res.data;
    activities.value = activities.value.map(item => {
        return {
            ...item,
            thumbnail: '/files/images/second-home-carousel/' + item.thumbnail
        }
    });

}

</script>

<template>
    <v-container fluid class="px-0">
        <v-carousel height="80vh" hide-delimiter-background cycle interval="3000" v-model="modalIndex">
            <v-carousel-item v-for="(item, index) in activities" :key="item.id" :value="index">
                <a :href="`/activity/${item.id}`"><v-img :src="item.thumbnail" cover eager></v-img></a>
            </v-carousel-item>
        </v-carousel>
    </v-container>
</template>
<style scoped></style>