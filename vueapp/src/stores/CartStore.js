import { ref } from 'vue'
import { defineStore } from 'pinia'
import axios from 'axios';
import { userStateStore } from './UserStateStore';


export const useCartStore = defineStore('cart', () => {
    const items = ref([]);
    const userStates = userStateStore();

    async function updateCartItems() {
        const res = await axios.get(`/api/cart?id=${userStates.userId}`);
        items.value = res.data.map(item => Object.assign({}, { ...item, checked: true }));
    }

    return { items, updateCartItems }
})