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

## Internacionalización: Globalización y localización

La **internacionalización (*I18n*)** implica la globalización y la localización.

* **Globalización (*G11n*)**. El proceso de crear una aplicación compatible con diferentes idiomas y regiones.
* **Localización (*L10n*)**. Es el proceso de adaptación/ personalización de nuestra aplicación para soportar nuevas necesidades lingüisticas o culturales. Aunque a priori puede parecer que es solamente la traducción de unos recursos conviene recordar que estas adaptaciones también incluyen: formato de números y fechas, símbolos de moneda, etc.. Es decir, el proceso de personalización de una aplicación para un determinado idioma y región.

> Tenemos que tener absolutamente claro la diferencia existente entre Culture y UICulture. Con Culture establecemos la cultura y sirve principalmente para dar formato a fechas, números y símbolos de moneda y UICulture sirve para traducir nuestros recursos.

NET Core tiene soporte para la internacionalización a través de un Middleware dentro del ensamblado Microsoft.Extensions.Localization y al ser un Middleware primero debemos añadirlo como dependencia.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddLocalization(options =>
    {
        //Definimos la ruta/ ubicación de nuestros recursos
        options.ResourcesPath = "Resources";
    });
}
```

Para posteriormente añadir el middleware al método Configure del Startup. Ojo! Es importante introducir este middleware justo antes que otros middlewares que dependan de la cultura como pueda ser el de MVC.

```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
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

Para utilizar los recursos dentro de un controlador debemos crear dentro de la carpeta de recursos uno con el nombre del controlador (*incluido el espacio de nombres sin ensamblado*), es este caso de ejemplo sería:

```
Features.Home.HomeController.es.resx
Features.Home.HomeController.en.resx
```

Y en el controlador inyectar **IStringLocalizer**. ¿Qué es IStringLocalizer? Es un artefacto creado para mejorar la productividad al desarrollar aplicaciones localizadas y utiliza tanto el ResourceManager como ResourceReader para proporcionar recursos en tiempo de ejecución. Si no encuentra la clave devuelve la misma clave solicitada, y este enfoque, puede cambiar un poco la percepción en el desarrollo ya que de esta forma no necesitamos ningún archivo de recursos por defecto. A mí personalmente me gusta más el enfoque tradicional.

En el controlador también podemos inyectar **IHtmlLocalizer** que es la implementación para los recursos que contienen código HTML.

```csharp
public HomeController(IStringLocalizer<HomeController> localizer)
{
    _localizer = localizer;
    var localizedValue = _localizer["Home"];
}
```

Conclusiones.

* ¿Qué no me gusta de esto?
  * Que no tengo un repositorio central de recursos.
  * Que ahora tengo que utilizar los recursos con magic strings, no como antes que existía una clase autogenerada para poder acceder a los recursos.

La solución a la primera es relativamente sencilla. El equipo de MS propone crearnos una clase Dummy llamada SharedResource y nuestros recursos SharedResource.en.resx, SharedResource.es.resx.

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

La solución a la segunda (magic strings) es menos trivial y lo dejaré para un artículo posterior, en el cualquier mostraré una primera aproximación.

Ahora bien **¿De cuantas opciones disponemos para hacer establecer la cultura? ¿Cuántos proveedores de cultura tenemos disponibles?** Disponemos del middleware de localización(*Microsoft.Extensions.Localization*) que utiliza 3 proveedores por defecto:

> *Ni que decir tiene y vuelvo a insistir en ello que no es lo mismo Culture (formato fechas, decimales, símbolo moneda) que UICulture (traducciones) y pongo un ejemplo muy concreto: Mi empresa trabaja en dolares (Culture en-US) y yo puedo ver la aplicación en español o inglés.*

* Mediante **query string**.

Si en la query string indicamos sólo una de las dos (culture/ ui-culture) nos cambia tanto Culture como UICulture por lo que si queremos que sean diferentes debemos indicar las dos en la querystring.

```script
//Los símbolos de moneda, formatos de número y fecha serán en-US salvo las traducciones que serán es-ES
?culture=en-US&ui-culture=es-ES

//Tanto Culture como UICulture serán en-US
?culture=en-US

//Tanto Culture como UICulture serán es-Es
?ui-culture=es-ES
```

* Mediante **cookie**

Un modo más persistente es enviar en la request la cultura mediante una cookie. El nombre por defecto de la cookie de cultura es ".AspNetCore.Culture" pero este nombre puede sobreescribirse durante la configuración.

```csharp
var cookieProvider = localizationOptions.RequestCultureProviders
    .OfType<CookieRequestCultureProvider>()
    .First();
cookieProvider.CookieName = "Culture";
```

```script
c=en-US|uic=es-ES

c=en-US

uic=en-US
```

> Durante las pruebas que he realizado para este post por sólo me ha funcionado la opción en la que hay que indicar tanto la Culture como UICulture.

* Mediante la cabecera **Accept-Language**

    Este proveedor comprueba la cabecera Accept-Language que ha sido enviada en la petición y no se puede diferenciar entre Culture y CultureUI.

```csharp
Accept-Language:en-US,en;q=0.8,es;q=0.6;
```

El orden de estos proveedores es importante ya que el último es el que prevalece, pero está claro que podemos cambiar este orden e incluso eliminarl alguno si procede.

```csharp
private static void RemoveAcceptLanguageProvider(RequestLocalizationOptions options)
{
    var acceptLanguageProvider = options.RequestCultureProviders
        .OfType<AcceptLanguageHeaderRequestCultureProvider>()
        .First();
    options.RequestCultureProviders.Remove(acceptLanguageProvider);
}
```

También existe otro proveedor que aunque no se encuentra entre los de por defecto es muy chulo, es el llamado **RouteDataRequestCultureProvider**, que como por su nombre podemos sospechar que la cultura pasará a ser parte de la ruta. Para añadirlo bastaría con incluirlo entre los proveedores por defecto y modificar el enrutado de nuestra aplicación.

```csharp
var requestProvider = new RouteDataRequestCultureProvider();
localizationOptions.RequestCultureProviders.Insert(0, requestProvider);
```

Revisar. Ojo! También podemos hacer un proveedor personalizado.

```csharp
options.RequestCultureProviders.Insert(0, new CustomRequestCultureProvider(async context =>
{
    return new ProviderCultureResult("en");
}));
```

> Using URL parameters is one of the approaches to localisation Google suggests as it is more user and SEO friendly than some of the other options.

Otras cosas chulas que hay que comentar:

* **Localización de vistas**. Tal y como hemos visto, en las vistas podemos inyectar componentes/ colaboradores y en este caso lo que podemos hacer es inyectar un servicio del tipo IViewLocalizer que es el encargado de proporcionar "cadenas localizadas" a una vista ya sea con soporte de HTML (*IHtmlLocalizer*) o no (*IStringLocalizer*).

```html
@inject IStringLocalizer<SharedResource> Localizer
@inject IHtmlLocalizer<SharedResource> HtmlLocalizer

<h2>@Localizer[SharedResourceKeys.Home]</h2>

<h2>@HtmlLocalizer[SharedResourceKeys.WelcomeExtendedMessage, "Buddy"]</h2>
```

Para poder utilizar esta característica es necesario añadirla como dependencias en nuestra clase  Startup.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc()
        .AddFeatureFolders()
        .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);
}
```

* **Cadenas formateadas**. Justo como hemos visto en el punto anterior tenemos soporte para las cadenas formateadas ya que las clases "localizadoras" como último parámetro admiten el típico params object[] que son los argumentos del string.Format.

* **Localización de DataAnnotations**. Si queremos que la localización funcione en los DataAnnotations de nuestros ViewModels debemos añadirla como dependencia en ConfigureServices de nuestra clase Startup.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc()
        .AddFeatureFolders()
        .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
        .AddDataAnnotationsLocalization(options =>
        {
            //Esto sólo lo incluiremos si queremos usar un almacén de traducciones diferente al de por defecto
            options.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(typeof(SharedResource));
        });
}
```

### Referencias

![https://docs.microsoft.com/en-us/aspnet/core/fundamentals/localization](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/localization)

![https://joonasw.net/view/aspnet-core-localization-deep-dive](https://joonasw.net/view/aspnet-core-localization-deep-dive)

![https://andrewlock.net/url-culture-provider-using-middleware-as-mvc-filter-in-asp-net-core-1-1-0/](https://andrewlock.net/url-culture-provider-using-middleware-as-mvc-filter-in-asp-net-core-1-1-0/)
