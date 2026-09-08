# TareasMVC — Gestor de tareas en ASP.NET Core

Aplicación web para organizar tareas con autenticación, adjuntos en la nube y soporte multi-idioma.

## ✨ Características

- **Autenticación** con ASP.NET Core Identity y **login externo con cuenta Microsoft**.
- CRUD de tareas con **reordenamiento** y pasos/subtareas.
- **Archivos adjuntos** almacenados en **Azure Blob Storage**.
- **Internacionalización (i18n)** con recursos localizados.
- Migraciones de base de datos con EF Core.

## 🛠️ Tecnologías

- **ASP.NET Core MVC** (.NET 7)
- **Entity Framework Core** + Migrations sobre **SQL Server**
- **ASP.NET Core Identity** + Microsoft Account
- **Azure.Storage.Blobs** · **AutoMapper**

## 🚀 Puesta en marcha

```bash
git clone https://github.com/dotJManuel/TareasMVC.git
cd TareasMVC/TareasMVC
```

1. Configura la cadena de conexión a SQL Server y los secretos (Azure Blob y credenciales de Microsoft OAuth). Se recomienda **user-secrets**:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<tu-cadena>"
```

2. Aplica las migraciones y ejecuta:

```bash
dotnet restore
dotnet ef database update
dotnet run
```
