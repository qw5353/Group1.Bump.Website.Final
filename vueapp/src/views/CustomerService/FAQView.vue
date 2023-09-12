<script setup>
import DefaultLayout from '@/components/layouts/DefaultLayout.vue';
import Questions from '@/components/FAQ/Questions.vue';
import { ref, onMounted, watchEffect } from 'vue';
import { useTitle } from '@vueuse/core';
import axios from 'axios';

const tab = ref(1);
const types = ref([]);
const questions = ref([]);

useTitle("Bump - 常見問題");

onMounted(async () => {
    try {
        const res = await axios.get('/api/FAQ/Types');
        types.value = res.data;
    } catch (err) {
        console.error(err);
    }
})

watchEffect(async () => {
    try {
        const res = await axios.get(`/api/FAQ?typeId=${tab.value}`);
        questions.value = res.data;
    } catch (err) {
        console.error(err);
    }
});
</script>

<template>
    <DefaultLayout>
        <main>
            <v-container class="faq-container pt-10">
                <v-card class="mx-auto h-100">
                    <h1 class="ms-4 mb-2">常見問題</h1>
                    <hr class="mb-3">
                    <div class="d-flex flex-row">
                        <v-tabs v-model="tab" direction="vertical">
                            <v-tab v-for="t in types" :key="t.typeId" :value="t.typeId" class="tab-text text-h6">{{ t.name
                            }}</v-tab>
                        </v-tabs>
                        <v-window v-model="tab" class="w-100 h-100">
                            <v-window-item v-for="(n, index) in types" :key="index" :value="tab">
                                <v-container fluid class="w-100 h-75">
                                    <Questions :questions="questions"></Questions>
                                </v-container>
                            </v-window-item>
                        </v-window>
                    </div>
                </v-card>
            </v-container>
        </main>
    </DefaultLayout>
</template>

<style scoped>
.faq-container {
    height: calc(100vh - 66px);
    max-width: 1440px;
}

.tab-text {
    letter-spacing: 0.125rem !important;
}

.faq-toolbar-title {
    line-height: 2rem;
}

.overflow-y-auto {
    overflow-y: auto;
}
</style>