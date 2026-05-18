# 电力交易员中级工实操考试中转平台

基于 **.NET 6** + **MySQL 8** 的考试中转服务，提供考试名单管理、多考试服务器考生同步、实操成绩回写、成绩导出及向 Java 第三方系统推送成绩。

## 功能概览

| 功能 | 说明 |
|------|------|
| 考试名单 | JSON / Excel 导入考生，按批次管理 |
| 考生同步 | 多台考试服务器通过 API 拉取待同步考生（单次最多 **200** 人）并确认 |
| 成绩回写 | 考试程序回写 SKILL/THEORY 成绩 |
| 成绩导出 | 管理端导出 Excel |
| Java 推送 | `POST /student/grade/callback/score`，RSA 公钥加密签名 |

## 快速启动（Docker）

```bash
docker compose up --build
```

- API: http://localhost:8080
- Swagger: http://localhost:8080/swagger
- 健康检查: http://localhost:8080/api/health

## 本地开发

```bash
# 需本地 MySQL 8，并修改 appsettings.json 连接串
dotnet run --project src/PowerTraderExam.Api
```

## 鉴权

所有业务接口（除健康检查）需在请求头携带：

```
X-Api-Key: <your-key>
```

| 角色 | 配置项 | 用途 |
|------|--------|------|
| 管理端 | `ApiKeys:Admin` | 名单导入、查询、导出、手动推送 |
| 考试服务器 | `ApiKeys:Servers:{serverId}` | 同步考生、回写成绩 |

默认开发密钥见 `src/PowerTraderExam.Api/appsettings.json`。

## 主要 API

### 管理端（Admin Key）

- `POST /api/batches` — 创建批次
- `POST /api/batches/{batchCode}/candidates` — JSON 导入名单
- `POST /api/batches/{batchCode}/candidates/import` — Excel 导入（列：准考证号、学员ID、姓名、身份证、单位）
- `GET /api/batches/{batchCode}/candidates` — 分页查询名单
- `GET /api/scores` — 分页查询成绩
- `GET /api/scores/export` — 导出 Excel
- `POST /api/scores/push` — 推送 Java（body 可选 `batchCode`、`subject`、`scoreIds`）

### 考试服务器（Server Key）

- `GET /api/sync/candidates?batchCode=&limit=` — 拉取待同步考生（`limit` 默认/上限 200）
- `POST /api/sync/candidates/confirm` — 确认已同步（body: `{ "candidateIds": [1,2] }`）
- `POST /api/scores` — 回写成绩

成绩回写示例：

```json
{
  "studentId": "123456",
  "subject": "SKILL",
  "situation": "NORMAL",
  "score": 85.5,
  "answerUrl": "https://example.com/answer/123456_skill.pdf"
}
```

## Java 成绩推送

配置 `appsettings.json` → `JavaPush`：

```json
{
  "JavaPush": {
    "BaseUrl": "http://java-host:8080",
    "PushPath": "/student/grade/callback/score",
    "PublicKeyPem": "-----BEGIN PUBLIC KEY-----\n...\n-----END PUBLIC KEY-----",
    "BatchSize": 50,
    "EnableBackgroundPush": true
  }
}
```

签名规则：

1. `plainText = <请求体原始 JSON 字符串> + <timestamp>`
2. `sign = Base64(RSA_Encrypt_PKCS1(plainText, 对方公钥))`
3. 请求头：`sign`、`timestamp`

推送体为 JSON 数组，字段：`studentId`、`subject`、`situation`、`score`、`answerUrl`。

## 项目结构

```
src/
  PowerTraderExam.Api/           # Web API
  PowerTraderExam.Application/   # DTO、接口、配置
  PowerTraderExam.Domain/        # 实体、枚举
  PowerTraderExam.Infrastructure/# EF Core、业务实现、Java 推送
```

## 许可证

内部项目，按需使用。
