<script setup>
import { mdiChat, mdiClose, mdiSend, mdiStickerEmoji } from '@mdi/js';
import { ref, computed, onUpdated, onUnmounted } from 'vue';
import ChatMessage from '../homepage/ChatMessage.vue';
import SystemMessage from '../homepage/SystemMessage.vue'
import { userStateStore } from '../../stores/UserStateStore';
import { v4 as uuidv4 } from 'uuid';
import * as signalR from "@microsoft/signalr";
import EmojiPicker from 'vue3-emoji-picker';
import 'vue3-emoji-picker/css';

const userState = userStateStore();


const chatVisible = ref(false);
const inputMessage = ref("");
const messages = ref([]);
const connected = ref(false);
const adminAlive = ref(true);
const isAuth = computed(() => userState.authenticate);
const username = computed(() => userState.userAccount);
const hasMessage = computed(() => inputMessage.value.length > 0);
const messageInputVisible = ref(true);
const emojiPickerVisible = ref(false);

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chat")
    .build();

const chatwindow = ref(null);

connection.on("messageReceived", (remoteUsername, message, sendtime) => {
    const newMessage = {
        message,
        isSelf: username.value === remoteUsername,
        key: uuidv4(),
        sendTime: sendtime.slice(0, 19).replace('T', ' ')
    };

    messages.value.push(newMessage);
});

connection.on("NoAdminAlive", async () => {
    adminAlive.value = false;
    messages.value = [];
});

connection.on("AdminAlive", () => {
    adminAlive.value = true
    connection.send('FetchHistoryMessage', username.value).then(() => (username.value));
});

connection.on("HistoryMessagesReceived", (msgs, ) => {
    const history = JSON.parse(msgs);
    
    messages.value.push(...history.map(m => {
        const { message, isSelf, sendTime } = m;
        return {
            message,
            isSelf,
            sendTime: sendTime.slice(0, 19).replace('T', ' '),
            key: uuidv4()
        }
    }));
})

connection
    .start()
    .then(() => {
        connected.value = true
    })
    .catch((err) => {
        adminAlive.value = false;
        console.log(err)
    });

connection.onclose(() => {
    messageInputVisible.value = false;
});

function send() {
    if (!hasMessage.value) return;

    connection.send("sendMessageToAdmin", username.value, inputMessage.value)
        .then(() => (inputMessage.value));

    inputMessage.value = "";
}

function onSelectEmoji(emoji) {
    inputMessage.value += emoji.i;
    emojiPickerVisible.value = false;
}

onUpdated(() => {
    chatwindow.value.$el.scrollTop = chatwindow.value.$el.scrollHeight;
})

onUnmounted(async () => {
    await connection.stop();
}) 

</script>

<template>
    <v-btn :icon="mdiChat" class="chat-btn" color="#E6A947" v-if="!chatVisible && isAuth"
        @click="chatVisible = true"></v-btn>
    <section v-if="chatVisible">
        <div class="chat-window d-flex flex-column h-100">
            <div class="chat-header px-3 pt-2 text-h6 d-flex my-3">
                <span>聊天室</span>
                <v-icon :icon="mdiClose" @click="chatVisible = false" size="small" class="ms-auto"></v-icon>
            </div>
            <v-divider :thickness="2"></v-divider>
            <div class="chat-body">
                <v-container fluid class="pa-4 px-6 pb-12 h-100 overflow-y-auto" ref="chatwindow">
                    <SystemMessage v-if="!adminAlive">目前沒有客服人員在線上，請您移駕至 <router-link
                            to="/contact-us">聯繫我們</router-link>填寫您的問題。</SystemMessage>
                    <SystemMessage v-if="!connected">連線中...</SystemMessage>
                    <ChatMessage v-for="message in messages" :is-self="message.isSelf" :key="message.key" v-show="adminAlive" :send-time="message.sendTime">{{ message.message
                    }}</ChatMessage>
                </v-container>
            </div>
            <div class="chat-footer d-flex align-center">
                <v-text-field placeholder="請輸入訊息" variant="solo-filled" class="p-6" @keyup.enter="send"
                    v-model="inputMessage" hide-details="auto" v-show="messageInputVisible">
                    <template v-slot:append-inner>
                        <EmojiPicker :native="false" :display-recent="true" @select="onSelectEmoji" v-if="emojiPickerVisible"></EmojiPicker>
                        <v-btn :icon="mdiStickerEmoji" @click="emojiPickerVisible = true" />
                        <v-btn :disabled="!hasMessage" :icon="mdiSend" @click="send" />
                    </template>
                </v-text-field>
            </div>
        </div>
    </section>
</template>

<style scoped>
.chat-btn {
    position: fixed;
    bottom: 3vh;
    right: 3vh;
    color: white;
}

.chat-header {
    flex: 0 0 40px;
    width: 100%;
}

.chat-body {
    flex: 1 1 auto;
    overflow-y: auto;
}

.chat-footer {
    flex: 0 0 36px;
    justify-self: end;
}

section {
    position: fixed;
    bottom: 1vh;
    right: 1vh;
    background-color: white;

    width: 300px;
    height: 400px;
    border-radius: 4%;
}
</style>