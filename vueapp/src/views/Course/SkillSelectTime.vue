<script setup>
import { ref, onMounted, computed } from 'vue';
import FieldTime from '../../components/Course/FieldTime.vue';
import DefaultLayout from '../../components/layouts/DefaultLayout.vue';
import { useTitle } from '@vueuse/core';
import { useRoute } from 'vue-router'
useTitle('Bump - 岩館A課表');
const field1 = ref(new URL('/src/assets/images/course-index-carousel/field1.jpg', import.meta.url).href);
const route = useRoute()
const fieldData = ref([])
const dataRef2 = computed(() => {
    if (!fieldData.value) return [];

    return fieldData["_rawValue"].map(dayTodo => {
        return {
            day: dayTodo.startDate.replaceAll('/', '-'),
            todos: [
                {
                    id: dayTodo.id,
                    date: dayTodo.startDate,
                    task: dayTodo.name
                }
            ]
        };
    });
})

onMounted(async () => {
    try {
        const res = await fetch(`/api/Skillcurriculums?fieldId=${route.params.id}`)
        fieldData.value = await res.json();
        console.log(JSON.stringify(fieldData));
        const firstData = fieldData.value[0];

        if (firstData) {
            firstFieldName.value = firstData.fieldName; // 假設 name 是你想要存取的參數
        }

    } catch (err) {
        console.log(err);
    }
})
const firstFieldName = ref('');

</script>

<template>
    <DefaultLayout>
        <v-container>
            <v-row>
                <v-col>
                    <v-img width="100%" height="500px" aspect-ratio="16/9" cover :src=field1></v-img>
                </v-col>
            </v-row>
            <v-container>
                <v-row>
                    <v-col class="text-center">
                        <h2>{{ firstFieldName }}課表</h2>
                    </v-col>
                </v-row>
            </v-container>
        </v-container>
        <v-container>
            <FieldTime :dataRef2="dataRef2"></FieldTime>
        </v-container>
        
    </DefaultLayout>
</template>

<style scoped></style>