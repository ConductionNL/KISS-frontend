// we export these from here so we can combine asynchronous imports with tree shaking, because
// we don't want to import the entire ckeditor (huge) and
// we don't want to import it all the time (rarely used wtihin the application)
// so we cant implement it as documented with .use(CKEditor) in the main.ts
// we want to import it asynchronously so the code gets split into a seperate file, downloaded on demand.
// but tree shaking, to keep it small, is not supported with an async import
// therefore we first import the bits we need in here and do an async import of this file wherever we need the editor
// this vite improvement might make this workaround obsolete: https://github.com/vitejs/vite/pull/14221
export { Ckeditor } from "@ckeditor/ckeditor5-vue";

export {
  ClassicEditor,
  Essentials,
  Autoformat,
  Bold,
  Italic,
  BlockQuote,
  Heading,
  Link,
  List,
  Paragraph,
  Table,
  TableToolbar,
  type EditorConfig,
} from "ckeditor5";
