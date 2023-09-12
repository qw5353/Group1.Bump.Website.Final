<script setup>
import { ref, onMounted } from 'vue';
import DefaultLayout from '../../components/layouts/DefaultLayout.vue';
import { useTitle } from '@vueuse/core';


useTitle('Bump - 課程');
import { useRoute } from 'vue-router'

const route = useRoute()
const SkillcurriculumsDetial = ref([]);
const course2 = ref([]);
onMounted(async () => {
  try {
    const res = await fetch(`/api/Skillcurriculums/${route.params.id}`)
    SkillcurriculumsDetial.value = await res.json();
    course2.value = SkillcurriculumsDetial["_rawValue"]
  } catch (err) {
    console.log(err)
  }
})

const formattedTime = (dateTime) => {
  if (!dateTime) return ''; // Handle case where dateTime is not set
  const timeString = dateTime.split('T')[1];
  const [hours, minutes] = timeString.split(':');
  return `${hours}:${minutes}`;
};
</script>

<template>
  <DefaultLayout>
    <v-container fluid class="p-0">
      <v-row>
        <v-col cols="8" class=" justify-end mb-6 ">
          <v-img width="900px" height="500px" aspect-ratio="16/9" cover
            :src="`/files/images/SkillCourses/${course2.thumbnail}`" transition="slide-x-transition"
            class="ms-auto my-auto"></v-img>
        </v-col>
        <v-col cols="4" class="p-12">
          <v-card class="w-75 d-flex justify-center flex-column" max-width="344" height="500" variant="outlined">
            <v-card-item>
              <div>
                <div class="text-h6 mb-1">
                  {{ course2.name }}
                </div>
                <div class="text-caption">{{ course2.description }}</div>
                <div class="larger-font">NT{{ course2.price }}元</div>
              </div>
            </v-card-item>

            <v-card-actions>
              <v-btn :to="`/skillcourseOrder/${course2.id}`" block variant="outlined">
                立即預定
              </v-btn>
            </v-card-actions>
          </v-card>
        </v-col>
      </v-row>
    </v-container>
    <v-container class="p-0">
      <v-row class=" ms-16 me-16 d-flex justify-center align-center">
        <v-col cols="8" class="bgc">
          <h2>課程資訊:</h2>
          <p>時間:{{ formattedTime(course2.startTime) }}-{{ formattedTime(course2.endTime) }}</p>
          <p>人數:{{ course2.peopleMin }}-{{ course2.peopleMax }}人</p>
          <p>授課教練:{{ course2.coachName }}</p>
        </v-col>
      </v-row>
    </v-container>
    <v-container class="p-0">
      <v-row class=" ms-16 me-16  d-flex justify-center align-center">
        <v-col cols="8" class="bgc2">
          <h2>注意事項:</h2>
          <p>請自備水壺或水杯（岩館提供飲水設備）<br>
            穿著舒適、適合運動之服裝<br>
            請事先修剪指甲<br>
            建議穿著或攜帶襪子</p>
        </v-col>
      </v-row>
    </v-container>
  </DefaultLayout>
</template>

<style scoped>
.fill-card {
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  width: 100%;
  height: 100%;
}

.bgc {
  background: linear-gradient(90deg, #FCBD9C 40%, #FEE5D7);
}

.bgc2 {
  background: linear-gradient(90deg, #FEEDD7 0%, #FCCC8D);
}

.bgc3 {
  background-color: yellow;
}

.larger-font {
  font-size: 20px;
  color: red;
}
</style>