<script setup>
import { ref, onMounted } from 'vue'
import { mdiEyeOffOutline, mdiEyeOutline } from '@mdi/js';
import { useRoute, useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import axios from 'axios';

const route = useRoute();
const router = useRouter();
const hidePwd = ref(true)
const snackbar = ref(false)
const timeout = ref(1000)
const result = ref('')
const loading = ref(false)
const pwdForm = ref(null)
const valid = ref(false)

const rules = ref({
    required: value => !!value || '必填',
    min: v => v.length >= 8 || '長度需要超過8字元',
    passwordMatch: (v) => v === form.value.password || '密碼需要輸入一樣的值',
})

const form = ref({
    "id": 0,
    "password": "",
    "confirmPassword": "",
    "confirmCode": ""
})

function load() {
    loading.value = true
    setTimeout(() => {
        loading.value = false
    }, 500)
}

async function changePwd() {

    load()
    if (!valid.value) return

    try {
        const res = await axios.post('/api/Account/editPassword', form.value)
        result.value = res.data
        snackbar.value = true
        ElMessage({
            showClose: true,
            message: result.value,
            type: 'success',
        })
        setTimeout(() => {
            router.push('/')
        }, 500)
    } catch (err) {
        console.log(err)
        snackbar.value = true
        result.value = err.response.data
    }
}

function getColor(result) {
    return result === '密碼變更成功!' ? 'success' : 'error'
}

onMounted(() => {
    form.value.id = route.query.memberId
    form.value.confirmCode = route.query.confirmCode
})

</script>

<template>
    <v-container>
        <v-row class="justify-center my-8">
            <span class="text-h6">更新密碼</span>
        </v-row>
        <v-snackbar v-model="snackbar" :timeout="timeout" variant="tonal" :color="getColor(result)">{{
            result }}
        </v-snackbar>

        <v-form v-model="valid" @submit.prevent ref="pwdForm">
            <v-row no-gutters>
                <v-text-field v-model="form.password" color="warning" label="新密碼" :rules="[rules.required, rules.min]"
                    placeholder="Enter your password" :type="hidePwd ? 'password' : 'text'"
                    :append-inner-icon="hidePwd ? mdiEyeOffOutline : mdiEyeOutline" @click:append-inner="hidePwd = !hidePwd"
                    variant="underlined"></v-text-field>
            </v-row>
            <v-row no-gutters>
                <v-text-field v-model="form.confirmPassword" color="warning" label="確認新密碼"
                    :rules="[rules.required, rules.min, rules.passwordMatch]" placeholder="Try again your password"
                    :type="hidePwd ? 'password' : 'text'" :append-inner-icon="hidePwd ? mdiEyeOffOutline : mdiEyeOutline"
                    @click:append-inner="hidePwd = !hidePwd" variant="underlined" @keyup.enter="changePwd()"></v-text-field>
            </v-row>
        </v-form>
        <v-btn color="warning" :loading="loading" class="mt-4" block @click="changePwd()">
            變更密碼
        </v-btn>
    </v-container>
</template>
