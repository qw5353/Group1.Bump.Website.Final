import { defineStore } from 'pinia';
import { ref } from 'vue';
import axios from 'axios';

export const useRegisterStore = defineStore('register', () => {

    const stepsData = ref({
        "id": 0,
        "account": "",
        "password": "",
        "confirmPassword": "",
        "name": "",
        "nickname": "",
        "thumbnail": "",
        "email": "",
        "gender": "",
        "birthday": "",
        "phoneNumber": "",
        "dmSubscribe": false
    })

    const frontEndError = ref([])
    const errorMsg = ref('')

    const register = async () => {
        try {
            const response = await axios.post('/api/Account/Register', stepsData.value);
            console.log(response.data);
            return response.data;
        } catch (err) {
            console.log(err)
            errorMsg.value = err.response.data
            return err.response.data
        }
    }

    const rules = ref({
        required: value => !!value || '必填',
        min: v => v.length >= 8 || '長度需要超過8字元',
        passwordMatch: (v) => v === stepsData.value.password || '密碼需要輸入一樣的值',
        email: value => {
            const pattern = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
            return pattern.test(value) || '請輸入有效email'
        }
    })

    const required = ref([value => !!value || '必填'])

    const valid = ref(false);

    return {
        stepsData, errorMsg, register,
        rules, required, valid, frontEndError
    }
})