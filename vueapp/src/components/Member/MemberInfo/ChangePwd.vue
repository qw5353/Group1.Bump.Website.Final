<script setup>
import { ref, computed } from 'vue'
import { userStateStore } from '../../../stores/UserStateStore';
import { mdiEyeOffOutline, mdiEyeOutline } from '@mdi/js';
import { ElMessage } from 'element-plus'
import axios from 'axios';

const userState = userStateStore();

const dialog = ref(false)
const result = ref('')
const hidePwd = ref(true)
const snackbar = ref(false)
const timeout = ref(2000)
const pwdForm = ref(null)

const cols = computed(() => {
    return [3, 9]
})

const changeInfo = ref({
    "oriPassword": "",
    "password": "",
    "confirmPassword": ""
})

const required = ref([
    value => !!value || '必填'
]);

const confirmRules = ref([
    value => !!value || '必填',
    (v) => v === changeInfo.value.password || '密碼需要輸入一樣的值',
]);

async function editPwd() {
    try {
        const res = await axios.post('/api/Member/EditPwd', changeInfo.value)
        result.value = res.data
        // snackbar.value = true
        ElMessage({
            showClose: true,
            message: result.value,
            type: 'success',
        })
        changeInfo.value = '';
        dialog.value = false

    } catch (err) {
        console.log(err)
        result.value = err.response.data
        snackbar.value = true
        dialog.value = false
    }
}

function getColor(result) {
    return result === '密碼修改成功!' ? 'success' : 'error'
}

//todo
//未認證帳號無法修改密碼

</script>
    
<template>
    <v-container class="h-auto">
        <v-row no-gutters>
            <v-col :cols="cols[0]">
                <v-sheet class="mt-16">
                    <v-row>
                        <v-col class="text-h2 text-center" cols="12"> {{ userState.userName }}</v-col>
                    </v-row>
                    <v-row>
                        <v-col class="text-overline text-center" cols="12"> @{{ userState.userAccount }}</v-col>
                    </v-row>
                    <v-row>
                        <!-- <v-chip class="ma-1 mx-12" label>
                            未認證會員
                        </v-chip> -->
                    </v-row>
                </v-sheet>
            </v-col>
            <v-col :cols="cols[1]">
                <v-card width="600" height="630" variant="flat">
                    <template v-slot:text>
                        <v-container>
                            <v-snackbar v-model="snackbar" :timeout="timeout" variant="tonal" :color="getColor(result)">
                                {{ result }}
                            </v-snackbar>
                            <v-row class="justify-center my-8">
                                <span class="text-h6">更新密碼</span>
                            </v-row>
                            <v-alert v-if="result && result !== '密碼修改成功!'" variant="tonal" type="error"
                                :text="result"></v-alert>
                            <v-form ref="pwdForm">
                                <v-row no-gutters class="mb-5">
                                    <v-text-field v-model="changeInfo.oriPassword" color="primary" label="舊密碼"
                                        :rules="required" placeholder="Enter your password" type='password'
                                        :append-inner-icon="mdiEyeOffOutline" variant="underlined"></v-text-field>
                                </v-row>
                                <v-row no-gutters>
                                    <v-text-field v-model="changeInfo.password" color="primary" label="新密碼"
                                        :rules="required" placeholder="Enter your password"
                                        :type="hidePwd ? 'password' : 'text'"
                                        :append-inner-icon="hidePwd ? mdiEyeOffOutline : mdiEyeOutline"
                                        @click:append-inner="hidePwd = !hidePwd" variant="underlined"></v-text-field>
                                </v-row>
                                <v-row no-gutters>
                                    <v-text-field v-model="changeInfo.confirmPassword" color="primary" label="確認新密碼"
                                        :rules="confirmRules" placeholder="Try again your password"
                                        :type="hidePwd ? 'password' : 'text'"
                                        :append-inner-icon="hidePwd ? mdiEyeOffOutline : mdiEyeOutline"
                                        @click:append-inner="hidePwd = !hidePwd" variant="underlined"></v-text-field>
                                </v-row>
                            </v-form>
                            <v-dialog v-model="dialog" persistent width="500">
                                <template v-slot:activator="activator">
                                    <v-btn color="orange-darken-4" variant="flat" v-on="activator" @click="dialog = true"
                                        block> 變更密碼 </v-btn>
                                </template>
                                <v-card>
                                    <v-card-title class="text-h5"> 確定變更密碼? </v-card-title>
                                    <v-card-actions>
                                        <v-spacer></v-spacer>
                                        <v-btn color="warning" variant="text" @click="dialog = false">
                                            取消
                                        </v-btn>
                                        <v-btn color="warning" variant="text" @click="editPwd">
                                            變更密碼
                                        </v-btn>
                                    </v-card-actions>
                                </v-card>
                            </v-dialog>
                        </v-container>
                    </template>
                </v-card>
            </v-col>
        </v-row>
    </v-container>
</template>