module Client

open Elmish
open Elmish.Browser.Navigation
open Elmish.React
open App.View
open App.State

#if DEBUG
open Elmish.Debug
open Elmish.HMR
#endif

Program.mkProgram init update view
|> Program.toNavigable Pages.urlParser urlUpdate
#if DEBUG
|> Program.withConsoleTrace
|> Program.withHMR
#endif
// |> Program.withReact "elmish-app"    // 日本語入力が出来ない。以下を使用する[https://github.com/elmish/react/issues/12]
|> Program.withReactUnoptimized "elmish-app"
#if DEBUG
|> Program.withDebugger // Chromeに Redux DevTools extension[https://github.com/zalmoxisus/redux-devtools-extension]を導入すること https://github.com/elmish/templates/issues/36 
#endif
|> Program.run
