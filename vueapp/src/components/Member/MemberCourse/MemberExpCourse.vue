<script setup>
import { ref, onMounted } from 'vue';
import "@/assets/scss/bootstrap.scss";
import "bootstrap/dist/js/bootstrap.js"
import axios from 'axios';
import { userStateStore } from '../../../stores/UserStateStore';
const courseData = ref([]);
const userStates = userStateStore();
onMounted(async () => {
    try {
        const res = await axios.get(`/api/ExperienceEnrollments/${userStates.userId}`);
        courseData.value = res.data;
        // console.log(courseData.value)
    } catch (err) {
        console.error(err);
    }
})
</script>

<template>
    <v-container fluid class="mx-n4">
        <table class="table custom-table-striped">
            <thead>
                <tr>
                    <th class="text-center">
                        編號
                    </th>
                    <th class="text-center">
                        體驗開始時間
                    </th>
                    <th class="text-center">
                        體驗結束時間
                    </th>
                    <th class="text-center">
                        體驗地點
                    </th>
                    <th class="text-center">
                        人數
                    </th>
                    <th class="text-center">
                        成立時間
                    </th>
                    <th class="text-center">
                        報名狀態
                    </th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="item in courseData" :key="item.id" class="">
                    <td align="center" valign="middle">{{ item.id }}</td>
                    <td align="center" valign="middle">{{ item.startTime }}</td>
                    <td align="center" valign="middle">{{ item.endTime }}</td>
                    <td align="center" valign="middle">{{ item.fieldName }}</td>
                    <td align="center" valign="middle">{{ item.numberOfPeople }}</td>
                    <td align="center" valign="middle">{{ item.createdAt }}</td>
                    <td align="center" valign="middle">成功</td>
                </tr>
                <br>
            </tbody>
        </table>
    </v-container>
</template>

<style scoped>
.margin1 {
    padding-top: 20px !important;
}

th {
    background-color: #dc9511 !important;
    color: rgb(255, 255, 255) !important;
}

.custom-table-striped tbody tr:nth-child(odd) td {
    background-color: #fef9f3;
}
</style>