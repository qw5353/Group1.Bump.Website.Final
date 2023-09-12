<script setup>
import { ref, onMounted, watch } from 'vue'
import { ElDatePicker } from 'element-plus'
import { useRoute, useRouter } from 'vue-router'
import { useRegisterStore } from '../../stores/registerStore';
import { userStateStore } from '../../stores/UserStateStore';
import { ElMessage } from 'element-plus'
import axios from 'axios'

const userStore = userStateStore()
const registerStore = useRegisterStore();
const datas = registerStore.stepsData;

const route = useRoute();
const router = useRouter()

const genders = ref(['', '女性', '男性', '其他'])
const policy = ref('')
const form = ref(null)
const loading = ref(false)

onMounted(() => {
    datas.account = route.query.Account;
    datas.name = route.query.Name;
    datas.email = route.query.Email
    datas.thumbnail = route.query.Thumbnail;
    datas.password = route.query.Account;
    datas.confirmPassword = route.query.Account;
})

const loginInfo = ref({
    account: "",
    password: "",
    stayLogin: false
})

function load() {
    loading.value = true
    setTimeout(() => {
        loading.value = false
    }, 100)
}

async function Submit() {
    load()
    if (!registerStore.valid) {
        ElMessage({
            showClose: true,
            message: '表單驗證有值錯誤',
            type: 'error',
        })
        return;
    }
    const result = await registerStore.register();
    if (result === "註冊成功!") {
        ElMessage({
            showClose: true,
            message: result,
            type: 'success',
        })
        loginInfo.value.account = registerStore.stepsData.account
        loginInfo.value.password = registerStore.stepsData.confirmPassword
        await userStore.Login(loginInfo.value);

        router.push('/')
    }
}

</script>

<template>
    <v-form v-model="registerStore.valid" @formSubmit.prevent ref="form">
        <v-container fluid>
            <v-row class="justify-center">
                <span class="text-h6">第三方登入註冊</span>
            </v-row>
            <v-row class="mt-2">
                <v-col cols="12">
                    <v-text-field v-model="datas.nickname" color="warning" label="暱稱" variant="underlined"></v-text-field>
                </v-col>
            </v-row>
            <v-row>
                <v-col cols="12" sm="6">
                    <v-combobox v-model="datas.gender" :items="genders" density="comfortable" label="性別"
                        variant="underlined"></v-combobox>
                </v-col>
                <v-col cols="12" sm="6">
                    <span class="demonstration">生日</span>
                    <div class="block">
                        <el-date-picker v-model="datas.birthday" type="date" placeholder="2000-01-01"
                            :rules="registerStore.required" />
                    </div>
                </v-col>

            </v-row>
            <v-text-field v-model="datas.phoneNumber" color="warning" label="手機" placeholder="+886 914110201"
                variant="underlined" :rules="registerStore.required"></v-text-field>

            <v-checkbox v-model="datas.dmSubscribe" label="訂閱電子報" density="compact"></v-checkbox>
            <v-checkbox v-model="policy" label="同意隱私權與政策設定" density="compact" :rules="registerStore.required">
                <template v-slot:label>
                    <div>
                        同意
                        <v-tooltip location="bottom">
                            <template v-slot:activator="{ props }">
                                <a target="_blank" href="/privacy" v-bind="props" @click.stop>
                                    隱私權
                                </a>
                            </template>
                            點我看Bump客戶隱私權條款
                        </v-tooltip>
                        與
                        <v-tooltip location="bottom">
                            <template v-slot:activator="{ props }">
                                <a target="_blank" href="/terms" v-bind="props" @click.stop>
                                    政策
                                </a>
                            </template>
                            點我看Bump電子商務約定條款
                        </v-tooltip>
                        設定
                    </div>
                </template>
            </v-checkbox>
        </v-container>
        <v-btn color="warning" :loading="loading" class="mt-7" block @click="Submit()">註冊為 Bump 會員</v-btn>
    </v-form>
</template>
<style scoped>
.block {
    text-align: center;
    border-right: solid 1px var(--el-border-color);
    flex: 1;
    width: 50px;
}

.block:last-child {
    border-right: none;
}

.demonstration {
    color: rgb(146, 146, 146);
}

a {
    text-decoration: none;
    color: orange
}
</style>
