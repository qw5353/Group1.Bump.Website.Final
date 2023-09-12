<script setup>
import { useRouter } from 'vue-router';
import { ref } from 'vue';
const NotFound = ref(new URL('/src/assets/images/404notfound/404notfound.jpg', import.meta.url).href);
const router = useRouter();
function backToPrev() {
    router.back()
}
</script>

<template>
    <!-- <h1>Page not found!</h1> -->
    <v-container>
        <v-row>
            <v-col>
                <v-img width="100%" aspect-ratio="16/9" cover :src="NotFound" transition="slide-x-transition"></v-img>
            </v-col>
        </v-row>
        <v-row>
            <v-col class="d-flex justify-center align-center">
                <v-btn class="bg-primary" @click="backToPrev()">返回前一頁</v-btn>
            </v-col>
        </v-row>
    </v-container>
</template>

<style scoped></style>
