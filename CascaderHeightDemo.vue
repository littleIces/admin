<template>
  <div class="demo-container">
    <h3>Select 下拉框（默认 max-height: 256px）</h3>
    <a-select
      style="width: 200px"
      placeholder="请选择"
      :options="selectOptions"
    />

    <h3>Cascader 级联选择器（调整后 max-height: 256px）</h3>

    <!-- 方案一：使用 popupClassName 添加自定义类名 -->
    <a-cascader
      style="width: 300px"
      :options="cascaderOptions"
      placeholder="请选择"
      popup-class-name="cascader-select-consistent"
    />

    <!-- 方案二：使用 dropdownMenuColumnStyle 内联样式控制 -->
    <a-cascader
      style="width: 300px"
      :options="cascaderOptions"
      placeholder="请选择（内联方式）"
      :dropdown-menu-column-style="{ maxHeight: '256px' }"
    />
  </div>
</template>

<script setup lang="ts">
const selectOptions = Array.from({ length: 30 }, (_, i) => ({
  label: `选项 ${i + 1}`,
  value: i + 1,
}));

const cascaderOptions = [
  {
    value: 'zhejiang',
    label: '浙江',
    children: Array.from({ length: 20 }, (_, i) => ({
      label: `城市 ${i + 1}`,
      value: `city-${i + 1}`,
      children: Array.from({ length: 15 }, (_, j) => ({
        label: `区域 ${j + 1}`,
        value: `area-${j + 1}`,
      })),
    })),
  },
  {
    value: 'jiangsu',
    label: '江苏',
    children: Array.from({ length: 20 }, (_, i) => ({
      label: `城市 ${i + 1}`,
      value: `city-${i + 1}`,
    })),
  },
];
</script>

<style>
/**
 * 通过 popupClassName="cascader-select-consistent" 局部作用
 * 将 Cascader 弹出面板每列最大高度设为 256px，与 Select 保持一致
 */
.cascader-select-consistent .ant-cascader-menu {
  max-height: 256px;
}

.demo-container {
  padding: 24px;
}

.demo-container h3 {
  margin: 16px 0 8px;
}
</style>
