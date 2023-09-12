<script setup>
import { ref, onMounted } from 'vue';
import axios from 'axios';

const activities = ref([]);
const modalIndex = ref(0);

onMounted(async () => {
    try {
        const res = await axios.get(`/api/SkillCourses`);
        activities.value = res.data;
    } catch (err) {
        console.error(err);
    }
})
</script>

<template>
    <v-container fluid class="px-0">
        <v-carousel height="80vh" hide-delimiter-background cycle interval="3000" v-model="modalIndex">
            <v-carousel-item v-for="(item, index) in activities" :key="item.id" :value="index">
                <a href="/course">
                    <v-img :src="`/files/images/SkillCourses/${item.thumbnail}`" cover eager></v-img>
                    {{ item.name }}
                </a>
            </v-carousel-item>
        </v-carousel>
    </v-container>
</template>
<style scoped></style>