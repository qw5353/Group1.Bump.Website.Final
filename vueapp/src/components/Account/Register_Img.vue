<script setup>
import { watch, ref } from 'vue';
import { createAvatar, schema } from '@dicebear/core';
import {
    micah, lorelei, bigEarsNeutral, adventurerNeutral, avataaarsNeutral, bigSmile,
    adventurer, loreleiNeutral, notionistsNeutral, pixelArt, pixelArtNeutral
} from '@dicebear/collection';
import { storeToRefs } from 'pinia';
import { useRegisterStore } from '@/stores/registerStore.js'

const store = useRegisterStore()
const datas = store.stepsData
const { thumbnail } = storeToRefs(useRegisterStore())
// const options = {
//     ...schema.properties,
//     ...micah.schema.properties,
// };

// console.log(options);

const avatarStyles = [
    bigEarsNeutral,
    adventurerNeutral,
    avataaarsNeutral,
    adventurer,
    loreleiNeutral,
    notionistsNeutral,
    pixelArt,
    pixelArtNeutral,
    bigSmile
];

const avatars = avatarStyles.map(style => createAvatar(style, {
    size: 128,
}).toDataUriSync())

const selectedSlide = ref(0)
datas.thumbnail = avatars[0]

const handleSlideChange = () => {
    datas.thumbnail = avatars[selectedSlide.value];
    // console.log(selectedAvatar.value);
};

watch(selectedSlide, handleSlideChange, () => {
    handleSlideChange()
});


</script>

<template>
    <v-carousel height="400" hide-delimiters progress="warning" v-model="selectedSlide">
        <v-carousel-item v-for="(avatar, i) in avatars" :key="i">
            <v-sheet height="100%">
                <div class="d-flex fill-height justify-center align-center">
                    <div class="text-h2">
                        <img :src="avatar" alt="Avatar" />
                    </div>
                </div>
            </v-sheet>
        </v-carousel-item>
    </v-carousel>
</template>

