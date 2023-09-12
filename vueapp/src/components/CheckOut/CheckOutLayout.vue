<script setup>
import { ref, computed } from 'vue';
import CheckOutCard from './CheckOutCard.vue';

</script>

<template>
    <v-container fluid class="p-0">
        <div class="content-container" id="checkout-container">
            <CheckOutCard></CheckOutCard>
        </div>
    </v-container>
</template>
  
<style scoped>
.content-container {
    width: 1000px;
    margin: 0 auto;
}
</style>