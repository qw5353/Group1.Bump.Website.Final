<script setup>
import CheckOutLayout from '../../components/CheckOut/CheckOutLayout.vue';
import DefaultLayout from '../../components/layouts/DefaultLayout.vue';
import { useTitle } from '@vueuse/core';

useTitle('Bump - 結帳');

</script>

<template>
    <v-container fluid class="h-screen pa-0 ma-0">
        <DefaultLayout>
            <main>
                <CheckOutLayout></CheckOutLayout>
            </main>
        </DefaultLayout>
    </v-container>
</template>

<style scoped>
main {
    min-height: calc(100% - 66px - 182px);
}
</style>