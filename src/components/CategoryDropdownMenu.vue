<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import type { TreeSelectChild, TreeSelectItem } from 'vant'

interface CategoryLeaf {
  id: string
  text: string
  desc?: string
}

interface CategoryNode {
  id: string
  text: string
  trend?: string
  children: CategoryLeaf[]
}

interface CategoryGroup {
  id: string
  text: string
  trend?: string
  children: CategoryNode[]
}

interface CategorySelection {
  top?: CategoryGroup
  middle?: CategoryNode
  leaf?: CategoryLeaf
}

const props = defineProps<{
  categories?: CategoryGroup[]
}>()

const emit = defineEmits<{
  (e: 'change', payload: CategorySelection): void
}>()

const fallbackCategories: CategoryGroup[] = [
  {
    id: 'smart-home',
    text: '智能家居',
    trend: 'HOT',
    children: [
      {
        id: 'voice',
        text: '语音助手',
        trend: '新品',
        children: [
          { id: 'mini-speaker', text: '小体积音箱', desc: '桌面/床头更协调' },
          { id: 'soundbar', text: '家庭影院声吧', desc: '3D环绕声体验' },
          { id: 'ai-remote', text: 'AI 场景遥控', desc: '一键联动多设备' }
        ]
      },
      {
        id: 'security',
        text: '安防监控',
        children: [
          { id: 'door-lock', text: '全自动门锁', desc: '免压把自动上锁' },
          { id: 'camera', text: '云台云存储摄像头' },
          { id: 'video-doorbell', text: '视频门铃', desc: '远程可视对讲' }
        ]
      },
      {
        id: 'environment',
        text: '空气环境',
        children: [
          { id: 'air-purifier', text: '空气净化器 Pro' },
          { id: 'humidifier', text: '无雾加湿器' },
          { id: 'monitor', text: 'CO₂/甲醛监测仪' }
        ]
      }
    ]
  },
  {
    id: 'digital',
    text: '数码影音',
    children: [
      {
        id: 'mobile',
        text: '手机通讯',
        trend: '热销',
        children: [
          { id: 'flagship-phone', text: '旗舰影像手机' },
          { id: 'foldable-phone', text: '折叠屏系列' },
          { id: 'gaming-phone', text: '电竞手机' }
        ]
      },
      {
        id: 'laptop',
        text: '轻薄本',
        children: [
          { id: 'ultra-slim', text: '1kg 轻薄本' },
          { id: 'creator', text: '创作本', desc: '4K 真彩屏' },
          { id: 'office', text: '商用办公本' }
        ]
      },
      {
        id: 'accessories',
        text: '配件周边',
        children: [
          { id: 'buds', text: '降噪耳机' },
          { id: 'watch', text: '全天候手表' },
          { id: 'power-bank', text: '超级快充电源' }
        ]
      }
    ]
  },
  {
    id: 'lifestyle',
    text: '品质生活',
    children: [
      {
        id: 'kitchen',
        text: '厨房小家电',
        children: [
          { id: 'steam-oven', text: '蒸烤炸一体机' },
          { id: 'smart-pot', text: '多段控温电饭煲' },
          { id: 'coffee', text: '全自动咖啡机' }
        ]
      },
      {
        id: 'cleaning',
        text: '清洁收纳',
        children: [
          { id: 'vacuum', text: '扫拖一体机器人' },
          { id: 'washer', text: '洗地机', desc: '自清洁/烘干' },
          { id: 'closet', text: '智能晾衣架' }
        ]
      },
      {
        id: 'health',
        text: '健康个护',
        children: [
          { id: 'massage', text: '筋膜枪/按摩椅' },
          { id: 'sleep', text: '助眠眼罩' },
          { id: 'care', text: '智能牙刷' }
        ]
      }
    ]
  }
]

const dropdownVisible = ref(false)
const activeTopIndex = ref(0)
const activeNavIndex = ref(0)
const activeLeafId = ref<string | number | undefined>(undefined)
const selectedPath = ref<CategorySelection>({})

const categories = computed<CategoryGroup[]>(() =>
  props.categories?.length ? props.categories : fallbackCategories
)

const currentTop = computed<CategoryGroup | undefined>(() => categories.value[activeTopIndex.value])

interface CategoryTreeLeaf extends TreeSelectChild {
  parentKey: string
  desc?: string
}

interface CategoryTreeItem extends TreeSelectItem {
  children?: CategoryTreeLeaf[]
}

const treeItems = computed<CategoryTreeItem[]>(() => {
  const top = currentTop.value
  if (!top) return []

  return top.children.map((middle) => ({
    text: middle.text,
    id: middle.id,
    badge: middle.trend,
    children: middle.children.map<CategoryTreeLeaf>((leaf) => ({
      text: leaf.text,
      id: leaf.id,
      parentKey: middle.id,
      desc: leaf.desc
    }))
  })) as CategoryTreeItem[]
})

const titleText = computed(() => {
  if (!selectedPath.value.top || !selectedPath.value.middle || !selectedPath.value.leaf) {
    return '请选择分类'
  }

  return `${selectedPath.value.top.text} / ${selectedPath.value.middle.text} / ${selectedPath.value.leaf.text}`
})

const breadcrumbText = computed(() => {
  if (!selectedPath.value.top || !selectedPath.value.middle || !selectedPath.value.leaf) {
    return '暂未选择'
  }
  return `${selectedPath.value.top.text} / ${selectedPath.value.middle.text} / ${selectedPath.value.leaf.text}`
})

watch(
  () => categories.value.length,
  (len) => {
    if (activeTopIndex.value >= len) {
      activeTopIndex.value = 0
    }
  }
)

watch(
  () => currentTop.value,
  (top) => {
    if (top && activeNavIndex.value >= top.children.length) {
      activeNavIndex.value = 0
    }
  }
)

const handleTopChange = (index: number) => {
  activeTopIndex.value = index
  activeNavIndex.value = 0
}

const handleNavChange = (index: number) => {
  activeNavIndex.value = index
}

const handleLeafSelect = (item: CategoryTreeLeaf) => {
  if (!currentTop.value) return
  const middle = currentTop.value.children.find((node) => node.id === item.parentKey)
  const leaf = middle?.children.find((child) => child.id === item.id)
  if (!middle || !leaf) return

  selectedPath.value = {
    top: currentTop.value,
    middle,
    leaf
  }
  activeLeafId.value = item.id as string
  dropdownVisible.value = false

  emit('change', selectedPath.value)
}

const resetSelection = () => {
  selectedPath.value = {}
  activeLeafId.value = undefined
}
</script>

<template>
  <van-dropdown-menu active-color="#1989fa">
    <van-dropdown-item
      v-model:show="dropdownVisible"
      :title="titleText"
      class="category-dropdown"
    >
      <div class="category-panel">
        <van-sidebar
          class="category-panel__primary"
          v-model="activeTopIndex"
          @change="handleTopChange"
        >
          <van-sidebar-item
            v-for="group in categories"
            :key="group.id"
            :title="group.text"
          >
            <template #title>
              <div class="primary-label">
                <span>{{ group.text }}</span>
                <span v-if="group.trend" class="primary-label__tag">{{ group.trend }}</span>
              </div>
            </template>
          </van-sidebar-item>
        </van-sidebar>

        <div class="category-panel__secondary">
          <van-tree-select
            :items="treeItems"
            :main-active-index="activeNavIndex"
            :active-id="activeLeafId"
            :height="320"
            @click-nav="handleNavChange"
            @click-item="handleLeafSelect"
          >
            <template #item="{ item }">
              <div class="leaf-item">
                <div class="leaf-item__title">{{ item.text }}</div>
                <div v-if="item.desc" class="leaf-item__desc">{{ item.desc }}</div>
              </div>
            </template>
          </van-tree-select>
          <p class="helper-text">提示：先定一级类目，再选择二级/三级即可完成</p>
        </div>
      </div>

      <div class="category-selected" v-if="selectedPath.leaf">
        <span class="label">已选</span>
        <span class="value">{{ breadcrumbText }}</span>
        <button class="clear-btn" type="button" @click="resetSelection">重置</button>
      </div>
      <div class="category-empty" v-else>
        暂未选择分类
      </div>
    </van-dropdown-item>
  </van-dropdown-menu>
</template>

<style scoped>
.category-dropdown :deep(.van-dropdown-item__content) {
  padding: 0;
}

.category-panel {
  display: flex;
  min-height: 320px;
}

.category-panel__primary {
  flex: 0 0 120px;
  border-right: 1px solid #f0f0f0;
}

.category-panel__secondary {
  flex: 1;
  padding: 12px;
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.primary-label {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 4px;
  font-weight: 500;
}

.primary-label__tag {
  font-size: 10px;
  color: #ee0a24;
  background-color: #ffece8;
  border-radius: 8px;
  padding: 0 6px;
}

.leaf-item {
  padding: 8px;
}

.leaf-item__title {
  font-size: 14px;
  color: #323233;
}

.leaf-item__desc {
  font-size: 12px;
  color: #969799;
  margin-top: 4px;
}

.helper-text {
  margin: 0;
  font-size: 12px;
  color: #969799;
  text-align: center;
}

.category-selected,
.category-empty {
  border-top: 1px solid #f2f3f5;
  padding: 12px 16px;
  font-size: 14px;
  display: flex;
  align-items: center;
  gap: 8px;
}

.category-selected .label {
  color: #969799;
}

.category-selected .value {
  flex: 1;
  color: #323233;
}

.clear-btn {
  border: none;
  background: #f2f3f5;
  border-radius: 99px;
  padding: 4px 12px;
  font-size: 12px;
  color: #576b95;
}

.clear-btn:active {
  opacity: 0.8;
}
</style>
