# KKF Inventory Management System

ระบบจัดการสินค้าและคำสั่งซื้อ พัฒนาขึ้นสำหรับการทดสอบ Programmer ของ KKF โดยแบ่งการทำงานออกเป็น 3 ส่วนหลัก ได้แก่ Frontend, Backend API และ Database

## 1. เทคโนโลยีที่ใช้

| ส่วน | เทคโนโลยี |
|---|---|
| Frontend | Vue 3, Vite, TypeScript |
| UI | PrimeVue 4, PrimeIcons |
| HTTP Client | Axios |
| Routing | Vue Router |
| Backend | ASP.NET Core Minimal API, .NET 10, C# |
| ORM | Entity Framework Core 10 |
| API Documentation | OpenAPI, Scalar |
| Database | Microsoft SQL Server |
| Backend Tests | xUnit, Moq, FluentAssertions |
| Frontend Tests | Vitest, Vue Test Utils |

## 2. โครงสร้างโปรเจค

```text
InventoryManagementSystem/
├── backend/
│   ├── src/
│   │   ├── Inventory.Api/             # API endpoints และจุดเริ่มต้นของระบบ
│   │   ├── Inventory.Application/     # Services, DTOs และ Abstractions
│   │   ├── Inventory.Domain/          # Domain entities
│   │   └── Inventory.Infrastructure/  # DbContext, Entity mappings และ Repositories
│   ├── tests/
│   │   ├── Inventory.UnitTests/       # Unit tests
│   │   └── Inventory.IntegrationTests/# Integration tests
│   ├── Inventory.slnx
│   └── dotnet-tools.json
│
├── frontend/
│   ├── src/
│   │   ├── components/                # Vue components ที่ใช้ร่วมกัน
│   │   ├── router/                    # Vue Router
│   │   ├── services/                  # ติดต่อ Backend API
│   │   ├── types/                     # TypeScript types
│   │   ├── utils/                     # ฟังก์ชันช่วยเหลือ
│   │   └── views/                     # หน้าจอของระบบ
│   ├── .env.development               # URL ของ Backend สำหรับ Development
│   ├── package.json
│   └── package-lock.json
│
├── database/
│   ├── 00_CreateDatabase.sql          # สร้างฐานข้อมูล KKF
│   ├── 01_CreateTables.sql            # สร้างตารางที่เกี่ยวข้อง
│   ├── 02_SeedGoods.sql               # ข้อมูลสินค้าเริ่มต้นสำหรับทดสอบ
│   └── KIG-ER-Diagram.pdf             # ER Diagram
│
└── README.md
```

## 3. สิ่งที่ต้องติดตั้งก่อนใช้งาน

- **.NET 10 SDK** สำหรับรัน Backend
- **Node.js** เวอร์ชัน 22.18 ขึ้นไปในสาย 22.x หรือ 24.12 ขึ้นไป
- **npm** (ติดตั้งมาพร้อม Node.js)
- **Microsoft SQL Server**
- **SQL Server Management Studio (SSMS)** หรือเครื่องมือสำหรับจัดการ SQL Server

ตรวจสอบเวอร์ชันที่ติดตั้งได้ด้วยคำสั่ง:

```bash
dotnet --version
node --version
npm --version
```

## 4. การเตรียมฐานข้อมูล

ไฟล์ SQL อยู่ในโฟลเดอร์ `database/` ให้เปิดและรันผ่าน SSMS ตามลำดับดังนี้

1. **`00_CreateDatabase.sql`** — สร้างฐานข้อมูล `KKF` หากยังไม่มีฐานข้อมูลนี้
2. **ตรวจสอบตาราง `CUS_NAME`** — ตามรายละเอียดโจทย์ ตารางลูกค้าและข้อมูลทดสอบจะจัดเตรียมโดย IT KIG ต้องมีตาราง `KKF.dbo.CUS_NAME` ก่อนรันสคริปต์ถัดไป
3. **`01_CreateTables.sql`** — เลือกฐานข้อมูล `KKF` แล้วรันเพื่อสร้างตาราง `GOODS_NAME`, `H_ORDER`, `D_ORDER` และ `M_ORDER`
4. **`02_SeedGoods.sql`** — (ทางเลือก) เพิ่มข้อมูลสินค้าตัวอย่างลงใน `GOODS_NAME` สำหรับทดสอบในเครื่อง ให้เลือกฐานข้อมูล `KKF` ก่อนรัน และรันเพียงครั้งเดียว เพราะหากมีข้อมูลรหัสเดียวกันอยู่แล้วจะเกิด Primary Key ซ้ำ


## 5. การตั้งค่า Backend

Backend อ่าน Connection String จากไฟล์:

`backend/src/Inventory.Api/appsettings.json`

ค่าตัวอย่างที่เตรียมไว้ใช้ SQL Server Express และ Windows Authentication:

```json
"ConnectionStrings": {
  "InventoryDb": "Server=localhost\\SQLEXPRESS;Database=KKF;Integrated Security=True;TrustServerCertificate=True;"
}
```

หากเครื่องของผู้ใช้งานใช้ SQL Server คนละ Instance ให้แก้เฉพาะส่วน `Server` ให้ตรงกับเครื่อง เช่น:

```json
"InventoryDb": "Server=.;Database=KKF;Integrated Security=True;TrustServerCertificate=True;"
```

แนวทางปรับค่า:

- `Server` — ชื่อเครื่องและ Instance ของ SQL Server เช่น `localhost`, `.`, `localhost\\SQLEXPRESS` หรือชื่อ Instance ที่ติดตั้งจริง
- `Database` — ชื่อฐานข้อมูล ในโปรเจคนี้ใช้ `KKF`
- `Integrated Security=True` — ใช้ Windows Authentication โดยบัญชี Windows ที่รัน Backend ต้องมีสิทธิ์เข้าถึงฐานข้อมูล
- หากใช้ SQL Server Authentication ให้เปลี่ยนเป็น `User ID` และ `Password` ตามบัญชีที่ได้รับ เช่น `Server=localhost;Database=KKF;User ID=<username>;Password=<password>;TrustServerCertificate=True;` และไม่ควรบันทึกรหัสผ่านจริงลงใน Git หรือไฟล์ที่ส่งต่อสาธารณะ

ตรวจสอบชื่อ Server/Instance และสิทธิ์การเข้าถึงฐานข้อมูลให้ถูกต้องก่อนรัน Backend

## 6. การตั้งค่า Frontend

มีไฟล์ `frontend/.env.development` เตรียมไว้แล้ว โดยกำหนด URL ของ Backend API เป็น:

```env
VITE_API_BASE_URL=http://localhost:5031/api
```

ค่านี้สอดคล้องกับ Backend ที่รันด้วย HTTP profile บนพอร์ต `5031` และ API routes ที่ขึ้นต้นด้วย `/api` หากมีการเปลี่ยน URL หรือพอร์ตของ Backend ให้แก้ค่าในไฟล์นี้ตามจริง จากนั้นเริ่ม Vite ใหม่เพื่อให้โหลดค่าการตั้งค่าอีกครั้ง

## 7. วิธีรันระบบ

ให้เปิด Terminal แยก 2 หน้าต่าง โดยรัน Backend และ Frontend แยกกัน

### 7.1 รัน Backend

จากโฟลเดอร์หลักของโปรเจค:

```bash
cd backend
dotnet restore
dotnet build Inventory.slnx
dotnet run --project src/Inventory.Api --launch-profile http
```

เมื่อเริ่มทำงานแล้ว Backend จะให้บริการที่:

| รายการ | URL |
|---|---|
| API Base URL | `http://localhost:5031/api` |
| Scalar API Reference | `http://localhost:5031/scalar/v1` |
| OpenAPI Document | `http://localhost:5031/openapi/v1.json` |
| Health Check | `http://localhost:5031/api/health/` |
| Database Health Check | `http://localhost:5031/api/health/database` |

Scalar และ OpenAPI เปิดใช้งานเมื่อรันใน Development environment ซึ่งกำหนดไว้ใน HTTP launch profile ที่เตรียมให้แล้ว

### 7.2 รัน Frontend

เปิด Terminal อีกหน้าต่าง จากโฟลเดอร์หลักของโปรเจค:

```bash
cd frontend
npm ci
npm run dev
```

จากนั้นเปิด URL ที่ Vite แสดงใน Terminal โดยค่าปกติคือ:

`http://localhost:5173`

ตรวจสอบให้แน่ใจว่า Backend ทำงานอยู่ และค่า `VITE_API_BASE_URL` ใน `frontend/.env.development` ตรงกับ URL ของ Backend

## 8. คำสั่ง Build และทดสอบ

### Backend

รันจากโฟลเดอร์ `backend/`:

```bash
dotnet build Inventory.slnx --configuration Release
dotnet test Inventory.slnx
```

### Frontend

รันจากโฟลเดอร์ `frontend/`:

```bash
npm run build
npm run test:unit -- --run
```

คำสั่ง `npm run build` จะตรวจสอบ TypeScript และ Build ไฟล์สำหรับ Production ส่วนคำสั่งทดสอบจะรัน Vitest แบบครั้งเดียว ไม่เข้าสู่ Watch mode

## 9. การแก้ปัญหาเบื้องต้น

- **Backend เชื่อมต่อ SQL Server ไม่ได้:** ตรวจสอบว่า SQL Server Service ทำงานอยู่, ชื่อ Server/Instance ถูกต้อง, Connection String ถูกต้อง และบัญชีที่ใช้มีสิทธิ์เข้าถึงฐานข้อมูล `KKF`
- **ไม่พบตารางหรือแจ้งว่าไม่มี `CUS_NAME`:** ตรวจสอบว่าเลือกฐานข้อมูล `KKF` และมีตาราง `CUS_NAME` ก่อนรัน `01_CreateTables.sql`
- **Frontend ติดต่อ API ไม่ได้:** ตรวจสอบว่า Backend ทำงานบน `http://localhost:5031` และค่า `VITE_API_BASE_URL` ใน `frontend/.env.development` เป็น `http://localhost:5031/api` รวมถึงตรวจสอบว่า Frontend ทำงานบนพอร์ต `5173` ซึ่งเป็น Origin ที่ Backend อนุญาตผ่าน CORS ในปัจจุบัน
- **เปิด Scalar หรือ OpenAPI ไม่ได้:** ให้รัน Backend ด้วยคำสั่งที่ใช้ `--launch-profile http` ตามตัวอย่างด้านบน
