# Challenge Intuit / Yappa – API de Clientes (Backend)

Este repositorio contiene la solución **backend** para el challenge de Intuit / Yappa: una API REST para el ABM de clientes sobre una base de datos relacional.

> ⚠️ Alcance: este repo cubre **exclusivamente el backend** (API + acceso a datos + tests + SonarQube).  

---

## 1. Tecnologías y arquitectura

**Stack principal:**

- [.NET 8] – Web API
- C#
- PostgreSQL – motor relacional
- Npgsql – provider de acceso a datos para PostgreSQL
- SonarQube – análisis estático y cobertura

**Organización por capas / proyectos:**
> Arquitectura limpia orientada a la metodologia de desarrollo de software DDD (Driven Domain Design). 

- `Clientes.Domain`  
  - Entidades de dominio (`Cliente`).
  - Lógica de dominio simple asociada al modelo.

- `Clientes.Application`  
  - Capa de **aplicación** y orquestación.
  - Uso de patrón **CQRS** con **MediatR**:
    - Commands: crear, actualizar, eliminar clientes.
    - Queries: obtener todos, obtener por Id, buscar por nombre.
  - Validaciones y reglas de negocio de la aplicación.

- `Clientes.Infrastructure`  
  - Acceso a datos sobre PostgreSQL.
  - Repositorios:
    - `ClienteReadRepository`
    - `ClienteWriteRepository`
  - Clase `DatabaseSeeder` que inicializa la base con la estructura y datos de prueba.
  - Uso de SQL parametrizado y soporte para utilizar **stored procedures**.

- `Clientes.Api`  
  - Proyecto Web API.
  - Controlador REST principal: `ClientesController`.
  - Configuración de:
    - Inyección de dependencias,
    - MediatR,
    - Logging,
    - Swagger (documentación interactiva de la API).

- `Clientes.Tests`  
  - Proyecto de tests automatizados para todas las capas.
  - Cobertura total del proyecto **> 80%**.

---
## 
**Manejo de Errores:**
El manejo de errores se realiza mediante excepciones específicas (ClienteNotFoundException, ClienteConflictException) y logging con ILogger, permitiendo detectar condiciones de error y comunicar respuestas adecuadas desde los controladores.
## 

## 2. Base de datos

### 2.1. Esquema

Se utiliza PostgreSQL con una tabla `clientes` basada en el script provisto en el enunciado:

```
La inicialización de esquema + datos se realiza desde Clientes.Infrastructure mediante la clase DatabaseSeeder, que ejecuta el script al levantar la aplicación (si corresponde).

2.2. Conexión a la base de datos

Para desarrollar en local, se utilizan User Secrets de .NET (para no exponer credenciales reales en el repositorio):
El proyecto Clientes.Api tiene configurado un UserSecretsId en su .csproj.
En Program.cs se cargan los User Secrets cuando el entorno es Development.

Ejemplo de secrets.json (no se commitea al repo):
json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=challenge_intuit;Username=postgres;Password=PASSWORD_REAL"
  }
}

3. Ejecución del proyecto
3.1. Requisitos previos
.NET 8 SDK instalado
PostgreSQL instalado y corriendo (local o remoto)

3.2. Pasos para levantar la API
Clonar el repositorio

bash
Copiar código
git clone https://github.com/FedeTor/ChallengeBackendIntui.git
Crear la base de datos vacía en PostgreSQL

Ejemplo sql:
CREATE DATABASE challenge_intuit;
Configurar la cadena de conexión con User Secrets (por ejemplo Administrando secretos de usuarios desde el ide vs)
Levantar la API

3.3. Swagger
Una vez levantada la API, la documentación interactiva está disponible en:
https://localhost:7143/swagger
Desde Swagger se pueden probar todos los endpoints del ABM de clientes.
Tambien esta generada la coleccion de postman para poder importar el archivo y poder usarlo. Ruta src/Documentation.

4. Endpoints de la API
La API sigue un estilo REST clásico:

Base: /api/clientes

MétodoRutaDescripción
GET/api/clientes Obtiene todos los clientes
GET/api/clientes/{id} Obtiene un cliente por su ID
GET/api/clientes/searchBusca clientes por nombre
POST/api/clientes Crea un nuevo cliente
PUT/api/clientes/{id} Actualiza un cliente existente
DELETE/api/clientes/{id} Elimina un cliente

4.1. Ejemplo de búsqueda por nombre
http
Copiar código
GET /api/clientes/search?nombre=mar
Devuelve todos los clientes cuyo nombre contenga "mar" en sus caracteres centrales (búsqueda parcial).

5. Validaciones y reglas de negocio
5.1. Campos obligatorios
Para la creación de un cliente se validan como obligatorios:
nombre
apellido
razonSocial
cuit
telefonoCelular
email
fechaNacimiento

5.2. Formatos
Email: se valida que tenga formato de correo electrónico válido.
CUIT: se valida que tenga un formato correcto (por ejemplo, "XX-XXXXXXXX-X").
Fecha de nacimiento: se valida que sea una fecha válida y con un formato aceptado por la API (YYYY-MM-DD).

5.3. Unicidad
Se garantiza la unicidad a nivel de base de datos mediante constraints:
id → PRIMARY KEY
cuit → UNIQUE
email → UNIQUE
Adicionalmente, la capa de aplicación puede validar y devolver mensajes claros cuando se intenta insertar un cliente con cuit o email ya existente.

6. Manejo de errores y logging
La API utiliza logging de ASP.NET Core (ILogger) para registrar errores en:
Handlers de commands/queries.
Repositorios de acceso a datos.
Controlador de la API.
En caso de errores no controlados se devuelve un mensaje genérico, y el detalle queda logueado internamente.

7. Calidad de código – SonarQube
Se utiliza SonarQube para analizar:
Code smells (mantenibilidad),
Bugs (reliability),
Vulnerabilidades y hotspots de seguridad,
Cobertura de código.
El script de ejecución está en la raíz del repo:
powershell:
.\run-sonar.ps1 -Token "TOKEN"

8. Tests
El proyecto de tests está en:
tests/Clientes.Tests

Se cubren:
Handlers de commands y queries (capa Clientes.Application).
Repositorios (Clientes.Infrastructure).
Controlador (Clientes.Api).
Entidad de dominio Cliente y su comportamiento asociado (Clientes.Domain).

8.1. Cómo ejecutar los tests
Desde la raíz del repo:
bash: dotnet test

9.  Notas finales
Este proyecto fue desarrollado en el contexto del challenge técnico Intuit / Yappa, siguiendo las consignas:

Uso de motor relacional (PostgreSQL),
API RESTful para ABM de clientes,
Uso de al menos un stored procedure desde la capa de datos,
Documentación con Swagger,
Manejo de errores y logging,
Análisis de calidad con SonarQube,
Cobertura de tests ≥ 80%.

---

## 10. Ejecución con Docker

1. Copiar el archivo `.env.example` a `.env` y ajustar las variables (usuario, contraseña y nombre de la base de datos PostgreSQL, puertos, etc.).
2. Construir las imágenes:

   ```bash
   docker compose build
   ```

3. Levantar los servicios en segundo plano:

   ```bash
   docker compose up -d
   ```

   - La API quedará disponible en `http://localhost:${API_PORT:-8080}`.
   - PostgreSQL expone el puerto `${DB_PORT:-5432}` hacia el host.
   - La conexión a la base se inyecta mediante la variable `ConnectionStrings__DefaultConnection`, apuntando al servicio `db` dentro de la red interna de Docker.
   - El seeding inicial de la base se ejecuta automáticamente en el arranque gracias a `DatabaseSeeder`.
    volumes:
      - ./src:/src/src
      - ~/.nuget/packages:/root/.nuget/packages:ro
```

Luego ejecutar normalmente `docker compose up` para que los cambios en el código se reflejen sin reconstruir la imagen.
README.md content (end).
