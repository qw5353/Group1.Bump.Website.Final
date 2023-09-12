<script setup>
import { ref, watch, onMounted } from 'vue';
import axios from 'axios';
const emailRules = ref([
    value => !!value || '必填欄位',
    value => (value || '').length <= 40 || '超過字數上限',
    value => {
        const pattern = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
        return pattern.test(value) || 'Invalid e-mail.'
    },
])
const phoneRules = ref([
    value => !!value || '必填欄位'
])
const requiredRules = ref([
    value => !!value || '必填欄位'
])
const selected = ref(false);

const receiver = ref(
    {
        name: "",
        phone: "",
        email: "",
        address: "",
        note: ""
    }
);
const memeberInfo = ref();
onMounted(async () => {
    try {
        const res = await axios.get('/api/cart/memberInfo?id=1');
        memeberInfo.value = res.data;
        console.log(memeberInfo.value);
    } catch (err) {
        console.error(err);
    }
})

watch(selected, function () {
    if (selected.value) {
        receiver.value = { ...receiver.value, ...memeberInfo.value };
        delete receiver.value.points;
        console.log('1231231231');
    } else {
        for (const key in receiver.value) {
            receiver.value[key] = "";
        }
    }
})

const emit = defineEmits(['orderInfo']);

watch(receiver, function () {
    emit('orderInfo', receiver.value);
}, {
    deep: true
})


</script>

<template>
    <p class="text-subtitle-1 font-weight-bold my-5">訂購者資料</p>
    <v-row no-gutters v-if="memeberInfo">
        <v-col cols="4">
            <v-text-field class="w-75" readonly>{{ memeberInfo.name }}</v-text-field>
        </v-col>
        <v-col cols="4">
            <v-text-field class="w-75" readonly>{{ memeberInfo.phone }}</v-text-field>
        </v-col>
        <v-col cols="4">
            <v-text-field class="w-100" readonly>{{ memeberInfo.email }}</v-text-field>
        </v-col>
    </v-row>

    <v-row no-gutters>
        <v-col cols="2">
            <p class="text-subtitle-1 font-weight-bold my-5">取貨者資料</p>
        </v-col>
        <v-col cols="2">
            <v-checkbox v-model="selected" label="同訂購者資料" color="orange"></v-checkbox>
        </v-col>
    </v-row>
    <v-row no-gutters class="mb-5">
        <v-col cols="4">
            <v-text-field :rules="requiredRules" label="取貨者" class="w-75" v-model="receiver.name"></v-text-field>
        </v-col>
        <v-col cols="4">
            <v-text-field :rules="phoneRules" label="手機號碼" class="w-75" v-model="receiver.phone"></v-text-field>
        </v-col>
        <v-col cols="4">
            <v-text-field :rules="emailRules" label="Email" class="w-100" v-model="receiver.email"></v-text-field>
        </v-col>
    </v-row>
    <v-text-field :rules="requiredRules" label="地址" class="w-100 mb-5" v-model="receiver.address"></v-text-field>
    <v-textarea counter label="備註" maxlength="300" single-line v-model="receiver.note"></v-textarea>
</template>

<style scoped>
p {
    letter-spacing: 0.2em !important;
}
</style>