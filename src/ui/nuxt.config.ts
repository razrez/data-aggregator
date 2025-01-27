// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  css: [
    'element-plus/dist/index.css',
    '@/assets/styles/global.css'
  ],
  build: {
    transpile: ['element-plus/es', '@element-plus/icons-vue']
  },
  compatibilityDate: '2024-11-01',
  devtools: { enabled: true },
  modules: ['@element-plus/nuxt']
})
