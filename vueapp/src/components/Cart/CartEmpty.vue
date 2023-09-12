<script setup>
import { ref } from 'vue';
import { mdiAlertCircleOutline } from '@mdi/js';
</script>

<template>
    <div class="hight">
        <v-row class="d-flex align-center ">
            <v-icon :icon="mdiAlertCircleOutline" class="icon grey"></v-icon>
            <p class="text-h4 ms-6 font-weight-bold grey">
                您的購物車是空的
            </p>
        </v-row>
    </div>
</template>
  
<style scoped>
.icon {
    height: 80px;
    width: 80px;
}

.hight {
    height: 400px;
    display: flex;
    justify-content: center;
    /* 水平居中 */
    align-items: center;
}

.grey {
    color: #E0E0E0;
}
</style>