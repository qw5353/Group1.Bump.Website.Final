<script setup>
import { defineProps, ref } from 'vue';
import { mdiChevronUp, mdiChevronDown } from '@mdi/js';
defineProps(['title', 'subtitle', 'image', 'description', 'id'])
const show = ref(false);
</script>

<template>
    <v-card class="mx-auto">

        <v-img :src="`/files/images/fields/${image}`" height="200px" cover></v-img>

        <v-card-title>
            {{ title }}
        </v-card-title>

        <v-card-subtitle>
            {{ subtitle }}
        </v-card-subtitle>

        <v-card-actions>
            <v-btn color="orange-lighten-2" variant="text" :to="`/ExperienceCourseOrder/${id}`">
                點我填報名表
            </v-btn>

            <v-spacer></v-spacer>

            <v-btn :icon="show ? mdiChevronUp : mdiChevronDown" @click="show = !show"></v-btn>
        </v-card-actions>

        <v-expand-transition>
            <div v-show="show">
                <v-divider></v-divider>

                <v-card-text>
                    {{ description }}
                </v-card-text>
            </div>
        </v-expand-transition>
    </v-card>
</template>
