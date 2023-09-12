<script setup>
import { ref, computed } from 'vue'
import { ElDatePicker } from 'element-plus'

const genders = ref(['', '女性', '男性', '其他'])

const policy = ref('')

import { storeToRefs } from 'pinia';
import { useRegisterStore } from '@/stores/RegisterStore.js'

const { name, nickName, gender, birthday, phoneNumber, edm } = storeToRefs(useRegisterStore())
const store = useRegisterStore()
const datas = store.stepsData

</script>

<template>
    <v-container fluid>
        <v-row>
            <v-col cols="12" sm="6">
                <v-text-field v-model="datas.name" color="warning" label="名字" variant="underlined" :rules="store.required"
                    :error-messages="store.frontEndError"></v-text-field>
            </v-col>
            <v-col cols="12" sm="6">
                <v-text-field v-model="datas.nickname" color="warning" label="暱稱" variant="underlined"></v-text-field>
            </v-col>
        </v-row>
        <v-row>
            <v-col cols="12" sm="6">
                <v-combobox v-model="datas.gender" :items="genders" density="comfortable" label="性別" variant="underlined"
                    :rules="store.required"></v-combobox>
            </v-col>
            <v-col cols="12" sm="6">
                <span class="demonstration">生日</span>
                <div class="block">
                    <el-date-picker v-model="datas.birthday" type="date" placeholder="2000-01-01" :rules="store.required" />
                </div>
            </v-col>
        </v-row>
        <v-text-field v-model="datas.phoneNumber" color="warning" label="手機" placeholder="+886 914110201"
            variant="underlined" :rules="store.required"></v-text-field>

        <v-checkbox v-model="datas.dmSubscribe" label="訂閱電子報" density="compact"></v-checkbox>
        <v-checkbox v-model="policy" label="同意隱私權與政策設定" density="compact" :rules="store.required"
            :error-messages="store.frontEndError">
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
