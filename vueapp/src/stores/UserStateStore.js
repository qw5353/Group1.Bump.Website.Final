import { defineStore } from 'pinia';
import { watch, computed, ref } from 'vue';
import { useCartStore } from './CartStore';
// import { useRouter } from 'vue-router';
import axios from 'axios';

export const userStateStore = defineStore('userStore', () => {

    axios.defaults.withCredentials = true;

    const authenticate = ref(false)
    const claims = ref([]);
    const userId = computed(() => claims.value.memberId)
    const userName = computed(() => claims.value.name)
    const userAccount = computed(() => claims.value.account)
    const userEmail = computed(() => claims.value.emailaddress)
    const userImg = computed(() => claims.value.profileImg)
    const userPhoneNumber = computed(() => claims.value.phoneNumber)
    const cart = useCartStore();
    const memberInfo = ref({})
    const memberImg = computed(() => memberInfo.value.thumbnail)
    const memberIsConfirm = computed(() => memberInfo.value.isConfirm)
    // const router = useRouter()

    const avatar = ref(null)

    function updateAvatar(newImg) {
        memberInfo.value.thumbnail = newImg
        console.log(userImg.value);
    }

    async function getAvatar(newImg) {
        const res = await axios.post('/api/Member/UpdateAvatar', JSON.stringify(newImg), {
            headers: {
                'Content-Type': 'application/json',
            },
        });
        return res.data
    }

    function setAuthentication(authenticated) {
        authenticate.value = authenticated
    }

    function IsLogin() {
        return axios.get('/api/Account')
    }

    async function GetProfile() {
        try {
            const res = await axios.get('/api/Member')
            memberInfo.value = res.data

            if (avatar.value) {
                const newAvatar = getAvatar(avatar.value)
                await updateAvatar(newAvatar)
            }

        } catch (err) {
            console.log(err)
        }
    };

    async function GetUserInfo() {
        try {
            const res = await axios.get('/api/Account/GetMemberStatus')
            claims.value = res.data.claims;
        } catch (err) {
            console.log(err)
        }
    };

    function Login(data) {
        return axios.post('/api/Account/Login', data)
            .then(() => {
                setAuthentication(true)
                GetUserInfo()
                GetProfile()
            })
    }

    function Logout() {
        return axios.delete('/api/Account/Logout')
            .then(() => {
                setAuthentication(false)
                localStorage.removeItem('userStore');
                cart.items = [];
                // console.log(router)
                // router.push('/');
            })
    }


    watch([authenticate, claims, memberInfo], () => {
        const datas = {
            authenticate: authenticate.value,
            claims: claims.value,
            memberInfo: memberInfo.value
        }
        localStorage.setItem('userStore', JSON.stringify(datas))
        if (!authenticate.value) localStorage.removeItem('userStore')
    })

    const storeState = localStorage.getItem('userStore');
    if (storeState) {
        const state = JSON.parse(storeState);
        authenticate.value = state.authenticate;
        claims.value = state.claims
        memberInfo.value = state.memberInfo
    }

    return {
        authenticate, claims,
        setAuthentication, IsLogin, GetUserInfo, Login, Logout, updateAvatar, getAvatar, GetProfile,
        userId, userName, userAccount, userEmail, userImg, userPhoneNumber,
        avatar, memberImg, memberIsConfirm
    }
})