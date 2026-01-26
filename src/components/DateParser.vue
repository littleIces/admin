<template>
  <div class="parser-container">
    <h2>字符串解析演示</h2>
    <div class="input-group">
      <label>输入字符串:</label>
      <input v-model="inputString" placeholder="例如: 第52周 (12.22-12.28) 截图" style="width: 300px; padding: 5px;" />
    </div>

    <div class="result-group" v-if="parsedResult">
      <h3>解析结果:</h3>
      <ul>
        <li><strong>周数:</strong> {{ parsedResult.week }}</li>
        <li><strong>开始日期:</strong> {{ parsedResult.startDate }}</li>
        <li><strong>结束日期:</strong> {{ parsedResult.endDate }}</li>
      </ul>
      <p>原始匹配: {{ parsedResult.match }}</p>
    </div>
    <div v-else class="no-match">
      <p>未匹配到有效格式。</p>
    </div>

    <div class="code-example">
      <h3>代码示例:</h3>
      <pre>
const regex = /第(\d+)周\s*\((\d{1,2}\.\d{1,2})\s*-\s*(\d{1,2}\.\d{1,2})\)/;
const match = str.match(regex);
if (match) {
  const week = match[1];
  const start = match[2];
  const end = match[3];
}
      </pre>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue';

const inputString = ref('第52周 (12.22-12.28) 截图');

const parsedResult = computed(() => {
  if (!inputString.value) return null;

  // 正则表达式解释:
  // 第(\d+)周   : 匹配 "第" + 数字(捕获组1) + "周"
  // \s*         : 匹配可能的空格
  // \(          : 匹配左括号
  // (\d{1,2}\.\d{1,2}) : 匹配日期格式如 12.22 (捕获组2)
  // \s*-\s*     : 匹配中间的连字符及可能的空格
  // (\d{1,2}\.\d{1,2}) : 匹配日期格式如 12.28 (捕获组3)
  // \)          : 匹配右括号
  const regex = /第(\d+)周\s*\((\d{1,2}\.\d{1,2})\s*-\s*(\d{1,2}\.\d{1,2})\)/;
  
  const match = inputString.value.match(regex);

  if (match) {
    return {
      week: match[1],
      startDate: match[2],
      endDate: match[3],
      match: match[0]
    };
  }
  return null;
});
</script>

<style scoped>
.parser-container {
  font-family: Arial, sans-serif;
  padding: 20px;
  border: 1px solid #ddd;
  border-radius: 8px;
  max-width: 600px;
  margin: 20px auto;
}
.input-group {
  margin-bottom: 20px;
}
.result-group {
  background-color: #f9f9f9;
  padding: 15px;
  border-radius: 4px;
}
.code-example {
  margin-top: 20px;
  background-color: #2d2d2d;
  color: #fff;
  padding: 15px;
  border-radius: 4px;
  overflow-x: auto;
}
.no-match {
  color: #666;
  font-style: italic;
}
</style>
