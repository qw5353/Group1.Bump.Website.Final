<script setup>
import { ref, computed } from 'vue'
import {
    ElForm, ElFormItem, ElSelect, ElOption,
    ElCol, ElSwitch, ElRadioGroup, ElRadio, ElColorPicker, ElSlider
} from 'element-plus'
import { createAvatar, schema } from '@dicebear/core';
import {
    miniavs, pixelArt, pixelArtNeutral, micah
} from '@dicebear/collection';
import { userStateStore } from '@/stores/UserStateStore.js'

const stateStore = userStateStore();

const cols = computed(() => {
    return [3, 9]
})

const avatarStyles = [
    miniavs,
    pixelArt,
    pixelArtNeutral
];

const miniavsOptions = {
    ...schema.properties,
    ...miniavs.schema.properties,
};

const pixelArtOptions = {
    ...schema.properties,
    ...pixelArt.schema.properties,
};

const pixelArtNeutralOptions = {
    ...schema.properties,
    ...pixelArtNeutral.schema.properties,
};

// console.log(miniavsOptions, pixelArtOptions, pixelArtNeutralOptions);

const styleOptions = ref({
    "seed": {
        "type": "string"
    },
    "flip": {
        "type": "boolean",
        "default": false
    },
    "rotate": {
        "type": "integer",
        "minimum": 0,
        "maximum": 360,
        "default": 0
    },
    "hair": {
        "type": "array",
        "items": {
            "type": "string",
            "enum": [
                "balndess",
                "slaughter",
                "ponyTail",
                "long",
                "curly",
                "stylish",
                "elvis",
                "classic02",
                "classic01"
            ]
        },
        "default": [
            "balndess",
            "slaughter",
            "ponyTail",
            "long",
            "curly",
            "stylish",
            "elvis",
            "classic02",
            "classic01"
        ]
    },
    "backgroundColor": {
        "type": "array",
        "items": {
            "type": "string",
            "pattern": "^#(?:[0-9a-fA-F]{3}){1,2}$"
        },
        "default": [
            "#ffdbac",
            "#f5cfa0",
            "#eac393",
            "#e0b687",
            "#cb9e6e",
            "#b68655",
            "#a26d3d",
            "#8d5524"
        ]
    },
    "eyes": {
        "type": "array",
        "items": {
            "type": "string",
            "enum": [
                "variant12",
                "variant11",
                "variant10",
                "variant09",
                "variant08",
                "variant07",
                "variant06",
                "variant05",
                "variant04",
                "variant03",
                "variant02",
                "variant01"
            ]
        },
        "default": [
            "variant12",
            "variant11",
            "variant10",
            "variant09",
            "variant08",
            "variant07",
            "variant06",
            "variant05",
            "variant04",
            "variant03",
            "variant02",
            "variant01"
        ]
    },
    "eyesColor": {
        "type": "array",
        "items": {
            "type": "string",
            "pattern": "^(transparent|[a-fA-F0-9]{6})$"
        },
        "default": [
            "76778b",
            "697b94",
            "647b90",
            "5b7c8b",
            "588387",
            "876658"
        ]
    },
    "glasses": {
        "type": "array",
        "items": {
            "type": "string",
            "enum": [
                "light07",
                "light06",
                "light05",
                "light04",
                "light03",
                "light02",
                "light01",
                "dark07",
                "dark06",
                "dark05",
                "dark04",
                "dark03",
                "dark02",
                "dark01"
            ]
        },
        "default": [
            "light07",
            "light06",
            "light05",
            "light04",
            "light03",
            "light02",
            "light01",
            "dark07",
            "dark06",
            "dark05",
            "dark04",
            "dark03",
            "dark02",
            "dark01"
        ]
    },
    "glassesColor": {
        "type": "array",
        "items": {
            "type": "string",
            "pattern": "^(transparent|[a-fA-F0-9]{6})$"
        },
        "default": [
            "4b4b4b",
            "323232",
            "191919",
            "43677d",
            "5f705c",
            "a04b5d"
        ]
    },
    "glassesProbability": {
        "type": "integer",
        "minimum": 0,
        "maximum": 100,
        "default": 10
    },
    "mouth": {
        "type": "array",
        "items": {
            "type": "string",
            "enum": [
                "sad10",
                "sad09",
                "sad08",
                "sad07",
                "sad06",
                "sad05",
                "sad04",
                "sad03",
                "sad02",
                "sad01",
                "happy13",
                "happy12",
                "happy11",
                "happy10",
                "happy09",
                "happy08",
                "happy07",
                "happy06",
                "happy05",
                "happy04",
                "happy03",
                "happy02",
                "happy01"
            ]
        },
        "default": [
            "sad10",
            "sad09",
            "sad08",
            "sad07",
            "sad06",
            "sad05",
            "sad04",
            "sad03",
            "sad02",
            "sad01",
            "happy13",
            "happy12",
            "happy11",
            "happy10",
            "happy09",
            "happy08",
            "happy07",
            "happy06",
            "happy05",
            "happy04",
            "happy03",
            "happy02",
            "happy01"
        ]
    },
    "mouthColor": {
        "type": "array",
        "items": {
            "type": "string",
            "pattern": "^(transparent|[a-fA-F0-9]{6})$"
        },
        "default": [
            "d29985",
            "c98276",
            "e35d6a",
            "de0f0d"
        ]
    }
})
const color1 = ref("#4351A0")

const avatar = ref('')
stateStore.avatar = avatar

const avatars = computed(() => {
    const theAvatars = avatarStyles.map(style => createAvatar(style, {
        seed: "Felix",
        flip: styleOptions.value.flip,
        rotate: styleOptions.value.rotate,
        hair: [styleOptions.value.hair],
        glasses: [styleOptions.value.glasses],
        backgroundColor: [color1.value.substring(1)],
        // blushes: styleOptions.value.blushes,
        // bodyColor: styleOptions.value.bodyColor,
        // mouth: styleOptions.value.mouth,
    }).toDataUriSync());

    avatar.value = theAvatars[0]
    return theAvatars
});

async function onSubmit() {
    try {
        const avatarData = await stateStore.getAvatar(stateStore.avatar)
        stateStore.updateAvatar(avatarData)
    }
    catch (err) {
        console.log(err)
    }
}

</script>

<template>
    <v-container class="h-auto">
        <v-row no-gutters>
            <v-col :cols="cols[0]">
                <v-sheet class="mt-10">
                    <v-avatar color="grey" size="200" rounded="10" v-if="avatars">
                        <!-- <v-img cover src="https://cdn.vuetifyjs.com/images/profiles/marcus.jpg"></v-img> -->
                        <!-- <v-img cover :src="avatars[0]" alt="Avatar"></v-img> -->
                        <img :src="avatars[0]" />
                    </v-avatar>
                    <!-- <div class="ms-13 mt-1 text-overline" size="50px">
                        @carolhyl
                    </div> -->
                    <v-btn class="ma-1 mx-8 mt-6" color="orange-darken-4" variant="flat" label @click="onSubmit">
                        確認變更大頭貼
                    </v-btn>
                </v-sheet>
            </v-col>
            <v-col :cols="cols[1]">
                <v-card class="mt-12" width="700px">
                    <el-form :model="styleOptions" label-width="80px">
                        <el-form-item label="頭髮">
                            <el-select v-model="styleOptions.hair" placeholder="選擇一個髮型">
                                <el-option label="balndess" value="balndess" />
                                <el-option label="slaughter" value="slaughter" />
                                <el-option label="ponyTail" value="ponyTail" />
                                <el-option label="long" value="long" />
                                <el-option label="curly" value="curly" />
                                <el-option label="stylish" value="stylish" />
                            </el-select>
                        </el-form-item>
                        <el-form-item label="眼鏡">
                            <el-radio-group v-model="styleOptions.glasses">
                                <el-radio label="null" />
                                <el-radio label="normal" />
                            </el-radio-group>
                        </el-form-item>
                        <el-form-item label="翻轉">
                            <el-col :span="11">
                                <el-switch v-model="styleOptions.flip" />
                            </el-col>
                        </el-form-item>
                        <el-form-item label="旋轉">
                            <el-col :span="11">
                                <el-slider v-model="styleOptions.rotate" max="360" />
                            </el-col>
                        </el-form-item>
                        <el-form-item label="背景顏色">
                            <el-col :span="11">
                                <el-color-picker v-model="color1" />
                            </el-col>
                        </el-form-item>
                        <!-- <el-form-item label="Activity type">
                            <el-checkbox-group v-model="styleOptions.hair">
                                <el-checkbox label="Online activities" name="type" />
                                <el-checkbox label="Promotion activities" name="type" />
                                <el-checkbox label="Offline activities" name="type" />
                                <el-checkbox label="Simple brand exposure" name="type" />
                            </el-checkbox-group>
                        </el-form-item> -->
                    </el-form>
                </v-card>
            </v-col>
        </v-row>
    </v-container>
</template>