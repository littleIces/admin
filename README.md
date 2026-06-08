# Ant Design Vue 4.x - Cascader 与 Select 弹出框高度统一方案

## 问题描述

在 Ant Design Vue 4.x 中，`Select` 下拉弹出框默认最大高度为 **256px**（`listHeight` 属性），
而 `Cascader` 级联选择器每列弹出面板默认最大高度为 **180px**。两者不一致会导致视觉体验不统一。

## 解决方案

### 方案一：CSS 全局覆盖

在全局样式文件中添加：

```css
.ant-cascader-menu {
  max-height: 256px !important;
}
```

### 方案二：通过 `popupClassName` 局部覆盖（推荐）

给需要调整的 Cascader 组件设置 `popupClassName`，再通过该类名定义样式：

```vue
<a-cascader
  popup-class-name="cascader-select-consistent"
  :options="options"
/>
```

```css
.cascader-select-consistent .ant-cascader-menu {
  max-height: 256px;
}
```

### 方案三：通过 `dropdownMenuColumnStyle` 属性（内联方式）

```vue
<a-cascader
  :dropdown-menu-column-style="{ maxHeight: '256px' }"
  :options="options"
/>
```

## 方案对比

| 方案 | 优点 | 缺点 |
|------|------|------|
| 全局 CSS | 一次设置，全局生效 | 影响所有 Cascader 实例 |
| popupClassName | 局部控制，不影响其他实例 | 需要额外添加 prop |
| dropdownMenuColumnStyle | 纯 props 控制，无需写 CSS | 每个实例都要单独设置 |

## 注意事项

1. Cascader 弹出层默认渲染到 `document.body`，如果使用 `scoped` 样式需要用 `:deep()` 穿透，或者使用单独的非 scoped style 块
2. 如果 Select 修改了 `listHeight`，Cascader 的 max-height 也应同步调整
3. 在 Ant Design Vue 4.x 中，`dropdownClassName` 已废弃，统一使用 `popupClassName`
