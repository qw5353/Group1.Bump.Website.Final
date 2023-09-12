<script setup>
import { ref, reactive, shallowRef } from 'vue'
import MemberInfo from './MemberInfo.vue';
import ChangePwd from './ChangePwd.vue'
import CreateAvatar from './CreateAvatar.vue'
const tab = ref(0)
const tabNames = ref([
    '個人資訊',
    '個人角色',
    '變更密碼'
])
const partialViews = shallowRef([
    {
        component: MemberInfo
    },
    {
        component: CreateAvatar
    },
    {
        component: ChangePwd
    }
])

</script>
<template>
    <v-card variant="flat">
        <v-slide-group v-model="tab" class="slide-group">
            <v-slide-group-item v-for="(tabName, i) in tabNames" :key="i" v-slot="{ isSelected, toggle }">
                <v-btn class="ma-2" :value="i" rounded variant="tonal" :color="isSelected ? 'warning' : 'grey'"
                    @click="toggle">
                    {{ tabName }}
                </v-btn>
            </v-slide-group-item>
        </v-slide-group>
        <v-window v-model="tab">
            <v-window-item v-for="(partialView, i) in partialViews" :key="i" :value="i">
                <v-container fluid>
                    <v-row>
                        <component :is="partialView.component"></component>
                    </v-row>
                </v-container>
            </v-window-item>
        </v-window>
    </v-card>
</template>

<style scoped>
.slide-group {
    justify-content: flex-end;
}
</style>