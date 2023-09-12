<script setup>

</script>

<template>
    <v-row>
        <span class="mx-auto system-msg-text-color text-subtitle-2 mb-3">
            <slot></slot>
        </span>
    </v-row>
</template>

<style scoped>
.system-msg-text-color {
    color: gray;
    opacity: 0.5;
}
</style>