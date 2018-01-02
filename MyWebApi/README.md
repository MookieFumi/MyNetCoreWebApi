# ASPNet Core/ ASPNet Core MVC

En este repo voy a ir metiendo todos los conceptos que he ido asimilando al ver temas de ASPNet Core y ASPNet Core MVC.

Todas estas notas de abajo las tengo que ir organizando y desarrollando.

https://docs.microsoft.com/en-us/aspnet/core/

# Conceptos básicos/ fundamentales

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

![Pipeline](https://c1.staticflickr.com/1/515/31564566283_cfa4105b3a_z.jpg)

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

Es un patrón que permite reducir el nivel de acoplamiento entre los objetos y sus colaboradores o dependencias y esto se consigue no creando directamente las instancias de esos colaboradores/ dependencias. *Normalmente estas dependencias serán recibidas por constructor*.

> El Principio de Inversión de Dependencia (DIP) establece que los módulos de alto nivel no deben depender de módulos de bajo nivel; ambos deben depender de abstracciones. Las abstracciones no deben depender de los detalles. Los detalles deben depender de las abstracciones.

  * Es inyección de dependencias de Net Core y no de ASPNet Core, puede ser usado por ejemplo en una consola
        http://panicoenlaxbox.blogspot.com.es/2017/11/crear-e-inicializar-un-contexto-en-ef.html
  * Ciclos de vida:
    * **AddTransient**
    * **AddSingleton**
    > Las clases estáticas son para métodos, no para mantener estado.
    * **AddScoped**

Podemos utilizar otro contenedor de dependencias que no sea el de serie, por ejemplo Autofaq...

```csharp
public IServiceProvider ConfigureServices(IServiceCollection services)
{
    var container = services.GetAutofacServiceProvider();
    return container;
}

public static IServiceProvider GetAutofacServiceProvider(this IServiceCollection services)
{
    var builder = new ContainerBuilder();

    //Services
    builder.Populate(services);

    //Register custom modules
    builder.RegisterModule<MainModule>();

    return new AutofacServiceProvider(builder.Build());
}
```

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

## Middlewares

https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware?tabs=aspnetcore2x

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

    public CustomMiddleware(RequestDelegate next)
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

Lo primero que tendríamos que definir son las responsabilidades que tienen los controladores, entre otras:
* Verify ModelState.IsValid.
* Return an error response if ModelState is invalid.
* Retrieve a business entity from persistence.
* Perform an action on the business entity.
* Save the business entity to persistence.
* Return an appropriate IActionResult.

Aspectos a destacar.

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
      * InputFormatter. A lo webapi, sólo a través de body (IInputFormatter).        
      * Negociación de contenido.
        * Negociar dos partes, yo servidor tengo que poder ofrecer . En ASPNet Core MVC la negociación de contenido se realiza a través de IActionResult.
        * Algunos ActionResult tien negociación de contenido y otros no:
          * ViewResult. Siempre html.
          * JSONResult. Siemrpe JSON.
          * ContentResult. Siempre texto plano.
          * OkObjectResult. Negociación de contenido.
          * NotFoundObjectResult. Negociación de contenido.
        * Para hacer uno personalizado (IOutputFormatter).
          * Produces.
            * Filtro de aplicación.
            * Limita que output formatters se pueden tener en consideración.
            * Ignorar cabecera accept.        
          * Output formatters predefenidos.
            * HttpContentOutputFormatter.
            * StringOutputFormatter.
            * StreamOutputFormatter.
            * JsonOutputFormatter

## Filtros

https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/filters

Los filtros en ASP.NET Core MVC le permiten ejecutar código antes o después de ciertas etapas del pipeline de ejecuición de la petición. Y existen una serie de filtros incorporados que manejan tareas tales como la autorización (impidiendo el acceso a los recursos para los cuales un usuario no está autorizado), asegurando que todas las solicitudes usen HTTPS y el caché de respuestas (cortocircuitando la línea de solicitud para devolver una respuesta en caché) aunque también tenemos la posibilidad de crear filtros personalizados para manejar aspectos transversales (cross-cutting) para su aplicación. Cada vez que desee evitar la duplicación de código en las acciones, los filtros son la solución. Por ejemplo, puede consolidar el código de manejo de errores en un filtro de excepción.

* **¿Cuándo un filtro o cuando un Middleware?**
    * Un filtro para un tema específico de MVC, no hay que olvidar que tenemos dos pipelines, el de Core y el específico de MVC.
    * Si quiero escribir respuesta un filtro.
    * Tipos de peticiones un Middleware.
    * Logging un Middleware.
    * Cortociercuito lo antes que puedo (1º Middleware y 2º Filtro).
    * Caché. En Middleware y si necesitamos negocio en filtro.
    * Un filtro está atado a MVC, por lo que si quisiéramos ir a NancyFx no podríamos reutilizarlo.
    * Validaciones de ViewModels por ejemplo con FluentValidation.

* Como cortocircuitar en un filtro.
    * Devolver un IActionResult en context.Result.

![MVC pipeline](https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/filters/_static/filter-pipeline-1.png)
![MVC pipeline](https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/filters/_static/filter-pipeline-2.png)

* Pipeline ActionExecution.
    * Authorization filters.
    * Antes de ejecutar la aplicación
        * ¿Tiene autorización el usuario para acceder al recurso?
        * No lanzar excepciones puesto que no el filtro de expceción no las manajeara.
    * Resource filters.
        * Antes y después de ejecutar la acción.
    * Model binding.
    * Action filters.
        * Antes y después de ejecutar la aplicación.
    * Exception filters.
    * Result filters.
        * Sólo si la acción ha sido ejecutada con éxito.
        * Surround.
    * Result execution
    * Se pueden heradar de los filtros built-in (AuthorizationFilter, ResultFilterAttribute, ActionFilterAttribute, ExceptionFilterAttribute).
* Inyección de dependencias.
  * No hay DI por constructor en los filtros.
  * ServiceFilterAtrtribute.
    * Lo recupera desde el contendor de dependencias.
  * TypeFilterAttribute.
    * Le podemos pasar parámetros.
  * Usar un Middleware como filtro.
    * Es un guarrería, matar moscas a cañonazos
* Estos son algunos de los filtros que vienen de serie (built-in):
  * Authorize.
  * AllowAnonymous.
  * RequireHttps.
  * RequireCache.

## Internacionalización: Globalización y localización.

La **internacionalización (*I18n*)** implica la globalización y la localización.

* **Globalización (*G11n*)**. El proceso de crear una aplicación compatible con diferentes idiomas y regiones.
* **Localización (*L10n*)**. Es el proceso de adaptación/ personalización de nuestra aplicación para soportar nuevas necesidades lingüisticas o culturales. Aunque a priori puede parecer que es solamente la traducción de unos recursos conviene recordar que estas adaptaciones también incluyen: formato de números, formato de fechas, símbolos de moneda, etc.. Es decir, el proceso de personalización de una aplicación para un determinado idioma y región.

> En el caso de MVC5 diferenciábamos entre System.Threading.Thread.CurrentCulture (sirve para establecer la cultura y sirve principalmente para dar formato a fechas, números y símbolos de moneda) y System.Threading.Thread.CurrentUICulture (sirve para traducir nuestros recursos).

NET Core tiene soporte para la internacionalización a través de un Middleware dentro del ensamblado Microsoft.Extensions.Localization y al ser un Middleware primero debemos añadirlo como dependencia.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddLocalization(options =>
    {
        options.ResourcesPath = "Resources";
    });
}
```

Para posteriormente añadir el middleware al método Configure del Startup, y es importante introducir este middleware justo antes de otros middlewares que dependan de la cultura como pueda ser el de MVC.

```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
    }

    app.UseStaticFiles();

    var supportedCultures = new List<CultureInfo>
    {
        new CultureInfo("es-ES"),
        new CultureInfo("en-US")
    };
    app.UseRequestLocalization(new RequestLocalizationOptions
    {
        DefaultRequestCulture = new RequestCulture(supportedCultures.First()),
        SupportedCultures = supportedCultures,
        SupportedUICultures = supportedCultures
    });

    app.UseMvcWithDefaultRoute();
}
```

Para utilizar los recursos dentro de un controlador debemos crear dentro de la carpeta de recursos uno con el nombre (*incluido el espacio de nombres sin ensamblado*) del controlador, es este caso de ejemplo sería:

```
Features.Home.HomeController.es.resx
Features.Home.HomeController.en.resx
```

Y en el controlador inyectar IStringLocalizer.

```csharp
public HomeController(IStringLocalizer<HomeController> localizer)
{
    _localizer = localizer;
    var localizedValue = _localizer["Home"];
}
```

¿Qué no me gusta de esto?

* Que no tengo un repositorio central de recursos.
* Que ahora tengo que utilizar los recursos con magic strings, no como antes .

La solución a la primera es relativamente sencilla. El equipo de MS propone crearnos una clase Dummy llamada SharedResource.

```csharp
// Dummy class to group shared resources
public class SharedResource
{
}

public HomeController(IStringLocalizer<SharedResource> sharedLocalizer)
{
    _sharedLocalizer = sharedLocalizer;
}
```

Ahora bien ¿De cuantas opciones dispones para hacer el setting de la cultura? ¿Cuántos proveedores de cultura tenemos disponibles?
El middleware de localización utiliza 3 proveedores por defecto:
* Mediante query string
```
?culture=fi-FI&ui-culture=fi-FI

?culture=fi-FI

?ui-culture=fi-FI
```
* Mediante cookie
    
    Un modo más persistente es enviar en la request la cultura mediante una cookie, el nombre por defecto de la cookie de cultura es ".AspNetCore.Culture" pero este nombre puede sobreescribirse.

```csharp
var cookieProvider = localizationOptions.RequestCultureProviders
    .OfType<CookieRequestCultureProvider>()
    .First();
cookieProvider.CookieName = "Culture";
```
    
* Mediante la cabecera Accept-Language

    Este proveedor comprueba la cabecera Accept-Language que ha sido enviada en la petición.

Está claro que podemos cambiar el orden de estos proveedores o incluso eliminarlos si procede.

```csharp
Accept-Language:en-US,en;
```

También existe otro proveedor que aunque no se encuentra entre los de por defecto es muy chulo, es el llamado RouteDataRequestCultureProvider, que por su nombre podemos sospechar que la cultura pasará a ser parte de la ruta. Para añadirlo bastaría con incluirlo entre los proveedores por defecto y modificar el enrutado de nuestra aplicación.

Revisar. Tengo que prepararlo.
> Using URL parameters is one of the approaches to localisation Google suggests as it is more user and SEO friendly than some of the other options.

```csharp
var requestProvider = new RouteDataRequestCultureProvider();
localizationOptions.RequestCultureProviders.Insert(0, requestProvider);
```

### Referencias
https://joonasw.net/view/aspnet-core-localization-deep-dive
https://andrewlock.net/url-culture-provider-using-middleware-as-mvc-filter-in-asp-net-core-1-1-0/

Otras cosas chulas que hay que comentar:

* Formatted strings
* IHtmlLocalizer
* Localized views (separate views for different cultures)
* Data annotation localization