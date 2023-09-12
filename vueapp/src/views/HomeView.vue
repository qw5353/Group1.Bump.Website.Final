<script setup>
import { useTitle } from '@vueuse/core';
import DefaultLayout from '../components/layouts/DefaultLayout.vue';
import FirstCarousel from '../components/homepage/FirstCarousel.vue';
import Chat from '@/components/FAQ/Chat.vue';
import SecondCarousel from '../components/homepage/SecondCarousel.vue';
import NewProductGroup from '../components/homepage/NewProducts.vue';
useTitle('Bump');
</script>

<template>
    <DefaultLayout>
        <main>
            <FirstCarousel class="my-12"></FirstCarousel>
            <SecondCarousel class="my-12"></SecondCarousel>
            <NewProductGroup class="my-12"></NewProductGroup>
        </main>
    </DefaultLayout>
    <Chat />
</template>

