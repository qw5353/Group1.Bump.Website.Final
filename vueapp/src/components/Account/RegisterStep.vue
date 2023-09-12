<template>
    <el-steps :active="active" finish-status="success" align-center>
        <el-step title="Step 1" description="創建帳號" />
        <el-step title="Step 2" description="選擇大頭貼" />
        <el-step title="Step 3" description="個人資訊" />
    </el-steps>
    <!-- <el-button style="margin-top: 12px" @click="next">Next step</el-button> -->
</template>
    
<script setup>
import { onMounted, defineEmits } from 'vue'
import 'element-plus/es/components/calendar/style/css'
import { ElSteps, ElStep } from 'element-plus'

const { active } = defineProps(['active'])
const emits = defineEmits()

// 這個方法將在父元件的 nextStep 被調用時觸發
function handleNextStepFromParent() {
    if (active.value < 2) {
        active.value += 1
    }
}

// 這個方法將在父元件的 prevStep 被調用時觸發
function handlePrevStepFromParent() {
    if (active.value > 0) {
        active.value -= 1
    }
}

// 監聽來自父元件的事件
onMounted(() => {
    emits('next-step', handleNextStepFromParent)
    emits('prev-step', handlePrevStepFromParent)
})
</script>
    
<style scoped></style>