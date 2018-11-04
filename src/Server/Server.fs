open System.IO
open System.Threading.Tasks

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open FSharp.Control.Tasks.V2
open Giraffe
open Saturn
open Shared

open Fable.Remoting.Server
open Fable.Remoting.Giraffe

// Dapperの初期化。null←→option の変換設定
DataAccess.addOptionHandlers()
// Sqliteの型変換設定
Repository.SqliteTypeHandler.addTypeHandlers()

let publicPath = Path.GetFullPath "../Client/public"
let port = 8085us

let webApp = router {
    forward "/api/ICounterApi" Services.Counter.apiRoute
    forward "/api/ITaxonomyApi" Services.Taxonomies.apiRoute
}

let app = application {
    url ("http://0.0.0.0:" + port.ToString() + "/")
    use_router webApp
    memory_cache
    use_static publicPath
    use_gzip
    app_config DbInit.Initialize
}

run app
