<script setup lang="ts">
  import TableRow from '@/components/TableRow.vue'
  import LoadSpinner from '@/components/LoadSpinner.vue'

  defineProps<{
    columns: string[],
    rows: any[],
    isFetching: boolean
  }>()

  const emit = defineEmits(['updateRow', 'deleteRow'])

  const updateRow = (updated: any) => {
    emit('updateRow', updated)
  }

  const deleteRow = (deleted: any) => {
    emit('deleteRow', deleted)
  }
</script>

<template>
  <div class="overflow-x-auto relative">
    <table class="w-full text-left text-text min-h-[13rem]">
      <thead class="bg-backgroundDark border-y border-y-line text-sm">
        <tr>
          <th scope="col" class="py-5 px-6">
            #
          </th>
          <th v-for="col in columns" v-bind:key="col" scope="col" class="py-5 px-6">
            {{ col }}
          </th>
          <th scope="col" class="py-5 px-6 text-right relative">
            <span class="sr-only">
              Actions
            </span>
          </th>
        </tr>
      </thead>
      <div v-if="isFetching"
           class="py-10 absolute right-[50%] left-[50%]"
      >
        <LoadSpinner />
      </div>
      <tbody v-else class="text-textDark w-full">
      <TableRow
          v-for="row in rows"
          v-bind:key="row.id"
          :row-data="row"
          @update="updateRow"
          @delete="deleteRow"
      />
      </tbody>
    </table>
  </div>
</template>

<style scoped>

</style>
