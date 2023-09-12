<script setup>
import { ref, onMounted, defineProps } from 'vue';
import axios from 'axios';

const props = defineProps(['id'])
const id = props.id;
const isOnSold = ref(false)
onMounted(async () => {
    try {
        if (id > 0) {
            const res = await axios.get(`/api/Shop/ProductStyles?id=${id}`);
            isOnSold.value = res.data.every(item => item.inventory === 0);
        }
    }
    catch (err) {
        console.error(err);
    }
})
</script>

<template>
    <div v-if="isOnSold" class="container align-self-end justify-self-center">
        <div class="d-flex justify-center align-center h-100">
            <h4 style="color:white">已完售</h4>
        </div>
    </div>
</template>
<style scoped>
.container {
    background-color: rgb(255, 178, 62, 0.9) !important;
    height: 30px;
    width: 100%;
    position: absolute;
}
</style>