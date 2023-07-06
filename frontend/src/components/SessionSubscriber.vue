<script lang="js">
  import axios from '@/shared/axios'
  import { computed, onMounted, reactive, ref, toRef, unref } from 'vue'
  import LoadSpinner from '@/components/LoadSpinner.vue'
  import dayjs from 'dayjs'
  import VueJsonPretty from 'vue-json-pretty'
  import 'vue-json-pretty/lib/styles.css'
  import { DynamicScroller, DynamicScrollerItem } from 'vue-virtual-scroller'
  import 'vue-virtual-scroller/dist/vue-virtual-scroller.css'

  // This component is JS as opposed to TS sin

  export default {
    components: { LoadSpinner, DynamicScroller, DynamicScrollerItem, VueJsonPretty },
    methods: { dayjs },
    props: {
      id: String
    },
    setup(props) {
      const results = ref([])
      const isConnecting = ref(true)

      const reversedResults = computed(() => {
        return unref(results).slice().reverse();
      });

      onMounted(async () => {
        const response = await axios().post(`session/${props.id}/negotiate`);
        const ws = new WebSocket(response.data.Url);

        ws.onopen = () => {
          console.log('Connected!');
          isConnecting.value = false;
        };

        ws.onmessage = (event) => {
          const data = JSON.parse(event.data);

          const rawResults = unref(results);
          if (rawResults.length >= 50) {
            rawResults.shift();
          }

          rawResults.push(data);

          results.value = [...rawResults];
        };
      });

      return {
        isConnecting,
        results: reversedResults
      }
    }
  }
</script>

<template>
  <div>
    <div v-if="isConnecting"
         class="py-10 absolute right-[50%] left-[50%]"
    >
      <LoadSpinner />
    </div>
    <DynamicScroller
        v-if="results.length"
        :items="results"
        key-field="Index"
        min-item-size="5"
        class="mt-5"
    >
      <template v-slot="{ item, index, active }">
        <DynamicScrollerItem
          :item="item"
          :active="active"
          :size-dependencies="[item.Result]"
          :data-index="index"
          class="mb-5"
        >
          <div class="bg-backgroundDark p-5 rounded-[3px]">
            <div class="flex justify-between">
              <strong>
                #{{ item.Index }}
              </strong>
              <p class="text-textGrey">
                {{ dayjs(item.CreatedAt).format('DD-MM-YYYY HH:mm:ss.SSS') }}
              </p>
            </div>
            <div class="bg-background p-2 rounded-[3px] text-textGrey text-sm mt-2">
              <VueJsonPretty :data="item.Result" :show-double-quotes="false" :show-line="false" />
            </div>
          </div>
        </DynamicScrollerItem>
      </template>
    </DynamicScroller>
  </div>
</template>
