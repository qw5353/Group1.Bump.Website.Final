<script setup>
import { mdiChevronUp } from '@mdi/js';
function scrollToTop() {
    setTimeout(() => {
        window.scrollTo({
            top: 0,
            behavior: "smooth",
        });
    }, 190);
}

</script>
<template>
    <button class="custom-btn" @click="scrollToTop">
        <v-icon class="custom-icon" :icon="mdiChevronUp" size="x-large"></v-icon>
    </button>
</template>
    
<style scoped>
.custom-btn {
    background-color: #fcc239;
    border-radius: 100%;
    height: 50px;
    width: 50px;
    position: fixed;
    bottom: 20px;
    right: 40px;
}

.custom-btn:hover {
    opacity: 0.5;
}

.custom-icon {
    color: white;
}
</style>