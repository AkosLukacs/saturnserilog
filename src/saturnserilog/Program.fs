module Server

open Saturn
open Config

open Giraffe.SerilogExtensions
open Serilog
open Serilog.Formatting.Json

let endpointPipe = pipeline {
    plug head
    // plug requestId
}

let webAppWithLogging = SerilogAdapter.Enable(Router.appRouter)

let app = application {
    pipe_through endpointPipe

    error_handler (fun ex _ -> pipeline { render_html (InternalError.layout ex) })
    use_router webAppWithLogging
    url "http://0.0.0.0:8085/"
    memory_cache
    use_static "static"
    use_gzip
    use_config (fun _ -> {connectionString = "DataSource=database.sqlite"} ) //TODO: Set development time configuration
}

[<EntryPoint>]
let main _ =
    printfn "Working directory - %s" (System.IO.Directory.GetCurrentDirectory())
    Log.Logger <-
      LoggerConfiguration()
        // add native destructuring
        .Destructure.FSharpTypes()
        // from Serilog.Sinks.Console
        .WriteTo.Console()
        .CreateLogger()
    run app
    0 // return an integer exit code