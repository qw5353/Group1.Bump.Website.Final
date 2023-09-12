<script setup>
import { ref, computed, watch, onMounted, toRef, defineProps, defineEmits } from 'vue'
import { ElDatePicker } from 'element-plus';
import axios from 'axios';
const emit = defineEmits(['memberInfoUpdated']);

const memberEditInfo = ref({
    "id": 0,
    "name": "",
    "nickname": "",
    "gender": "",
    "birthday": "",
    "phoneNumber": "",
    "dmSubscribe": false
})

async function getEditInfo() {
    try {
        await axios.get('/api/Member/EditProfile')
            .then(res => memberEditInfo.value = res.data);
        emit('memberInfoUpdated', memberEditInfo.value)
    } catch (err) {
        console.log(err.data)
    }
}

onMounted(() => {
    getEditInfo()

})

</script>
    
<template>
    <v-container class="h-auto">
        <v-row no-gutters>
            <v-col>
                <v-sheet class="text-center">
                    <v-row>
                        <v-col class="text-overline self-align-center"> Hi! {{ memberEditInfo.name }}</v-col>
                    </v-row>
                    <v-row>
                        <v-col>
                            <v-text-field v-model="memberEditInfo.name" label="姓名" variant="underlined"></v-text-field>
                        </v-col>
                        <v-col><v-text-field v-model="memberEditInfo.nickname" label="暱稱" variant="underlined"
                                color="white"></v-text-field></v-col>
                    </v-row>
                    <v-row>
                        <v-col>
                            <v-text-field v-model="memberEditInfo.gender" label="性別" variant="underlined"></v-text-field>
                        </v-col>
                        <v-col style="text-align: left;">
                            <span class="demonstration">生日</span>
                            <div class="block">
                                <el-date-picker v-model="memberEditInfo.birthday" type="date"
                                    :placeholder="new Date(memberEditInfo.birthday)" popper-class="z-99999" />
                            </div>
                        </v-col>
                    </v-row>
                    <v-row>
                        <v-col cols="6">
                            <v-text-field v-model="memberEditInfo.phoneNumber" label="Phone"
                                variant="underlined"></v-text-field>
                        </v-col>
                        <v-col cols="auto">
                            <v-switch v-model="memberEditInfo.dmSubscribe" label="DM Subscribe" color="warning"
                                hide-details></v-switch>
                        </v-col>
                    </v-row>
                </v-sheet>
            </v-col>
        </v-row>
    </v-container>
</template>
    
<style>
.z-99999 {
    z-index: 99999 !important;
}

.block {
    text-align: center;
    border-right: solid 1px var(--el-border-color);
    flex: 1;
    width: 50px;
}

.demonstration {
    color: rgb(146, 146, 146);
}
</style>