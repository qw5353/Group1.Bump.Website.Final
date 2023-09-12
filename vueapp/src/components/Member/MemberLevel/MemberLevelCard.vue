<script setup>
import { ref, onMounted, defineProps, computed } from 'vue'
import { mdiArrowRightThin, mdiClockTimeEight } from '@mdi/js';

const props = defineProps({
    card: Object
})

const formatPointRate = computed(() => {
    const rate = props.card.gainPointRate * 100;
    return rate.toFixed(0) + '%'
})

const formatTimePeriod = computed(() => {
    const time = props.card.timePeriod;
    if (time === 0) return '無期限限制'
    else return '1年'
})

function getCardColor(variant) {
    if (variant === 'amber-darken-4') {
        return 'amber-darken-4';
    } else {
        return 'amber-accent-4';
    }
};

const getVariant = computed(() => {
    const variant = props.card.color
    if (variant === 'amber-darken-4') {
        return 'elevated';
    } else {
        return 'flat';
    }
})

</script>
<template>
    <v-card :key="card.memberCardId" class="mx-auto" max-width="344" :color="getCardColor(props.card.color)"
        :variant=getVariant>
        <v-card-text>
            <v-chip class="ma-2" color="white"> {{ card.levelName }}</v-chip>
            <p class="text-h4 text--primary">Lv.{{ card.levelOrder }} {{ card.name
            }}</p>
            <v-row class="mt-2">
                <v-col cols="auto" class="align-self-end">
                    <p>會員點數回饋:</p>
                </v-col>
                <v-col>
                    <p class="text-h3">{{ formatPointRate }}</p>
                </v-col>
            </v-row>
        </v-card-text>
        <div v-if="card.startDate" class="d-flex py-1 justify-space-between">
            <v-list-item density="compact" :prepend-icon="mdiClockTimeEight">
                <v-list-item-subtitle>{{ card.startDate }}</v-list-item-subtitle>
            </v-list-item>
            <v-list-item density="compact" :prepend-icon="mdiArrowRightThin">
                <v-list-item-subtitle>{{ card.endDate }}</v-list-item-subtitle>
            </v-list-item>
        </div>
        <div v-else>
            <v-list-item density="compact" :prepend-icon="mdiClockTimeEight">
                <v-list-item-subtitle>會員期限: {{ formatTimePeriod }}</v-list-item-subtitle>
            </v-list-item>
        </div>
    </v-card>
</template>
    
<style></style>