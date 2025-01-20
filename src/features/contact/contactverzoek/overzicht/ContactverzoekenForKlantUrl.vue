<template>
  <contactverzoeken-overzicht
    v-if="contactverzoeken?.page"
    :contactverzoeken="contactverzoeken.page"
  >
    <template v-for="(_, slotName) in $slots" #[slotName]="props">
      <slot :name="slotName" v-bind="props"></slot>
    </template>
  </contactverzoeken-overzicht>
</template>

<script lang="ts" setup>
import type { PaginatedResult } from "@/services";
import { useLoader } from "@/services/use-loader";
import { watchEffect } from "vue";
import ContactverzoekenOverzicht from "./ContactverzoekenOverzicht.vue";
import { fetchContactverzoekenByKlantId } from "./service";
import type { ContactverzoekOverzichtItem } from "./types";

defineSlots();

const props = defineProps<{
  klantUrl: string;
  gebruikKlantInteracties: boolean;
}>();

const emit = defineEmits<{
  load: [data: PaginatedResult<ContactverzoekOverzichtItem>];
  loading: [data: boolean];
  error: [data: boolean];
}>();

const {
  data: contactverzoeken,
  loading,
  error,
} = useLoader(() => {
  if (props.klantUrl)
    return fetchContactverzoekenByKlantId(
      props.klantUrl,
      props.gebruikKlantInteracties,
    );
});

watchEffect(
  () => contactverzoeken.value && emit("load", contactverzoeken.value),
);
watchEffect(() => emit("loading", loading.value));
watchEffect(() => emit("error", error.value));
</script>
