<script setup>
import { defineProps } from 'vue';
defineProps(["items"]);

</script>
<template>
    <v-breadcrumbs :items="items" class="text-subtitle-2">
        <template v-slot:title="{ item }">
            {{ item.title }}
        </template>
    </v-breadcrumbs>
</template>
    
<style scoped></style>