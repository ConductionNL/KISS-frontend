<template>
  <ckeditor
    :editor="ClassicEditor"
    :config="config"
    v-model="modelValue"
    v-bind="$attrs"
  />
</template>

<script setup lang="ts">
import type { EditorConfig } from "./ckeditor-exports";
import { computed } from "vue";

const props = defineProps<{ modelValue?: string }>();
const emit = defineEmits<{ (e: "update:modelValue", v?: string): void }>();
const modelValue = computed({
  get: () => props.modelValue,
  set: (val) => emit("update:modelValue", val),
});

// we don't want to import directly from ckeditor
// see ./ckeditor-exports for an explanation of this workaround
const { ClassicEditor, Ckeditor, ...plugins } = await import(
  "./ckeditor-exports"
);

const config: EditorConfig = {
  plugins: Object.values(plugins),
  toolbar: [
    "heading",
    "|",
    "bold",
    "italic",
    "link",
    "bulletedList",
    "numberedList",
    "blockQuote",
    "undo",
    "redo",
    "insertTable",
  ],
  table: {
    defaultHeadings: { rows: 1 },
    contentToolbar: ["tableColumn", "tableRow"],
  },
  licenseKey: "GPL",
};
</script>

<style src="ckeditor5/ckeditor5.css"></style>
