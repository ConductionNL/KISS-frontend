import { fileURLToPath, URL } from "url";
import { defineConfig, type ProxyOptions, loadEnv } from "vite";
import vue from "@vitejs/plugin-vue";
import basicSsl from "@vitejs/plugin-basic-ssl";
import { createRequire } from "node:module";
//import ckeditor5 from "@ckeditor/vite-plugin-ckeditor5";
const require = createRequire(import.meta.url);

const proxyCalls = [
  "/api",
  "/signin-oidc",
  "/signout-callback-oidc",
  "/healthz",
];

const getProxy = (
  env?: Record<string, string>,
): Record<string, ProxyOptions> | undefined => {
  const targetPort = env?.BFF_SSL_PORT;
  if (!targetPort) return undefined;
  const redirectOpts: ProxyOptions = {
    target: `http://localhost:${targetPort}`,
    secure: false,
    headers: {
      "x-forwarded-proto": "https",
    },
  };
  return Object.fromEntries(proxyCalls.map((key) => [key, redirectOpts]));
};

// https://vitejs.dev/config/
export default defineConfig(({ mode }) => {
  const env =
    mode === "development" ? loadEnv(mode, process.cwd(), "") : undefined;
  const proxy = env && getProxy(env);
  return {
    plugins: [vue(), basicSsl()],
    server: {
      port: 3000,
      proxy,
    },
    resolve: {
      alias: {
        "@": fileURLToPath(new URL("./src", import.meta.url)),
      },
    },
    build: {
      assetsInlineLimit: 0,
    },
    test: {
      coverage: {
        all: true,
      },
    },
    css: {
      preprocessorOptions: {
        scss: {
          // resolves deprecation warnings from dart-sass
          api: "modern",
        },
      },
    },
  };
});
