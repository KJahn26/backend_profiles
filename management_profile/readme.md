# ⚙️ Configuración del Proyecto — `appsettings.json`

Este archivo contiene la configuración principal de la aplicación ASP.NET Core. A continuación se describen cada una de las secciones.

---

## 📋 Tabla de Contenidos

- [Logging](#logging)
- [AllowedHosts](#allowedhosts)
- [ConnectionStrings](#connectionstrings)
- [EmailSettings](#emailsettings)

---

## 🪵 Logging

Controla el nivel de detalle de los registros de la aplicación.

| Clave | Valor | Descripción |
|---|---|---|
| `Default` | `Information` | Nivel de log por defecto para toda la aplicación |
| `Microsoft.AspNetCore` | `Warning` | Solo registra advertencias y errores del framework ASP.NET Core |

**Niveles disponibles** (de menor a mayor severidad):
`Trace` → `Debug` → `Information` → `Warning` → `Error` → `Critical`

---

## 🌐 AllowedHosts

```json
"AllowedHosts": "*"
```

Permite solicitudes desde **cualquier host**. En producción se recomienda restringir este valor a los dominios específicos de la aplicación, por ejemplo:

```json
"AllowedHosts": "midominio.com;www.midominio.com"
```

---

## 🗄️ ConnectionStrings

Cadena de conexión hacia la base de datos **PostgreSQL**.

| Parámetro | Valor | Descripción |
|---|---|---|
| `Host` | `postgres` | Nombre del host o contenedor de la base de datos |
| `Port` | `5432` | Puerto estándar de PostgreSQL |
| `Database` | `profiles_db` | Nombre de la base de datos |
| `Username` | `user_db` | Usuario de acceso |
| `Password` | `1234` | Contraseña del usuario |

> ⚠️ **Advertencia de seguridad:** No expongas credenciales reales en el repositorio. Usa variables de entorno o un gestor de secretos (como [dotnet user-secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) o **Azure Key Vault**) en entornos de producción.

---

## 📧 EmailSettings

Configuración del servicio de envío de correos electrónicos.

| Parámetro | Valor | Descripción |
|---|---|---|
| `Email` | `jahn8916@gmail.com` | Dirección de correo remitente |
| `Password` | `xajr sfgm nzwg xepl` | Contraseña de aplicación de Gmail (App Password) |

> 💡 **Nota:** El valor de `Password` corresponde a una **contraseña de aplicación** de Google (App Password), necesaria cuando se tiene habilitada la verificación en dos pasos en la cuenta de Gmail.

> ⚠️ **Advertencia de seguridad:** Nunca subas contraseñas reales a un repositorio público. Considera almacenar este valor en variables de entorno:
> ```bash
> export EmailSettings__Password="tu_password_aqui"
> ```

---

## 🚀 Configuración para distintos entornos

ASP.NET Core permite sobrescribir valores por entorno usando archivos adicionales:

| Archivo | Entorno |
|---|---|
| `appsettings.json` | Base / compartido |
| `appsettings.Development.json` | Desarrollo local |
| `appsettings.Production.json` | Producción |

---

## 🔒 Buenas prácticas de seguridad

1. Agrega `appsettings*.json` al `.gitignore` si contiene credenciales reales.
2. Usa **variables de entorno** para sobreescribir valores sensibles en producción.
3. Considera usar **Docker secrets** o un **vault** si el proyecto corre en contenedores.