<script setup lang="ts">
  import axios from '@/shared/axios'
  import type { AxiosResponse } from 'axios'
  import { onMounted, ref } from 'vue'
  import LoadSpinner from '@/components/LoadSpinner.vue'
  import dayjs from 'dayjs'
  import VueJsonPretty from 'vue-json-pretty'
  import 'vue-json-pretty/lib/styles.css'

  interface Negotiate {
    Url: string
  }

  interface Result {
    Index: number,
    Status: string,
    CreatedAt: string,
    Result: any
  }

  const props = defineProps<{
    id: string
  }>()

  const results = ref<Result[]>([])
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
          const data: Result = JSON.parse(event.data)

          results.value.push(data)
        }
      })

    const example = [
      {
        Index: 1249,
        Status: "succeeded",
        CreatedAt: "2023-06-30T09:01:35.7198207Z",
        Result: {
          predictions: [
            {
              label: 'car',
              confidence: 0.531231,
              boundingBox: {
                x1: 92,
                x2: 241,
                y1: 123,
                y2: 491,
                width: 200,
                height: 502
              }
            }
          ]
        }
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

    // results.value = duplicate(example, 500)
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
      <div>
        <ul class="flex flex-col space-y-2 mt-5">
          <li
              v-for="(value, key) in results.reverse()" :key="key"
              class="bg-backgroundDark p-5 rounded-[3px] "
          >
            <div class="flex justify-between">
              <strong>
                #{{ value.Index }}
              </strong>
              <p class="text-textGrey">
                {{ dayjs(value.CreatedAt).format('DD-MM-YYYY HH:mm:ss.SSS') }}
              </p>
            </div>
            <div class="bg-background p-2 rounded-[3px] text-textGrey text-sm mt-2">
              <VueJsonPretty :data="value.Result" :show-double-quotes="false" :show-line="false" />
            </div>
          </li>
        </ul>
      </div>
    </div>
  </div>
</template>
