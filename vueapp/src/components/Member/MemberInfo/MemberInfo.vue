<script setup>
import { ref, computed, onMounted, nextTick } from 'vue'
import { userStateStore } from '@/stores/UserStateStore';
import MemberInfoForm from './MemberInfoForm.vue';
import axios from 'axios';

const userStore = userStateStore()

const cols = computed(() => {
    return [3, 9]
})

const loading = ref(false)

function load() {
    loading.value = true
    setTimeout(() =>
        (loading.value = false), 3000)
}
const dialog = ref(false)

const memberInfo = ref({
    "id": 0,
    "account": "",
    "memberLevelName": "",
    "thumbnail": "",
    "name": "",
    "nickname": "",
    "email": "",
    "gender": "",
    "birthday": "",
    "phoneNumber": "",
    "points": 0,
    "dmSubscribe": false,
    "createdAt": "",
    "lastModified": "",
    "isConfirm": true,
    "isBanned": false
})

const editInfo = ref()

function memberEditedInfo(updated) {
    editInfo.value = updated
}

async function getMemberInfo() {
    try {
        await axios.get('/api/Member')
            .then(res => memberInfo.value = res.data)
    } catch (err) {
        console.log(err)
    }
}

async function submitEdit() {
    try {
        const response = await axios.post('/api/Member/EditProfile', editInfo.value);
        const result = response.data;
        if (result == '修改成功!') {
            // 合併更新到 memberInfo 物件中
            memberInfo.value = { ...memberInfo.value, ...editInfo.value };
        }
        dialog.value = false;
        await nextTick();
    } catch (err) {
        console.log(err.response)
    }
}

function formatDate(dateString) {
    const date = new Date(dateString);
    return `${date.getFullYear()}-${(date.getMonth() + 1).toString().padStart(2, '0')}-${date.getDate().toString().padStart(2, '0')}`;
}

onMounted(() => {
    getMemberInfo()
    userStore.GetProfile()
})


</script>
    
<template>
    <v-container class="h-auto">
        <v-row class="" no-gutters>
            <v-col :cols="cols[0]">
                <v-sheet class="mt-10">
                    <v-avatar color="white" size="200" rounded="10" variant='elevated'>
                        <v-img cover :src="userStore.memberImg"></v-img>
                    </v-avatar>
                    <div class="ms-12 mt-1 text-overline" size="50px">
                        @{{ memberInfo.account }}
                    </div>
                    <v-chip class="ma-1 mx-12" label :color="memberInfo.isConfirm == 1 ? 'success' : 'grey'">
                        {{ memberInfo.isConfirm == 1 ? '已認證會員' : '未認證會員' }}
                    </v-chip>
                </v-sheet>
            </v-col>
            <v-col :cols="cols[1]">
                <v-sheet class="">
                    <v-row class="mt-2 ms-1">
                        <v-chip color="warning" text-color="white" style="width: 150px" class="justify-center"
                            variant="outlined" size="x-large">{{ memberInfo.memberLevelName }}</v-chip>
                    </v-row>
                    <v-row>
                        <v-col class="text-h2" cols="6">{{ memberInfo.name }}</v-col>
                    </v-row>
                    <v-row>
                        <v-col class="text-subtitle-1" cols="6"> Hi! {{ memberInfo.nickname }}</v-col>
                    </v-row>
                    <v-row>
                        <v-col cols="3">
                            <v-text-field :model-value="memberInfo.gender" label="性別" variant="underlined"
                                readonly></v-text-field>
                        </v-col>
                        <v-col cols="5">
                            <v-text-field :model-value="formatDate(memberInfo.birthday)" label="生日" variant="underlined"
                                readonly></v-text-field>
                        </v-col>
                    </v-row>
                    <v-row>
                        <v-col cols="12" sm="8">
                            <v-text-field :model-value="memberInfo.email" label="Email" variant="underlined"
                                readonly></v-text-field>
                        </v-col>
                    </v-row>
                    <v-row>
                        <v-col cols="5">
                            <v-text-field :model-value="memberInfo.phoneNumber" label="手機" variant="underlined"
                                readonly></v-text-field>
                        </v-col>
                        <v-col cols="3" class="ms-12">
                            <v-switch :model-value="memberInfo.dmSubscribe" label="訂閱電子報" color="warning" hide-details
                                readonly></v-switch>
                        </v-col>
                    </v-row>
                    <v-row style="height: 20px;">
                    </v-row>
                    <v-row>
                        <v-col cols="5">
                        </v-col>
                        <v-col cols="3">
                            <v-dialog v-model="dialog" persistent width="700">
                                <template v-slot:activator="{ props }">
                                    <v-btn :loading="loading" color="orange-darken-4" v-bind="props" variant="flat"
                                        @click="load" block> 編輯資訊</v-btn>
                                </template>
                                <v-card>
                                    <MemberInfoForm @memberInfoUpdated="memberEditedInfo">
                                    </MemberInfoForm>
                                    <v-card-actions>
                                        <v-spacer></v-spacer>
                                        <v-btn color="blue-darken-1" variant="text" @click="dialog = false">
                                            Close
                                        </v-btn>
                                        <v-btn color="blue-darken-1" variant="text" @click="submitEdit">
                                            Save
                                        </v-btn>
                                    </v-card-actions>
                                </v-card>
                            </v-dialog>
                        </v-col>
                    </v-row>
                    <v-row style="height: 150px;">
                        <v-col cols="5" class="text-subtitle-2" align-self="end">
                            帳號創建時間: {{ formatDate(memberInfo.createdAt) }}
                        </v-col>
                        <v-col cols="5" class="ms-10 text-subtitle-2" align-self="end">
                            最後修改時間: {{ formatDate(memberInfo.lastModified) }}
                        </v-col>
                    </v-row>
                </v-sheet>
            </v-col>
        </v-row>
    </v-container>
</template>
    