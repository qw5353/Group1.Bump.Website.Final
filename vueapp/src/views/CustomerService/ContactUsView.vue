<script setup>
import DefaultLayout from '@/components/Layouts/DefaultLayout.vue'
import axios from 'axios';
import { ref } from 'vue';
import { mdiAlertCircleOutline } from '@mdi/js';
import { useTitle } from '@vueuse/core';

useTitle('Bump - 聯繫我們');

const valid = ref(false);
const sending = ref(false);
const alertVisible = ref(false);
const form = ref(null);
const snackbar = ref(false);

const name = ref("");
const nameRules = ref([
    value => {
        if (value) return true

        return '姓名必填'
    },
]);

const phoneNumber = ref("");
const phoneNumberRules = ref([
    value => {
        if (value) return true;

        return '電話號碼必填';
    },
]);

const email = ref("")
const emailRules = ref([
    value => {
        if (value) return true;

        return 'E-mail必填';
    },
    value => {
        if (/.+@.+\..+/.test(value)) return true;

        return '請使用合法的Email';
    },
]);

const subject = ref("");
const subjectRules = ref([
    value => {
        if (value) return true;

        return '問題主旨必填';
    },
])

const inquiry = ref("");
const inquiryRules = ref([
    value => {
        if (value) return true;

        return "問題內容必填";
    }
]);

async function submit() {
    if (!valid.value) return;

    
    try {
        sending.value = true;
        
        await axios.post('/api/ContactUs', {
            name: name.value,
            subject: subject.value,
            inquiry: inquiry.value,
            email: email.value,
            phoneNumber: phoneNumber.value
        });

        snackbar.value = true;
        form.value.reset();
    } catch (error) {
        alertVisible.value = true;
    } finally {
        sending.value = false;
    }
}

</script>

<template>
    <DefaultLayout>
        <main class="zy-container d-flex mx-auto py-8">
            <v-container class="container-bg px-md-16 px-sm-8">
                <h1>聯繫我們</h1>
                <v-alert v-if="alertVisible" color="error" :icon="mdiAlertCircleOutline"
                    class="mb-3">請再度嘗試, 若還是失敗請聯絡我們客服告知您遇到困難!</v-alert>
                <v-divider :thickness="2" class="border-opacity-75 mb-6" color="warning"></v-divider>
                <v-form v-model="valid" @submit.prevent ref="form">
                    <v-row>
                        <v-col cols="12">
                            <v-text-field v-model="name" :rules="nameRules" label="姓名" variant="outlined"
                                required></v-text-field>
                        </v-col>

                        <v-col cols="12">
                            <v-text-field v-model="phoneNumber" :rules="phoneNumberRules" :counter="10" label="電話號碼"
                                variant="outlined" required></v-text-field>
                        </v-col>

                        <v-col cols="12">
                            <v-text-field v-model="email" :rules="emailRules" label="E-mail" variant="outlined"
                                required></v-text-field>
                        </v-col>

                        <v-col cols="12">
                            <v-text-field v-model="subject" :rules="subjectRules" label="問題主旨" variant="outlined"
                                required></v-text-field>
                        </v-col>

                        <v-col cols="12">
                            <v-textarea v-model="inquiry" :rules="inquiryRules" auto-grow variant="outlined" label="詢問內容"
                                :counter="1000" required></v-textarea>
                        </v-col>

                        <v-col cols="12">
                            <v-btn type="submit" color="warning" variant="outlined" class="mx-auto d-flex" @click="submit"
                                :loading="sending">送出</v-btn>
                        </v-col>
                    </v-row>
                </v-form>
            </v-container>
        </main>
    </DefaultLayout>

    <v-snackbar v-model="snackbar" color="success" :timeout="3000">
        <v-alert type="success">我們已經收到您的聯繫!</v-alert>

        <template v-slot:actions>
            <v-btn color="white" variant="text" @click="snackbar = false">
                Close
            </v-btn>
        </template>
    </v-snackbar>
</template>

<style scoped>
h1 {
    text-align: center;
}

.container-bg {
    background-color: rgba(230, 169, 71, 0.1);
    border-radius: 20px;
}
</style>