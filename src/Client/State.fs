module App.State

open Elmish
open Elmish.Browser.Navigation
open Fable.Core.JsInterop
open Fable.Import
open Fable.Import.Browser
open Fable.PowerPack.Fetch
open Fable.Remoting.Client
open Thoth.Elmish
open Pages
open Shared


open App
open App.Types
open App.Notification
open Fulma.FontAwesome

let urlUpdate (result: Page option) (model: App.Types.Model) =
    match result with
    | None ->
        Browser.console.error("Error parsing url: " + Browser.window.location.href)
        model, Navigation.modifyUrl (toPageUrl model.CurrentPage) 
    | Some Page.Home ->
        let m, cmd = Home.State.init()
        { model with PageModel = HomeModel m }, Cmd.map HomeMsg cmd
    | Some Page.Counter ->
        let m, cmd = Counter.State.init()
        { model with PageModel = CounterModel m }, Cmd.map CounterMsg cmd
    | Some Page.Janken ->
        let m, cmd = Janken.State.init()
        { model with PageModel = JankenModel m }, Cmd.map JankenMsg cmd
    | Some Page.Taxonomies ->
        let m, cmd = Taxonomies.State.init()
        { model with PageModel = TaxonomiesModel m }, Cmd.map TaxonomiesMsg cmd

let init result =
    let (home, _) = Home.State.init()
    let (model, cmd) =
      urlUpdate result
        { Note = ""
          PageModel = HomeModel home   }
    model, cmd

let update msg model =
    match msg, model.PageModel with
    // 通知メッセージ
    | NotificationMsg msg, _ ->
        let errorToast note = 
            Toast.message note.message
            |> Toast.position Toast.TopRight
            |> Toast.noTimeout
            |> Toast.icon Fa.I.TimesCircle
            |> Toast.error
        let warningToast note = 
            Toast.message note.message
            |> Toast.position Toast.TopRight
            |> Toast.title note.title
            |> Toast.noTimeout
            |> Toast.icon Fa.I.ExclamationTriangle
            |> Toast.warning
        let successToast note = 
            Toast.message note.message
            |> Toast.position Toast.TopRight
            |> Toast.title note.title
            |> Toast.icon Fa.I.CheckCircle
            |> Toast.success
        let infoToast note = 
            Toast.message note.message
            |> Toast.position Toast.TopRight
            |> Toast.title note.title
            |> Toast.icon Fa.I.InfoCircle
            |> Toast.info
        match msg with
        | MsgType.Error note ->
            model, errorToast note
        | MsgType.Warning note ->
            model, warningToast note
        | MsgType.Success note ->
            model, successToast note
        | MsgType.Info note ->
            model, infoToast note
    // 例外メッセージ
    | ErrorMsg exn, _ ->
        let notify (exn:exn) = 
            Cmd.ofMsg (NotificationMsg (MsgType.Error { Note.title = ""; message = exn.Message }))
        match exn with
        | :? ProxyRequestException as ex -> 
            match ex.StatusCode with
            | _ -> 
                { model with Note = ex.Message } , notify exn
        | _ ->
            { model with Note = exn.Message } , notify exn

    | HomeMsg msg, HomeModel m ->
        let (model', cmd) = Home.State.update msg m
        { model with PageModel = HomeModel model' }, Cmd.map HomeMsg cmd
    | HomeMsg _, _ ->
        model, Cmd.none
    
    | CounterMsg msg, CounterModel m ->
        let (model', cmd) = Counter.State.update msg m
        { model with PageModel = CounterModel model' }, Cmd.map CounterMsg cmd
    | CounterMsg _, _ ->
        model, Cmd.none

    | JankenMsg msg, JankenModel m ->
        let (model', cmd) = Janken.State.update msg m
        { model with PageModel = JankenModel model' }, Cmd.map JankenMsg cmd
    | JankenMsg _, _ ->
        model, Cmd.none

    | TaxonomiesMsg msg, TaxonomiesModel m ->
        match msg with
        | Taxonomies.Types.Msg.ApiError exn -> 
            model, Cmd.ofMsg (ErrorMsg exn)
        | Taxonomies.Types.Msg.Notify notifyMsg -> 
            model, Cmd.ofMsg (NotificationMsg notifyMsg)
        | _ ->
            let (model', cmd) = Taxonomies.State.update msg m
            { model with PageModel = TaxonomiesModel model' }, Cmd.map TaxonomiesMsg cmd
    | TaxonomiesMsg _, _ ->
        model, Cmd.none
    