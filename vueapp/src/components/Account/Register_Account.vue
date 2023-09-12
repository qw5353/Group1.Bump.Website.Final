<script setup>
import { ref, computed } from 'vue'
import { mdiEyeOffOutline, mdiEyeOutline } from '@mdi/js';
import { useRegisterStore } from '@/stores/registerStore.js'

const hidePwd = ref(true)
const store = useRegisterStore()
const datas = store.stepsData
const accountRules = computed(() => [store.rules.required, store.rules.min]);
const comfirmRules = computed(() => [store.rules.required, store.rules.min, store.rules.passwordMatch]);
const emailRules = computed(() => [store.rules.required, store.rules.min, store.rules.email]);

</script>

 
<template>
    <v-container>
        <v-text-field v-model="datas.account" color="warning" :rules="accountRules" label="帳號" placeholder="myAccount"
            variant="underlined" :error-messages="store.frontEndError"></v-text-field>

        <v-text-field v-model="datas.email" color="warning" type="email" :rules="emailRules" label="信箱"
            placeholder="example@gmail.com" variant="underlined" :error-messages="store.frontEndError"></v-text-field>

        <v-text-field v-model="datas.password" color="warning" label="密碼" :rules="accountRules"
            placeholder="Enter your password" :type="hidePwd ? 'password' : 'text'"
            :append-inner-icon="hidePwd ? mdiEyeOffOutline : mdiEyeOutline" @click:append-inner="hidePwd = !hidePwd"
            variant="underlined" :error-messages="store.frontEndError"></v-text-field>

        <v-text-field v-model="datas.confirmPassword" color="warning" label="確認密碼" :rules="comfirmRules"
            placeholder="Try again your password" :type="hidePwd ? 'password' : 'text'"
            :append-inner-icon="hidePwd ? mdiEyeOffOutline : mdiEyeOutline" @click:append-inner="hidePwd = !hidePwd"
            variant="underlined" :error-messages="store.frontEndError"></v-text-field>
    </v-container>
</template>