# ASPNet Core/ ASPNet Core MVC

En este repo voy a ir metiendo todos los conceptos que he ido asimilando al ver temas de ASPNet Core y ASPNet Core MVC.

Todas estas notas de abajo las tengo que ir organizando y desarrollando.

# Conceptos básicos

Para saber las versiones tanto del SDK como del runtime tenemos varios comandos:

```
dotnet --info       (SDK + Runtime)
dotnet --version    (SDK)
```

Para indicar la plataforma de destino de una aplicación .NET Core se especifica en el archivo del proyecto, mediante monikers de la plataforma de destino (TFM - Target framework moniker).
```
<PropertyGroup>
	<TargetFramework>netcoreapp2.0</TargetFramework>
</PropertyGroup>
```

En general podemos hablar que existen dos dos tipos de despliegue y se debe indicar en el csproj:
* **FDD. Framework-dependent deployments**. Heredado de full framework, es decir, 
desplegamos sólo la aplicacióny las dependencias y no es necesario desplegar .NET Core, ya que la aplicación utilizará la versión de .NET Core presente en el sistema de destino. Este es el modelo de implementación predeterminado para las aplicaciones .NET Core.
* **SCD. Self-contained deployments.** Desplegamos la aplicación y las dependencias de terceros requeridas así como la versión de .NET Core utilizada para construir la aplicación.

```
dotnet publish -r win7-x64
    Sin -r hace FDD 
    Con -r hace SCD
```

> Concepto Huella de memoria (memory foot print). Revisar

> **Proxy inverso**. Meter algo delante de kestrel (IIS, nginx) y cuando hablamos de un entorno de producción debemos asumir que Kestrel no es apto sin un proxy inverso.

# Introducción

## Compartir código

* **PCL**. 
  * Class library/ eliges el target al principio.
* **Shared projects**. 
  * Directivas de compilador para ejecutar condicionalmente según plataforma dónde corre
* **.Net Standard**.    
  * Es la evolución de PCL, sigue habiendo una librería portable pero con otro enfoque.    
    * Su numeracion es lineal no como PCL.    
    * No hay breaking changes (NETStandar 1.6 es compatible con 1.5, 1.4, etc...)
    * .Net Standar es una especificación, no una implementación.
    * Concepto Open-ended.
    * .NET portability analyzer (tool VS20017).

> Por lo que se puede decir que Net Core 2.0 implementa Net Standar 2.0

El pipeline de una aplicación de .Net Core es el siguiente:
* Host - Entorno de ejecución
* Server
* Middleware
* Framework (terminal)
    * Por ejemplo el de MVC
* Application

## Net Core
* Clase **Startup**.cs
  * Es la clase que utilizamos para configurar nuestra aplicación.
  * Método **ConfigureServices**.
    * Método para configurar nuestros y hay que recordar que los servicios en terminología de Core son las dependencias, por lo que el ConfigureServices es donde registro mis dependencias.
        * Es opcional.
        * Sólo se inyecta el IServiceCollection
  * Método **Configure**.         
    * Es el método dónde se configura el pipeline de ejecución.
    * Se ejecuta después de ConfigureServices.
	* Sólo se inyecta IApplicationBuilder, IHostingEnvironment, etc...

> ¿Qué está inyectado por defecto?
    En ctor. IHostingEnvironment, ILoggerFactory, IConfiguration

* **Inyección de dependencias** - DI.
  * Es inyección de dependencias de Net Core y no de ASPNet Core, puede ser usado por ejemplo en una consola
        http://panicoenlaxbox.blogspot.com.es/2017/11/crear-e-inicializar-un-contexto-en-ef.html
  * Ciclos de vida:
    * **AddTransient**
    * **AddSingleton**
    > Las clases estáticas son para métodos, no para mantener estado.
    * **AddScoped**
* Notas
  * Por defecto, una app siempre está en producción
  * Métodos Configure y ConfigureService por environment, incluida la clase Startup
  * Antes teníamos un global.asax, ahora tenemos en Configure la posibilidad de inyectar IApplicationLifetime

* **Configuración del Host**
  * En Program.cs se configura el host    
  * UseUrl    
  * UserEnvironment

* **Configuración**
  * Antes teníamos System.Configuration (obsoleta, no instanciable, api compleja)
  * Ahora es acceso clave/ valor
  * Streamed configuration. Es vital el orden que le das a los proveedores de Configuración, para leer de estos ficheros la ruta debe ser completa, y el sepador es : Revisar y buscar ejemplo
  * Tipado. Permite al resto de la aplicación (fuera de Startup) acceder por DI a la configuración a través de addOptions en ConfigureServices.
   > Siempre debemos ir a tipado.
  * Refresco. Sólo funciona con la configuración no tipada. El problema lo resuelve IOptionsSnapshot<T>
  * Secretos de usuarios.
    * Es un origen/ proveedor de datos más que sólo está en tu máquina para elementos de configuración que respresentan elementos de seguridad.
    * Sólo afecta a desarrollo.
    * Desde VS. Botón derecho -> Manage user secret.
    * Si vas con VSVCode dotnet user-secrets para administrarlo desde comandos.

* **Logging**
	* Configure (ILoggerFactory)
    * Tipos        
      * Consola        
      * Debug. Va al output window.        
      * Crear personalizados.
      * **Serilog**.
        * Permite **Structured logging**, que es la capacidad de manejar no como strings sino como datos/ objetos.
        * Seq. Consumir Structured logging.
        * Enrichers. Enriquecedores.
            * Thread.
            * Process.
            * Environment.
        * Sinks. Dónde guardaremos los datos.            
          * RollingFile.
          * ColoredConsole.
          * MSSqlServer.
          * Slack.
    * Filtrar nivel de traza.        
      * LoggerFactory .WithFilter en Configure como LogLevel.None        
      * Podemos filtrar por código o por appsettings.json    
      * Para filtrar una request por petición, tenemos dentro de AddConsole(includeScopes: true)
    * Scopes.	
      * Interesante. Agrupar logging. 

* **Caching**
    * InMemomyCaching.         
      * Memoria de la máquina.        
      * Cache aside.
    * IDistributedCaching.        
      * Caché sincronizada entre varios servidores.         
      * SQL Server/ Redis.

* **Middlewares**.
    * Es un concepto que se introdujo en Owin.
    * **Son componentes que forma un pipeline entre el servidor y nuestra aplicación**.
    * Se procesan en orden.
    ![Middleware pipeline](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/_static/request-delegate-pipeline.png)
    * Middleware pass throw Revisar concepto
    * Ojo! Tiene que acabar en un *Run siempre*, ojo! o un *Use sin hacer invoke del siguiente*.
    * Tipos de Middleares
      * **Use**
        * Response.HasStarted. Es útil para indicar si las cabeceras han sido enviadas o el body ha sido escrito. Si es true, sólo es válido llamar a Respone.WriteAsync y no se puede por otro lado modificar la cabeceras.
      * **Run**
        * Es un Middleware terminal.	
      * **Map**
        * Divide el pipeline en ramas o branches en función de la ruta de petición.
      * **MapWhen**        
        * Es map pero con una condición, hacer una rama su se evalua cierto el predicado
    * Clase.
	```
    public class CustomMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestResponseLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
           await _next(context);            
        }
    }
	```
    * Ciclo de vida. Sólo se crea una vez durante la inicialización del pipeline.
    * Es importante la posición del Middleware, ejemplo claro con Compression (ahora ASPNet realiza la compresión así no dependemos del IIS) antes o después por ejemplo de los estáticos.
    * Existen una serie de Middleware ya creados que son los llamados **Built-in middleware**
      * **Authentication**. Provides authentication support.        
      * **CORS**. Configures Cross-Origin Resource Sharing.        
      * **Response Caching**. Provides support for caching responses.        
      * **Response Compression**. Provides support for compressing responses.        
      * **Routing**. Defines and constrains request routes.        
        * No es exclusivo de MVC, es ASPNet Core.
      * **Session**.Provides support for managing user sessions.        
      * **Static Files**. Provides support for serving static files and directory browsing.
      * **URL Rewriting Middleware**. Provides support for rewriting URLs and redirecting requests.
        * Mapear las URL de peticiones entrantes a las URL que internamente utiliza la aplicación.    
        * Por ejemplo forzar el tráfico Https, en vez de configurarlo en IIS.	
      * **Session**. ASPNet Session, por defecto viene sin sesión porque sesión es un middleware (services.AddSession(), app.UseSession()).
      * **MVC**. Es otro middleware de ASPNet Core, es decir services.AddMvc()/ app.UseMvcWithDefaultRoute()
    
# ASPNet MVC 

## Controladores
* Normalmente heredan de Controller.
* Normalmente devuelve a IActionResult aunque podemos devolver un string o un objeto y él hará el wrapper.
* Pueden ser clases POCO (La convención es que acabe en el palabra Controller).
* El atributo *ActionName* es para especificar el nombre de la acción.
* El atributo *NonAction*. Que no se puede alcanzar por ruta/ ¿Entonces no sería un método privado?.
* Selección de acción con atributos como antes y se aprovecha para la ruta HttpGet("baz")
* Rutas.
  * Iremos por defecto.    
  * Constraints. int, bool, datetime, decimal, double, float... Aunque podemos crear las nuestras propias con IRouteConstraint.
  * Atribute data routing.
    * [Route("MiHome")] a nivel de controlador.
    * Con / en la ruta de la acción sobreescribe y no hereda de Route.
    * Otro ejemplo de Route es para separar en Route las rutas de api [Route("api/[controller]")].
* Inyección de dependencias (DI) por método/ acción mediante el uso del atributo [FromServices].
* **IActionResult**.  Si devolvemos algo distinto IActionResult será serializado a JSON y devuelto con content-type "application/json" excepto string.
  * Existen una serie de tipos predefinidos que son: ContentResult (text/plain), EmptyResult, FileResult, StatusCodeResult, OkResult, NoContent, BadRequestResult, UnauthorizedResult, ViewResult, PartialViewResult, JsonResult
* Razor. Ahora las vistas con asíncronas y se le pueden inyectar dependencias con @inject.

> Las operaciones CPU Bound (cálculos) son síncronas (sync)

> Las operaciones IO Bound (base de datos, ficheros) son asíncronas (async)

* **Binding**. Antes de seguir conviene rescatar lo siguiente:
  
> Los valores que vienen de URL usan Culture Insensitive.

> Los valores que vienen por post llevan cultura.

  * *Binding en MV5*. 
    * *Value providers*. Query string, route values, headers, body. Son extensibles, leer datos de distintas partes de la petición.
            MVC itera sober todos los value providers, por lo que el último léido es el prevalecido.
	* *Model binders*. 
      * Rellenan los parámetros de la acción desde el sitio común donde los value providers han dejado los datos.            
      * Desconocen de dónde han venido los datos.            
      * Por cada parámetro de una acción sólo puedo haber un model binder.
   * *Binding en WebApi 2*.
     * *Parámetros simples*. Values providers.        
     * *Parámetros complejos*.             
       * Media-type formatter                
       * Body. 
         * A diferencia de los value providers devuelve el dato ya enlazado.
         * El body es leído por un media type formatter y sólo puede ser leído una vez.
         * Sólo un parámetro puede ser leído desde el body.
         * Con el atributo [FromBody] permite leer el parámetro simple desde el body.
         * Con el atributo [FromUrl] permite leer un parámetro complejo con un value provider, no leer del body.
    * **Binding en Core**. Por defecto es el binding de **MVC5**, es decir,  "a lo MVC".
      * Leer desde body si el content type es application/x.www....
      * No entiende de JSON.
      * Atributos. A lo MVC5. Aplicables tanto a parámetros como a propiedades de un ViewModel.                
        * FromForm.                
        * FromQuery.
        * FromRoute.
        * FromHeader.                                                
        * FromBody. 
          * A lo WebApi con media formatter, sólo un parámetro puede leer del body.
        * JSON. 
          * No hay ningún value provider que lea JSON (A lo MVC).
          * Si hay un media-type formatter que entiende JSON (FromBody).
      * ModelBinder.
        * Enlazar un tipo con datos con una representación concreta.
        * IModelBinder. ¿Cómo asocio un parámetro de con un IModelBinder? Con IModelBinderProvider.
      * InputFormatter.
            A lo webapi, sólo a través de body
            IInputFormatter
        Negociación de contenido
            Negociar dos partes, yo servidor tengo que poder ofrecer 
            En Core MVC la negociación de contenido se realizar a través de IActionResult
            Algunos ActionResult tien negociación de contenido y otros no   
                ViewResult. Siempre html
                JSONResult. Siemrpe JSON
                ContentResult. Siempre texto plano
                OkObjectResult. Negociación de contenido
                NotFoundObjectResult. Negociación de contenido
            Para hacer uno personalizados
                IOutputFormatter
        Produces
            Filtro de aplicaciónLimita que output formatters se pueden tener en consideración
            Ignorar cabecera accept
        Output formatters predefenidos
            HttpContentOutputFormatter
            StringOutputFormatter
            StreamOutputFormatter
            JsonOutputFormatter
        ¿Cuándo un filtro o cuando un Middleware?
            Un filtro para un tema específico de MVC
        Es decir, y no hay que olvidar que tenemos dos pipelines, el de Core y el específico de MVC
        Como cortocircuitar en un filtro
            Devolver un IActionResult en context.Result
        Pipeline
            ActionExecution Ver imagen en Google
                Authorization filters
                    Antes de ejecutar la aplicación
                    ¿Tiene autorización el usuario para acceder al recurso?
                    No lanzar excepciones puesto que no el filtro de expceción no las manajeara.
                Resource filters
                    Antes y después de ejecutar la acción.
                Model binding
                Action filters
                    Antes y después de ejecutar la aplicación
                Exception filters
                Result filters
                    Sólo si la acción ah sido ejecutada con éxito
                    Surround
                Result execution
        Se pueden heradar de los filtross builtin
            AuthorizationFilter
            ResultFilterAttribute
            ActionFilterAttribute
            ExceptionFilterAttribute
        DI
            No hay DI por constructor en los filtros
            ServiceFilterAtrtribute.
                Lo recupera desde el contendor de dependencias.
            TypeFilterAttribute
                Le podemos pasar parámetros
            Usar un Middleware como filtro  
                Es un guarrería, matar moscas a cañonazos
        De serie
            Authorize
            AllowAnonymous
            RequireHttps
            RequireCache