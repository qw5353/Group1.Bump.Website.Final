<template>
    <div class="text-center">
        <v-menu open-on-hover>
            <template v-slot:activator="{ props }">
                <v-btn href="/Login" icon class="ml-auto" v-bind="props">
                    <v-icon :icon="mdiAccountOutline" color="white" size="large"></v-icon>
                </v-btn>
            </template>

            <v-card class="mx-auto pa-2 rounded-xl rounded-ts-0" max-width="300">
                <v-list>
                    <v-btn color="success" class="mt-4" to="/Login" block>
                        登入
                    </v-btn>
                    <v-list-subheader>會員專區</v-list-subheader>

                    <v-list-item v-for="(item, i) in items" :key="i" type="btn" :value="item" color="warning" rounded="xl"
                        :to="item.uri">
                        <template v-slot:prepend>
                            <v-icon :icon="item.icon"></v-icon>
                        </template>
                        <v-list-item-title v-text="item.text"></v-list-item-title>
                    </v-list-item>
                </v-list>
            </v-card>
        </v-menu>
    </div>
</template>

<script setup>
import { mdiClipboardList, mdiAccount, mdiHandball, mdiTicketPercent, mdiAccountOutline } from '@mdi/js';
import { ref } from 'vue';
const items = ref([
    { text: '會員中心', icon: mdiAccount, uri: '/Member/Index' },
    { text: '訂單中心', icon: mdiClipboardList, uri: '/Member/Orders' },
    { text: '課程中心', icon: mdiHandball, uri: '/Member/Courses' },
    { text: '優惠券匣', icon: mdiTicketPercent, uri: '/Member/Coupons' }
])
</script>
