<script setup>
import { ref, computed } from 'vue';
import DefaultLayout from '../../components/layouts/DefaultLayout.vue';
import { useTitle } from '@vueuse/core';
useTitle('Bump - 課程');
import 'element-plus/es/components/form/style/css'
import 'element-plus/es/components/time-select/style/css'
import 'element-plus/es/components/button/style/css'
import { ElTimeSelect } from 'element-plus';
import { ElInput } from 'element-plus';
import { ElForm } from 'element-plus';
import { ElFormItem } from 'element-plus';
import { ElButton } from 'element-plus';
import { ElDatePicker } from 'element-plus';
import { ElConfigProvider } from 'element-plus'
import { ElSelect } from 'element-plus'
import { ElOption } from 'element-plus'
import dayjs from 'dayjs'
import axios from 'axios';
//取得場館ID
import { useRoute, useRouter } from 'vue-router'
const route = useRoute()
const field1 = ref([]);
const fieldName = ref(field1.value?.name)
const title = ref(new URL('/src/assets/images/course-index-carousel/title.jpg', import.meta.url).href);
const loadFields = async () => {
    const response = await fetch(`/api/Fields/${route.params.id}`)
    const datas = await response.json();
    field1.value = datas;
    fieldName.value = field1.value?.name;

}
loadFields();
//抓取會員資料
import { userStateStore } from '../../stores/UserStateStore';
const useStore = userStateStore()

//表單資料
const form = ref({
    id: '',
    fieldId: route.params.id,
    name: '',
    phone: '0912345678',
    email: '',
    peopleNumber: '1',
    selectedOption: '',
    startTime: '',
    endTime: '',
    date: new Date()
})

form.value.id = useStore.userId
form.value.name = useStore.userName
form.value.email = useStore.userEmail

//日期不得選今天以前
const disabledDate = (time) => {
    return time.getTime() < Date.now()
}
//預設方案資料
const options = [
    {
        value: 1,
        label: '一對一方案',
    },
    {
        value: 2,
        label: '一對二方案',
    },
    {
        value: 3,
        label: '一對三方案',
    },
    {
        value: 4,
        label: '一對四方案',
    }
]
//設定人數與方案互相綁定
form.value.selectedOption = computed(() => options.find(option => option.value == form.value.peopleNumber)?.label);

//時間選擇器
const startPickerOptions = ref({
    start: '08:00',
    step: '01:00',
    end: '23:00',
});

const endPickerOptions = ref({
    start: '08:00',
    step: '01:00',
    end: '23:00',
});

form.value.endTime = computed(() => {
    return dayjs(form.value.startTime, 'HH:mm').add(1, 'hour').format('HH:mm');
})
//將form資料轉成可以寫入資料庫的格式
const Todata = computed(() => {
    if (!form.value) return {};
    return {
        MemberId: form.value.id,
        NumberOfPeople: form.value.peopleNumber,
        FieldId: form.value.fieldId,
        StartTime: `${form.value.date} ${form.value.startTime}`,
        EndTime: `${form.value.date} ${form.value.endTime}`
    };
});

//寫入資料
const responseData = ref(null);
const error = ref(null);
const router = useRouter();

const postData = async () => {
    try {
        const response = await axios.post('/api/ExperienceEnrollments', Todata.value);

        router.push('/ExperienceCourseFinish');
        error.value = null;
    } catch (e) {
        error.value = e.message;
        responseData.value = null;
    }
};

</script>

<template>
    <el-config-provider :locale="locale">
        <DefaultLayout>
            <v-container>
                <v-row no-gutters>
                    <v-col cols="12" class="d-flex justify-center">
                        <v-img width="75%" aspect-ratio="16/9" cover :src="title" transition="slide-x-transition"></v-img>
                    </v-col>
                </v-row>
            </v-container>
            <v-container>
                <v-row>
                    <v-col cols="4"></v-col>
                    <v-col cols="4" class="text-center">
                        <h2>立即預約體驗{{ fieldName }}</h2>
                    </v-col>
                    <v-col cols="4"></v-col>
                </v-row>
            </v-container>
            <v-container class="d-flex justify-center align-center">
                <el-form :model="form" label-width="120px" class="custom-form ">
                    <v-row>
                        <v-col>
                            <el-form-item label="會員名字:">
                                <el-input v-model="useStore.userName" disabled />
                            </el-form-item>
                        </v-col>
                        <v-col>
                            <el-form-item label="會員電話:">
                                <el-input v-model="form.phone" disabled />
                            </el-form-item>
                        </v-col>
                    </v-row>
                    <v-row>
                        <v-col>
                            <el-form-item label="會員email:">
                                <el-input v-model="useStore.userEmail" disabled />
                            </el-form-item>
                        </v-col>
                    </v-row>
                    <v-row>
                        <v-col>
                            <el-form-item label="人數:">
                                <el-select v-model="form.peopleNumber" placeholder="人數" @input="handleInput">
                                    <el-option label=1 value=1 />
                                    <el-option label=2 value=2 />
                                    <el-option label=3 value=3 />
                                    <el-option label=4 value=4 />
                                </el-select>
                            </el-form-item>
                        </v-col>
                        <v-col>
                            <el-form-item label="體驗課方案:">
                                <el-select disabled v-model="form.selectedOption" class="m-2" placeholder="體驗課方案">
                                </el-select>
                            </el-form-item>
                        </v-col>
                    </v-row>
                    <v-row>
                        <v-col>
                            <el-form-item label="上課日期:" prop="date" :rules="[{ required: true, message: '日期不得空白' }]">
                                <el-date-picker v-model="form.date" type="date" placeholder="選擇一個日期" style="width: 100%"
                                    :disabled-date="disabledDate" value-format="YYYY-MM-DD" />
                            </el-form-item>
                        </v-col>
                        <v-col>
                            <el-form-item label="上課時間:" prop="startTime" :rules="[{ required: true, message: '時間不得空白' }]">
                                <el-time-select v-model="form.startTime" :picker-options="startPickerOptions"
                                    placeholder="選擇時間" format="HH:mm" step="01:00"></el-time-select>
                            </el-form-item>
                        </v-col>
                        <v-col>
                            <el-form-item label="結束時間:">
                                <el-time-select v-model="form.endTime" :picker-options="endPickerOptions" placeholder="結束時間"
                                    format="HH:mm" step="01:00" disabled></el-time-select>
                            </el-form-item>
                        </v-col>
                        <v-col>
                            <el-form-item>
                                <el-button type="warning" plain @click="postData">表單送出</el-button>

                            </el-form-item>
                        </v-col>
                    </v-row>
                </el-form>
            </v-container>

            <v-container>
                <v-row class="d-flex justify-center align-center">

                    <v-col cols="6" class="bgc2 ">
                        <h2>體驗注意事項:</h2>
                        <p>限首次、年滿 18 歲且出示身分證件<br>
                            請提前一天預約體驗時間<br>
                            請自備水壺或水杯（岩館提供飲水設備）<br>
                            穿著舒適、適合運動之服裝<br>
                            請事先修剪指甲<br>
                            建議穿著或攜帶襪子</p>
                    </v-col>

                </v-row>
            </v-container>


        </DefaultLayout>
    </el-config-provider>
</template>

<style scoped>
.bgc2 {
    background: linear-gradient(90deg, #FEEDD7 0%, #FCCC8D);
}

.text-center {
    text-align: center;
}

.custom-form {
    padding: 10px;
    /* 自定义内边距 */
    width: 1000px;
    /* 自定义宽度 */
}
</style>