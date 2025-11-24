import { createApp } from 'vue'
import { ConfigProvider, DropdownItem, DropdownMenu, Sidebar, SidebarItem, TreeSelect } from 'vant'
import 'vant/lib/index.css'
import './style.css'
import App from './App.vue'

const app = createApp(App)

app.use(ConfigProvider)
app.use(DropdownMenu)
app.use(DropdownItem)
app.use(Sidebar)
app.use(SidebarItem)
app.use(TreeSelect)

app.mount('#app')
