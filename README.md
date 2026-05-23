# Sistema de Control de Farmacia - Backend (ASP.NET Core Web API)

Este repositorio contiene la lógica de negocio, servicios de persistencia y la API REST para el **Sistema de Control de Farmacia**. La aplicación está diseñada bajo una arquitectura desacoplada basada en controladores, utilizando **.NET 8** y **Entity Framework Core** para interactuar de forma eficiente con una base de datos **MySQL**. El sistema cuenta con soporte completo para contenedorización mediante Docker.

---

## Características del Proyecto
- **Framework:** ASP.NET Core Web API (.NET 8.0).
- **Arquitectura:** API REST basada en controladores, inyección de dependencias y separación de responsabilidades.
- **Acceso a Datos:** Entity Framework Core (Enfoque Code-First / Migrations).
- **Base de Datos:** MySQL.
- **Autenticación y Seguridad:** Autenticación basada en Bearer Tokens JWT (JSON Web Tokens).
- **Contenerización:** Configuración lista para Docker y Docker Compose para entornos reproducibles.

---

## Requisitos Previos

El proyecto fue desarrollado y probado con el siguiente entorno local:
- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MySQL (v8.0 o superior)](https://www.mysql.com/) o Docker Desktop activo.
- [Docker & Docker Compose](https://www.docker.com/)
- [Git](https://git-scm.com/)

---

## Configuración de Variables de Entorno (`.env`)

El proyecto utiliza variables de entorno para gestionar dinámicamente las credenciales críticas, la configuración JWT y las cadenas de conexión a la base de datos sin exponer datos sensibles en el código fuente.

### Estructura de Referencia (`.env.example`)
Crea un archivo llamado `.env` en la raíz del proyecto y completa los valores basándote en esta plantilla:

```env
SQL_PASSWORD=poner_aqui_una_clave_segura
JWT_KEY=generar_una_clave_de_32_caracteres_o_mas
ASPNETCORE_ENVIRONMENT=Development
DB_CONNECTION=Server=mysql_db;Port=3306;Database=db_farmacia;Uid=root;Pwd=poner_aqui_la_clave;
```

## Instalación y Ejecución

1. Clonar el repositorio

```bash
git clone <URL_DEL_REPOSITORIO>
```

2. Ingresar al directorio del proyecto

```bash
cd Control_Farmacia_Backend
```

3. Restaurar paquetes NuGet

```bash
dotnet restore
```

4. Aplicar Migraciones de Entity Framework (Generar Base de Datos):

Asegúrate de tener instalada la herramienta de EF Core globalmente

```bash
dotnet tool install -g dotnet-ef
```

```bash
dotnet ef database update
```

5. Ejecutar la API

```bash
dotnet run
```

La API estará disponible en:

```txt
https://localhost:5001
```

## Ejecución con Docker Compose

El proyecto incluye un archivo `docker-compose.yml` para compilar la API e inicializar el servidor de MySQL de forma automática y aislada dentro de contenedores:

```bash
docker-compose up --build -d
```

Verificar que los servicios estén corriendo:

```bash
docker ps
```

Detener el entorno de contenedores:

```bash
docker-compose down
```
