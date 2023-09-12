<script setup>
import { ref, onMounted, defineProps } from 'vue';
import axios from 'axios';

const props = defineProps(['id'])
const newProductDisplay = ref(false);

onMounted(async () => {
    try {
        const productId = props.id
        const res = await axios.get(`/api/Shop/IsNewProduct?Id=${productId}`);
        newProductDisplay.value = res.data;
    }
    catch (err) {
        console.error(err);
    }
})


</script>
<template>
    <div class="container" v-show="newProductDisplay">
        <div>
            <div class="rounded-pill newBadgeStyle d-flex justify-center"><span>new</span></div>
        </div>
    </div>
</template>
<style scoped>
.container {
    position: relative;
    top: 3px;
    right: 6px;
    z-index: 999;
}

.newBadgeStyle {
    background-color: #FFB23E;
    color: white;
    height: 18px;
    width: 40px;
    font-size: 0.5em;
    font-weight: bold;
    letter-spacing: 1px
}
</style>