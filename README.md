# ASPNet Core/ ASPNet Core MVC

En este repo voy a ir metiendo todos los conceptos que he ido asimilando al ver temas de ASPNet Core y ASPNet Core MVC.

Todas estas notas de abajo las tengo que ir organizando y desarrolloando.

dotnet --info       (SDK + Runtime)
dotnet --version    (SDK)

TFM - Target framework moniker

Hay dos tipos de despliegue:
    FDD. Framework-dependent deployment (Heredado de full framework)
    SCD. Self-contained deployment
    Se tiene que indicar en el csproj.

TFM Nuget/

Huella de memoria (memory foot print)

dotnet publish -r win7-x64
    Sin -r hace FDD 
    Con -r hace SCD

Proxy inverso. Meter algo delante de kestrel (IIS, nginx)
Kestrel, no apto para producción sin proxy inverso.
NET Core Windows Server Hosting

PCL. Class library
    Eliges el target a principio.
Shared projects.
    Directivas del compilador para ejecutar condicionalmente según plataforma dónde corre
.NET Standard.
    Evolución de PCL. Sigue haciendo una librería portable pero con otro enfoque.
    Su numeracion es lineal no como BCL
    No hay breaking change (NETStandar 1.6 es compatible con 1.5, 1.4, etc...)
    NETStandar es una especificación, no una implementación.
    Open-ended.
    .NET portability analyzer (tool VS20017)

NetCore2.0 implementa NETStandar 2.0

Host - Entorno de ejecución
Server
Middleware
Framework (terminal)
    Por ejemplo el de MVC
Application

Startup
    ConfigureServices.
        Los servicios en terminología de Core son las dependencias, por lo que el ConfigureServices es donde registro mis dependencias.
        Es opcional.
        Sölo se inyecta el IServiceCollection
    Configure. 
        Configura el pipeline.
        Se ejecuta después de ConfigureServices.
        Sólo se inyecta IApplicationBuilder, IHostingEnvironment, etc...
    ¿Qué está inyectado por defecto?
    En ctor. IHostingEnvironment, ILoggerFactory, IConfiguration
DI
    Es DI de Net Core no de ASPNet Core, puede ser usado por ejemplo en una consola
        http://panicoenlaxbox.blogspot.com.es/2017/11/crear-e-inicializar-un-contexto-en-ef.html
    Ciclos de vida:
        AddTransient
        AddSingleton
            Ver StackOverflow. static vs singleton
                Las clases estáticas son para métodos, no para mantener estado.
        AddScoped
Tooling.
    dotnet watch
    dotnet ef
Por defecto, una app siempre está en producción
Métodos Configure y ConfigureService por environment, incluida la clase Startup
Antes teníamos un global.asax, ahora tenemos en Configure la posibilidad de inyectar IApplicationLifetime

Configuración
    Antes teníamos System.Configuration (obsoleta, no instanciable, api compleja)
    Ahora es acceso clave/ valor
    Streamed configuration. Es vital el orden que le das a los proveedores de Configuración
        Para leer de estos ficheros la ruta debe ser completa, y el sepador es :
    Tipado. 
        Permite al resto de la aplicación (fuera de Startup) acceder por Di a la configuración a través de addOptions en ConfigureServices
        Siempre debemos ir a tipado.
    Refresco. Sólo funciona con la configuración no tipada. El problema lo resuelve IOptionsSnapshot<T>

Secretos de usuarios.
    Es un origen/ proveedor de datos más que sólo está en tu máquina para elementos de configuración que respresentan elementos de seguridad.
    Sólo afecta a desarrollo.
    Desde VS. Botón derecho -> Manage user secret
    Si vas con VSVCode dotnet user-secrets para administrarlo desde comandos

Host settings.
    En Program.cs se configura el host
    UseUrl
    UserEnvironment

Logging
    Configure (ILoggerFactory)
    Tipos
        Consola
        Debug. Va al output window.
        Crear personalizados.
    Filtrar nivel de traza.
        LoggerFactor.WithFilter en Configure como LogLevel.None
        Podemos filtrar por código o por appsettings.json
    Para filtrar una request por petición, tenemos dentro de AddConsole(includeScopes: true)
    Serilog.
        Structured logging. La capacidad de manejar no como strings sino como datos/ objetos.
        Seq. Consumir Structured logging.
        Enrichers. Enriquecedores.
            Thread.
            Process.
            Environment.
        Sinks. Dónde guardaremos los datos.
            RollingFile
            ColoredConsole
            Slak
            MSSqlServer
    Scopes.
        Interesante. Agrupar logging. 
Caching
    IMemomryCaching. 
        Memoria de la máquina
        Cache aside.
    IDistributedCaching.
        Caché sincronizada entre varios servidores. 
        SQL Server/ Redis.

Middlewares.
    Se introdujo en Owin.
    Son componentes que forma un pipeline entre el servidor y nuestra aplicación.
    Se procesan en orden.
    Middleware pass throw
        Ojo! Tiene que acabar en un Run siempre, ojo! o un Use sin hacer invoke del siguiente
        Use
            Response.HasStarted. Es útil para indicar si las cabeceras han sido enviadas o el body ha sido escrito.
            Si es true, sólo es válido llamar a Respone.WriteAsync y no sew puede por otro lado modificar la cabeceras.            
        Run
            Terminal
        Map
            Divide el pipeline en ramas o branches en función de la ruta de petición
        MapWhen
            Es map pero con una condición, hacer una rama su se evalua cierto el predicado
    Clase.
    Ciclo de vida. Sólo se crea una vez durante la inicialización del pipeline.
    Built-in middleware
        Authentication
            Provides authentication support.
        CORS
            Configures Cross-Origin Resource Sharing.
        Response Caching
            Provides support for caching responses.
        Response Compression
            Provides support for compressing responses.
        Routing	
            Defines and constrains request routes.
        Session	
            Provides support for managing user sessions.
        Static Files
            Provides support for serving static files and directory browsing.
        URL Rewriting Middleware
            Provides support for rewriting URLs and redirecting requests.
Session
    ASPNet Session
    Por defecto viene sin sesión porque sesión es un middleware (services.AddSession(), app.UseSession())
Es importante la posición del Middleware, ejemplo claro con Compression (ahora ASPNet realiza la compresión así no dependemos del IIS) antes o después por ejemplo de los estáticos. Brutal!
Url rewrite.
    Mapear las URL de peticiones entrantes a las URL que internamente utiliza la aplicación
    Por ejemplo forzar el tráfico Https, en vez de configurarlo en IIS.
Routing
    No es exclusivo de MVC, es ASPNet Core.
 MVC
    Es otro middleware de ASPNet Core, es decir services.AddMvc()/ app.UseMvcWithDefaultRoute()
    Controladores
        Normalmente heredan de Controller
        Normalmente devuelve a IActionResult aunque podemos devolver un string o un objeto y él hará el wrapper
        Puedes ser clases POCO (La convención es que acaba en el palabra Controller)
        ActionName es para especificar el nombre de la aplicación
        NonAction. Que no se puede alcanzar por ruta.
        Selección de acción con atributos como antes y se aprovecha para la ruta HttpHet("baz")
        Rutas.
            Iremos por defecto.
            Constraints. int, bool, datetime, decimal, double, float... Aunque podemos crear las nuestras propias con IRouteConstraint
            Atribute data routing.
                [Route("MiHome")] a nivel de controlador
                    Con / en la ruta de la acción sobreescribe y no hereda de Route
            Otro ejemplo de Route es para separar en Route las rutas de api [Route("api/[controller]")]
        Inyección por método/ acción con [FromServices]
        IActionResult
            Si devolvemos algo distinto IActionResult será serializado a JSON y devuelto con content-type "application/json excepto string
            Tipos predefinidos
                ContentResult. text/plain
                EmptyResult.
                FileResult. 
                StatusCodeResult
                OkResult
                NoContent
                BadRequestResult
                UnauthorizedResult
                ViewResult
                PartialViewResult
                JsonResult
        Razor
            Ahora las vistas con asíncronas
            Se le pueden inyectar depdendicar con @inject
    CPU Bound   sync
    IO Bound    async
    Los valores que vienen de URL usan Culture insensitive
    Los valores que vienen por post llevan cultura
    Binding en MV5
        Value providers.
            Query string, route values, headers, body. Son extensibles, leer datos de distintas partes de la petición.
            MVC itera sober todos los value providers, por lo que el último léido es el prevalecido.
        Model binders.
            Rellenan los parámetros de la acción desde el sitiio común donde los value providers han dejado los datos.
            Desconocen de dónde han venido los datos.
            Por cada parámetro de una acción sólo puedo haber un model binder.
    Binding en WebApi 2
        Parámetros simples
            Values providers
        Parámetros complejos
            media-type formatter
                body
                A diferencia de los value providers devuelve el dato ya enlazado.
        El body es leído por un media type formatter y sólo puede ser leído una vez
        Sólo un parámetro puede ser leído desde el body 
            [FromBody] Leer el parámetro simple desde el body
            [FromUrl] Leer un parámetro complejo con un value provider, no leer del body
    Binding en Core
        Por defecto es el binding de MVC5. "A lo MVC". Leer desde body si el content type es application/x.www....
            No entiende de JSON.
            Atributos. A lo MVC5. Aplicables tanto a parámetros como a propiedades de un ViewModel
                FromForm.
                FromQuery.                 
                FromRoute. 
                FromHeader.                                         
        A lo WebApi con media formatter, sólo un parámetro puede leer del body
            FromBody. 
        ** JSON    
            No hay ningún value provider que lea JSON (A lo MVC).
            Si hay un media-type formatter que entiendo JSON (FromBody)
        ModelBinder
            Enlazar un tipo con datos con una representación concreta
            IModelBinder
            ¿Cómo asocio un parámetro de con un IModelBinder?
                Con IModelBinderProvider
        InputFormatter
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