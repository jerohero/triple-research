<script setup lang="ts">
  import axios from '@/shared/axios'
  import type { AxiosResponse } from 'axios'
  import { onMounted, ref } from 'vue'
  import LoadSpinner from '@/components/LoadSpinner.vue'

  interface Negotiate {
    Url: string
  }

  const props = defineProps<{
    id: string
  }>()

  const events = ref<any[]>()
  const isConnecting = ref<boolean>()

  onMounted(async () => {
    isConnecting.value = true

    axios().post(`session/${ props.id }/negotiate`)
      .then((res: AxiosResponse<Negotiate>) => {
        console.log(res.data.Url)
        const ws = new WebSocket(res.data.Url)

        ws.onopen = () => {
          console.log('Connected!')
          isConnecting.value = false
        }

        ws.onmessage = (event: MessageEvent) => {
          const data = JSON.parse(event.data)
          events.value = [data, ...data.value]
        }
      })

    const example = [
      {
        predictions: [
          {
            class: 'car',
            confidence: 0.5,
            x1: 92,
            x2: 241,
            y1: 123,
            y2: 491,
            width: 200,
            height: 502
          }
        ]
      }
    ]

    function duplicate(array: any[], duplicator: number){
      const buildArray = [];
      for(let i=0; i<array.length; i++){
        for(let j=0; j<duplicator; j++){
          buildArray.push(array[i]);
        }
      }
      return buildArray;
    }

    events.value = duplicate(example, 500)
  })
</script>

<template>
  <div>
    <div v-if="isConnecting"
         class="py-10 absolute right-[50%] left-[50%]"
    >
      <LoadSpinner />
    </div>
    <div>
<!--      <div v-for="event in events" v-bind:key="Date.now() + event">-->
<!--        <div>-->
<!--          <p> {{ JSON.stringify(event) }} </p>-->
<!--        </div>-->
<!--      </div>-->
      <div>
        <ul>
          <li v-for="(value, key) in events" :key="key">
            <strong>{{ key }}:</strong>
            <span v-if="typeof value === 'object' && !Array.isArray(value)">
            <div>
            <ul>
              <li v-for="(value, key) in value" :key="key">
                <strong>{{ key }}:</strong> {{ value }}
              </li>
            </ul>
          </div>
        </span>
            <span v-else-if="Array.isArray(value)">
          <ul>
            <li v-for="(item, index) in value" :key="index">
              <span v-if="typeof item === 'object'">
                  <div>
                <ul>
                  <li v-for="(value, key) in item" :key="key">
                    <strong>{{ key }}:</strong> {{ value }}
                  </li>
                </ul>
              </div>
              </span>
              <span v-else>{{ item }}</span>
            </li>
          </ul>
        </span>
            <span v-else>{{ value }}</span>
          </li>
        </ul>
      </div>
    </div>
  </div>
</template>
