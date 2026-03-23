# Background Job Processing

基于 ASP.NET Core 8.0 的后台任务处理系统，使用 `Channel<T>` 实现内存任务队列，支持并发处理、失败重试和优先级调度。

## 架构

- **JobQueue** — 基于 `Channel<T>` + `ConcurrentDictionary` 的线程安全任务队列
- **JobProcessorService** — `BackgroundService` 实现，支持可配置的并发数和指数退避重试
- **IJobHandler** — 任务处理器接口，通过 DI 注册不同类型的处理器

## 内置任务类型

| 类型 | 说明 |
|------|------|
| `send-email` | 模拟发送邮件 |
| `data-export` | 模拟数据导出 |
| `generate-report` | 模拟报告生成（带进度） |

## API

### 提交任务

```
POST /api/jobs
Content-Type: application/json

{
  "type": "send-email",
  "payload": "{\"to\":\"user@example.com\",\"subject\":\"Hello\",\"body\":\"World\"}",
  "priority": 1,
  "maxRetries": 3
}
```

### 查询任务

```
GET /api/jobs/{id}
```

### 列出任务（可选按状态过滤）

```
GET /api/jobs?status=Completed
```

### 取消任务

```
DELETE /api/jobs/{id}
```

### 队列统计

```
GET /api/jobs/stats
```

## 配置

在 `appsettings.json` 中配置最大并发数：

```json
{
  "JobProcessing": {
    "MaxConcurrency": 4
  }
}
```

## 运行

```bash
cd BackgroundJobProcessing
dotnet run
```

默认监听 `http://localhost:5000`，Swagger UI 可通过 `/swagger` 访问。

## 扩展

实现 `IJobHandler` 接口并注册到 DI 容器即可添加新的任务类型：

```csharp
public class MyJobHandler : IJobHandler
{
    public string JobType => "my-job";

    public async Task<string> HandleAsync(string? payload, CancellationToken ct)
    {
        // 处理逻辑
        return "done";
    }
}

// Program.cs
builder.Services.AddScoped<IJobHandler, MyJobHandler>();
```
