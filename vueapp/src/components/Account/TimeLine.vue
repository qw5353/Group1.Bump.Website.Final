<template>
    <v-timeline direction="horizontal">
        <v-timeline-item v-for="(item, index) in timelineItems" :key="index" :dot-color="getDotColor(index)" size="small">
            <template v-slot:opposite> <strong>{{ item.label }}</strong></template>
        </v-timeline-item>
    </v-timeline>
</template>

<script setup>
import { ref, computed } from 'vue';

const props = defineProps({
    dotColor: String,
    currentStep: Number
});

const timelineItems = ref([
    { label: "創建帳號" },
    { label: "上傳大頭貼" },
    { label: "個人資訊" }
]);

const getDotColor = (stepIndex) => {
    if (props.currentStep === 3) {
        return props.dotColor
    } else if (stepIndex <= props.currentStep) {
        return props.dotColor;
    } else {
        return "grey";
    }
};
</script>