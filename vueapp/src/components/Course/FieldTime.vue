<script setup>
import { ref } from 'vue';
import 'element-plus/es/components/calendar/style/css'
import { ElCalendar } from 'element-plus'
defineProps(['dataRef2'])
</script>

<template>
  <el-calendar>
    <template #date-cell="{ data }">
      <p :class="data.isSelected ? 'is-selected' : ''">
        {{ data.day.split('-').slice(1).join('-') }}
        {{ data.isSelected ? '✔️' : '' }}
      <div v-for="todo in dataRef2.find(d => d.day === data.day)?.todos" :key="todo.id">
        {{ todo.task }}
        <v-btn color="orange-lighten-2" variant="text" :to="`/SkillCourseDetial/${todo.id}`">
          點我看課表
        </v-btn>
      </div>
      </p>
    </template>
  </el-calendar>
</template>

<style scoped>
.date-cell {
  text-align: center;
  padding: 5px;
  border: 1px solid #ddd;
}

.is-selected {
  color: #1989fa;
  background-color: #f0f7ff;
}

.todo {
  margin-top: 5px;
  color: #666;
}

.remark-text {
  font-size: 8px;
}

.calendar-item {
  flex-direction: column;
}

.calendar-time {
  height: 16px;
  line-height: 16px;
}
</style>


