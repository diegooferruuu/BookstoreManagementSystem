# Módulo de Reportes de Ventas

Este módulo implementa un sistema completo de reportes de ventas utilizando arquitectura hexagonal y el patrón Builder para la generación de reportes en PDF y Excel.

## 🏗️ Estructura del Proyecto

### ServiceSales (Arquitectura Hexagonal)

```
ServiceSales/
├── Domain/                          # Capa de Dominio (Núcleo)
│   ├── Interfaces/
│   │   ├── ISaleRepository.cs       # Puerto: Repositorio de ventas
│   │   └── ISalesReportService.cs   # Puerto: Servicio de reportes
│   └── Models/
│       ├── Sale.cs                  # Entidad: Venta
│       ├── SaleDetail.cs            # Entidad: Detalle de venta
│       ├── SaleReportFilter.cs      # DTO: Filtros para reportes
│       └── SaleReportData.cs        # DTO: Datos del reporte
│
├── Application/                     # Capa de Aplicación
│   └── Services/
│       └── SalesReportService.cs    # Servicio de generación de reportes
│
└── Infrastructure/                  # Capa de Infraestructura
    └── Repositories/
        └── SaleRepository.cs        # Implementación del repositorio
```

## 🔗 Integración con LibraryWeb

### Páginas Razor

```
LibraryWeb/Pages/Sales/Reports/
├── Index.cshtml                     # Vista de reportes
└── Index.cshtml.cs                  # Lógica de la página
```

### Menú de Navegación

Se agregó una nueva opción en el menú principal:
- **Ventas** > **Reportes**

## 📊 Características

### Filtros de Reportes

1. **Por Usuario (Vendedor)**: Filtra ventas por el usuario que realizó la venta
2. **Por Cliente**: Filtra ventas por cliente específico
3. **Por Fecha Inicial**: Filtra desde una fecha específica
4. **Por Fecha Final**: Filtra hasta una fecha específica

### Formatos de Salida

- **PDF**: Reporte profesional con tablas y totales
- **Excel**: Hoja de cálculo con formato y totales

### Datos Incluidos en el Reporte

- Fecha de venta
- Nombre del cliente
- Nombre del vendedor
- Método de pago
- Total de la venta
- Estado de la venta
- Notas adicionales
- **Total general** y cantidad de ventas

## 🗄️ Esquema de Base de Datos

### Tabla: sales

```sql
Column         | Type                     | Description
---------------|--------------------------|-------------
id             | uuid                     | ID único de la venta
client_id      | uuid                     | ID del cliente (FK)
user_id        | uuid                     | ID del usuario/vendedor (FK)
sale_date      | timestamp with time zone | Fecha de la venta
total          | numeric(10,2)            | Total de la venta
payment_method | varchar(50)              | Método de pago
status         | varchar(20)              | Estado de la venta
notes          | text                     | Notas opcionales
created_at     | timestamp with time zone | Fecha de creación
```

### Relaciones

- `sales.client_id` → `clients.id`
- `sales.user_id` → `users.id`
- `sale_details.sale_id` → `sales.id` (CASCADE)

## 🚀 Uso

### 1. Acceso a la Página de Reportes

1. Iniciar sesión como Admin o Employee
2. Navegar a **Ventas** > **Reportes**

### 2. Configurar Filtros (Opcional)

- Seleccionar usuario vendedor (opcional)
- Seleccionar cliente (opcional)
- Establecer rango de fechas (opcional)

### 3. Generar Reporte

- Clic en **Generar PDF** para reporte en PDF
- Clic en **Generar Excel** para reporte en Excel

El archivo se descargará automáticamente con el formato:
```
Reporte_Ventas_YYYYMMDD_HHMMSS.pdf
Reporte_Ventas_YYYYMMDD_HHMMSS.xlsx
```

## 💻 Código de Ejemplo

### Uso desde Code-Behind

```csharp
// Inyectar servicios
public IndexModel(
    ISalesReportService salesReportService,
    IClientService clientService,
    IUserService userService)
{
    _salesReportService = salesReportService;
    _clientService = clientService;
    _userService = userService;
}

// Generar reporte
var filter = new SaleReportFilter
{
    UserId = Guid.Parse("..."),
    ClientId = Guid.Parse("..."),
    StartDate = new DateTime(2025, 1, 1),
    EndDate = new DateTime(2025, 12, 31)
};

var reportBytes = await _salesReportService.GenerateSalesReportAsync(
    filter, 
    "pdf", 
    cancellationToken
);
```

### Uso desde API (Futuro)

```csharp
app.MapPost("/api/sales/reports", async (
    ISalesReportService service,
    SaleReportFilter filter,
    string format) =>
{
    var bytes = await service.GenerateSalesReportAsync(filter, format);
    var contentType = await service.GetReportContentType(format);
    var extension = await service.GetReportFileExtension(format);
    
    return Results.File(bytes, contentType, $"report{extension}");
});
```

## 🔧 Configuración en Program.cs

```csharp
// Registrar servicios de Sales
builder.Services.AddSingleton<ISaleRepository, SaleRepository>();
builder.Services.AddSingleton<ISalesReportService, SalesReportService>();
```

## 📦 Dependencias

El módulo depende de:

1. **ServiceCommon**: Para servicios de reportes (Builder Pattern)
2. **ServiceUsers**: Para obtener información de usuarios/vendedores
3. **ServiceClients**: Para obtener información de clientes
4. **Npgsql**: Para acceso a PostgreSQL

## 🎨 Diseño de la Interfaz

- **Bootstrap 5**: Framework CSS
- **Font Awesome**: Iconos
- **Cards**: Organización visual
- **Formularios**: Selectores y filtros
- **Botones grandes**: Acciones principales destacadas
- **Alertas**: Mensajes de error e información

## 🔒 Seguridad y Permisos

- Requiere autenticación
- Disponible para roles: **Admin** y **Employee**
- Uso de autenticación basada en cookies

## 📈 Flujo de Generación de Reportes

```
Usuario selecciona filtros
         ↓
IndexModel.OnPostGenerate[Pdf|Excel]Async()
         ↓
SalesReportService.GenerateSalesReportAsync()
         ↓
SaleRepository.GetFilteredSalesAsync()
         ↓ (datos)
ReportBuilderFactory.CreateBuilder(type)
         ↓
ReportDirector.ConstructReport()
         ↓
[PdfReportService | ExcelReportService].GenerateReport()
         ↓
byte[] (archivo descargable)
```

## ✨ Ventajas de la Arquitectura

1. **Separación de responsabilidades**: Cada capa tiene una función clara
2. **Testeable**: Fácil de crear pruebas unitarias
3. **Extensible**: Agregar nuevos formatos sin modificar código existente
4. **Mantenible**: Código limpio y organizado
5. **Reutilizable**: Servicios compartidos con otros módulos

## 🔮 Posibles Extensiones

1. **Gráficos**: Agregar gráficos a los reportes PDF
2. **Más filtros**: Por producto, por rango de montos, etc.
3. **Reportes programados**: Envío automático por email
4. **Dashboard**: Visualización en tiempo real
5. **Exportar a CSV**: Formato adicional
6. **Reportes personalizados**: Selección de columnas

## 🐛 Troubleshooting

### Error: "No se encontraron ventas"
- Verificar que existan ventas en la base de datos
- Revisar los filtros aplicados

### Error al generar PDF/Excel
- Verificar que QuestPDF y ClosedXML estén instalados
- Revisar permisos de escritura de archivos

### Dropdown vacío
- Verificar que existan usuarios y clientes en la base de datos
- Revisar la conexión a la base de datos

## 📝 Notas de Desarrollo

- Los reportes se generan en memoria (no se guardan en disco)
- La descarga es inmediata al navegador
- Los filtros son opcionales (sin filtros = todos los registros)
- Se incluye validación de fechas en el frontend
