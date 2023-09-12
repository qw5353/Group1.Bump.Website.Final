<script setup>
import DefaultHeader from './DefaultHeader.vue';
import DefaultFooter from './DefaultFooter.vue';
</script>

<template>
    <DefaultHeader></DefaultHeader>
    <slot></slot>
    <DefaultFooter></DefaultFooter>
</template>