<script setup>
import axios from 'axios';
import { ref, computed, onMounted } from 'vue'
import { mdiEyeOffOutline, mdiEyeOutline } from '@mdi/js';
import { useRouter, useRoute } from 'vue-router';
import { userStateStore } from '../../stores/UserStateStore';
const userStore = userStateStore();

axios.defaults.withCredentials = true
const name = ref('')
const password = ref('')
const rememberMe = ref(false)
const hidePwd = ref(true)
const form = ref(null)
const router = useRouter();
const route = useRoute();

const passwordRules = ref([
    v => !!v || '密碼必填',
    v => (v && v.length >= 8 && v.length <= 20) ||
        '密碼長度需要介於8-20字元之間, 並且包含 大小寫字母 & 數字',
])

const errMsg = ref('')

const validate = async () => {
    const { valid } = await form.value.validate()
    if (valid) {
        var data = {
            "account": name.value,
            "password": password.value,
            "stayLogin": rememberMe.value
        }
        try {
            await userStore.Login(data);
            const redirectUrl = route.query?.redirect;
            if (redirectUrl) {
                router.push({ path: redirectUrl });
            } else {
                router.go(-1);
            }
        } catch (err) {
            errMsg.value = err.response.data
            console.log(err)
        }
    };
    return valid;
};

</script>

<template>
    <v-sheet width="300" class="mx-auto">
        <v-alert v-if="errMsg" type="error" :text="errMsg" variant="tonal"></v-alert>
        <v-form ref="form">
            <v-text-field v-model="name" :counter="20" label="Account" variant="underlined" required></v-text-field>

            <v-text-field v-model="password" :type="hidePwd ? 'password' : 'text'" :counter="20" :rules="passwordRules"
                variant="underlined" label="Password" :append-inner-icon="hidePwd ? mdiEyeOffOutline : mdiEyeOutline"
                @click:append-inner="hidePwd = !hidePwd" @keyup.enter="validate">
            </v-text-field>

            <v-checkbox v-model="rememberMe" label="下次登入時記住密碼"></v-checkbox>

            <div class="d-flex flex-column">
                <v-btn color="success" class="mt-4" block @click="validate">
                    登入
                </v-btn>
                <v-btn href="/ForgetPwd" color="warning" class="mt-4" block>
                    忘記密碼
                </v-btn>
            </div>
        </v-form>
    </v-sheet>
</template>

<style scoped>
.err {
    color: red;
    font-size: 14px;
    margin-top: 2px;
    margin-bottom: 5px;
    display: block;
}
</style>
    
