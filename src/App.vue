<script setup lang="ts">
import { ref } from 'vue'
import CategoryDropdownMenu from './components/CategoryDropdownMenu.vue'

const selectionText = ref('暂未选择分类')

const handleChange = (selection: { top?: { text: string }; middle?: { text: string }; leaf?: { text: string } }) => {
  if (!selection.top || !selection.middle || !selection.leaf) {
    selectionText.value = '暂未选择分类'
    return
  }
  selectionText.value = `${selection.top.text} / ${selection.middle.text} / ${selection.leaf.text}`
}
</script>

<template>
  <main class="page">
    <section class="hero">
      <p class="hero__eyebrow">Vue 3 · Vant 4</p>
      <h1>DropdownMenu 三级类目筛选</h1>
      <p class="hero__subtitle">
        下拉后弹出 Sidebar + TreeSelect 组合，实现“一级侧边 + 二级导航 + 三级叶子”体验，适合商品类目、城市区域等复杂选择场景。
      </p>
    </section>

    <section class="demo-card">
      <CategoryDropdownMenu @change="handleChange" />
      <div class="result-card">
        <p class="result-card__title">实时选择结果</p>
        <p class="result-card__value">{{ selectionText }}</p>
      </div>
    </section>
  </main>
</template>

<style scoped>
.page {
  min-height: 100vh;
  padding: 56px 16px 80px;
  max-width: 960px;
  margin: 0 auto;
  display: flex;
  flex-direction: column;
  gap: 32px;
}

.hero__eyebrow {
  color: #576b95;
  font-size: 14px;
  letter-spacing: 0.08em;
  text-transform: uppercase;
  margin-bottom: 8px;
}

.hero h1 {
  margin: 0;
  font-size: clamp(26px, 6vw, 40px);
  color: #1d1d1f;
}

.hero__subtitle {
  margin-top: 12px;
  max-width: 640px;
  color: #4a4a4a;
  line-height: 1.6;
}

.demo-card {
  background: #ffffff;
  border-radius: 24px;
  box-shadow: 0 20px 60px rgba(20, 43, 93, 0.08);
  padding: 32px;
  display: flex;
  flex-direction: column;
  gap: 24px;
}

.result-card {
  background: #f5f7ff;
  border-radius: 16px;
  padding: 16px 20px;
}

.result-card__title {
  margin: 0 0 6px;
  font-size: 14px;
  color: #86909c;
}

.result-card__value {
  margin: 0;
  font-size: 16px;
  font-weight: 600;
  color: #1d2129;
}

@media (max-width: 640px) {
  .demo-card {
    padding: 20px;
  }
}
</style>
