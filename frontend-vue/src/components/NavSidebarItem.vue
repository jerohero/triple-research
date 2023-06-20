<script setup lang="ts">
  import { ref } from 'vue'
  import type { INavSidebarItem } from '@/shared/interfaces'

  const props = defineProps<{
    item: INavSidebarItem,
    activeRoute?: string,
    isFoldable?: boolean
  }>()

  const isFolded = ref<boolean>(false)

  const onClick = () => {
    if (props.item.cb)
      props.item.cb()
    if (props.isFoldable)
      isFolded.value = !isFolded.value
  }
</script>

<template>
  <div class="select-none">
    <div class="group flex py-3.5 cursor-pointer pl-10 hover:bg-secondaryHover transition-colors ease-in duration-100"
         :class="props.activeRoute === props.item.href && !isFoldable ? 'bg-secondaryActive' : ''"
         @click="onClick"
    >
      <div class="mr-5">
        <ion-icon :name="item.icon" class="text-xl group-hover:animate-wiggle pointer-events-none"/>
      </div>
      <p>
        {{ item.name }}
      </p>
    </div>
    <div v-if="isFoldable && item.subItems && !isFolded" class="">
      <ul class="list-disc">
        <RouterLink :to="subItem.href"
                    v-for="subItem in item.subItems"
                    v-bind:key="subItem.name"
                    class="text-sm hover:bg-secondaryHover pl-20 py-2 text-textGrey flex flex-col
                      transition-colors ease-in duration-100 animate-fadeIn"
                    :class="props.activeRoute === subItem.href ? 'bg-secondaryActive text-text' : ''"
        >
          <li>
            {{ subItem.name }}
          </li>
        </RouterLink>
      </ul>
    </div>
  </div>
</template>

<style scoped>

</style>
