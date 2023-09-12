# 須知

## 使用說明

### 如何抓到登入的使用者

Previous. 這些都要先在右上角 登入後 才抓的到值

Step1. 引入 store.js

`import { userStateStore } from '../../stores/UserStateStore';`

Step2. 幫引入的store 取個名字 (以下舉例'store' 可自行更換)

`const store = userStateStore()`

Step3. 直接抓裡面的屬性即可使用

目前驗證票有存的資訊 : userId / userName / userAccount / userEmail / userImg
store.userId

Optional 如果同一個畫面有分 [ 已認證 & 未認證 ] 的資訊

可以在最前面引入
`const isAuthenticated = computed(() => store.authenticate)`
使用 isAuthenticated 去當判斷式使用

---

1. 先至 vueapp, npm install

## Config Setting

1. 要修改port 請分別至 webapi/properties/launchSettings.json 及 vueapp/vite.config.js 及 vueapp/.vscode/launchJson底下查看設定

## 環境需求

1. 安裝 NodeJS, npm [可考慮安裝 PNPM 速度快蠻多的]
