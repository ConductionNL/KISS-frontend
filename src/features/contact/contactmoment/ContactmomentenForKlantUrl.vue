<template>
  <contactmomenten-overzicht
    v-if="contactmomenten?.page"
    :contactmomenten="contactmomenten.page"
  >
    <template v-for="(_, slotName) in $slots" #[slotName]="props">
      <slot :name="slotName" v-bind="props"></slot>
    </template>
  </contactmomenten-overzicht>
</template>
<script setup lang="ts">
import type { PaginatedResult } from "@/services";
import type { ContactmomentViewModel } from "@/services/openklant2";
import { useLoader } from "@/services/use-loader";
import { watchEffect } from "vue";
import ContactmomentenOverzicht from "./ContactmomentenOverzicht.vue";
import { fetchContactmomentenByKlantId } from "./service";

defineSlots();

const props = defineProps<{
  klantUrl: string;
  gebruikKlantInteracties: boolean;
}>();

const emit = defineEmits<{
  load: [data: PaginatedResult<ContactmomentViewModel>];
  loading: [data: boolean];
  error: [data: boolean];
}>();

const {
  data: contactmomenten,
  loading,
  error,
} = useLoader(() => {
  if (props.klantUrl)
    return fetchContactmomentenByKlantId(
      props.klantUrl,
      props.gebruikKlantInteracties,
    );
});

watchEffect(() => contactmomenten.value && emit("load", contactmomenten.value));
watchEffect(() => emit("loading", loading.value));
watchEffect(() => emit("error", error.value));
</script>
