<script setup>
import { ref } from 'vue'
import { ElMessage } from 'element-plus'
import { useRouter } from 'vue-router';
import axios from 'axios';

const router = useRouter()
const rules = ref({
    required: value => !!value || '必填',
    email: value => {
        const pattern = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
        return pattern.test(value) || '請輸入有效email'
    }
})

const email = ref('')
const result = ref('')
const snackbar = ref(false)
const timeout = ref(1000)
const loading = ref(false)

function load() {
    loading.value = true
    setTimeout(() => {
        loading.value = false
    }, 1000)
}

//後端撈資料看信箱是否存在
async function sentComfirmEmail() {
    try {
        load()
        const res = await axios.post('/api/Account/newPassword', JSON.stringify(email.value), {
            headers: {
                'content-Type': 'application/json'
            }
        })
        result.value = res.data

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
        result.value = err.response.data
        snackbar.value = true
    }
}

function getColor(result) {
    return result === '請去收信更改密碼' ? 'success' : 'error'
}

</script>

 
<template>
    <v-container>
        <v-row class="justify-center my-8">
            <span class="text-h6">忘記密碼</span>
        </v-row>
        <v-snackbar v-model="snackbar" :timeout="timeout" variant="tonal" :color="getColor(result)">{{
            result }}
        </v-snackbar>
        <v-row align="center" no-gutters style="height: 180px">
            <v-text-field v-model="email" color="warning" :rules="[rules.required, rules.email]" label="信箱"
                placeholder="example@gmail.com" variant="underlined" @keyup.enter="sentComfirmEmail()"></v-text-field>
            <v-btn color="warning" :loading="loading" class="mt-4" block @click="sentComfirmEmail()">
                送出認證信
            </v-btn>
        </v-row>
        <v-row>
        </v-row>
    </v-container>
</template>