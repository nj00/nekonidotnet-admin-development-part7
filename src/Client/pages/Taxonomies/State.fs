module Taxonomies.State

open Elmish
open Fable.Remoting.Client
open Types
open Shared
open Shared.BlogModels


let getApi : ITaxonomyApi =
    Remoting.createApi()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<ITaxonomyApi>

let initPagenation = { rowsPerPage = 5L; currentPage = 1L; allRowsCount = -1L;}

/// <summary>
/// 一覧取得
/// </summary>
let getList (criteria:ListCriteria) =
    let param = { taxonomyType =
                    match criteria.taxonomyType with
                    | Category -> Some TaxonomyTypeEnum.Category
                    | Tag -> Some TaxonomyTypeEnum.Tag
                    | Series -> Some TaxonomyTypeEnum.Series
                    | _ -> None
                  pagenation = criteria.page }

    Cmd.ofAsync
        getApi.getTaxonomies
        param
        (Ok >> Loaded)
        ApiError

/// <summary>
/// Init関数
/// <summary>
let init () : Model * Cmd<Msg> =
    let model = {
        listCriteria = { taxonomyType = All; page = initPagenation}
        dataList = None
        currentRec = None
    }
    let cmd = getList model.listCriteria
    model, cmd

/// <summary>
/// Update関数
/// </summary>
let update (msg : Msg) (model : Model) : Model * Cmd<Msg> =
    let api = getApi

    // idが負の値は追加、それ以外は更新を行う
    let updateOrInsert (taxonomy:Taxonomy) : Cmd<Msg> =
        let serverApi = 
            if taxonomy.Id < 0L then
                api.addNewTaxonomy
            else
                api.updateTaxonomy
        Cmd.ofAsync serverApi taxonomy (Ok >> Saved) ApiError


    match msg with
    // 一覧再読み込み
    | Reload -> 
        {model with currentRec = None}, getList model.listCriteria

    // 一覧抽出条件変更
    | CriteriaChanged x ->
        let newCriteria = {x with page = initPagenation } 
        {model with listCriteria = newCriteria}, Cmd.ofMsg Reload

    // ページング
    | PageChanged newPage ->
        {model with listCriteria = { model.listCriteria with page = newPage} }, Cmd.ofMsg Reload

    // 一覧読み込み
    | Loaded (Ok x) -> 
        { model with dataList = Some x.data; listCriteria = {model.listCriteria with page = x.pagenation } }, 
        Cmd.none

    // 新規追加
    | AddNew -> 
        { model with currentRec = Some { Id = -1L; Type = TaxonomyTypeEnum.Category; Name = ""; UrlSlug = ""; Description = None; }}, 
        Cmd.none

    // 一覧からデータ選択
    | Select x -> 
        model, 
        Cmd.ofAsync 
            api.getTaxonomy 
            x.Id 
            (Ok >> Selected) 
            ApiError
    | Selected (Ok x) ->
        { model with currentRec = x }, Cmd.none
    
    // 値変更
    | RecordChanged changed ->
        { model with currentRec = Some changed }, Cmd.none

    // 保存
    | Save x ->
        { model with currentRec = Some x}, updateOrInsert x

    // 削除
    | Remove x ->
        model,
        Cmd.ofAsync
            api.removeTaxonomy
            x
            (Ok >> Removed)
            ApiError

    // 更新／削除後は一覧データ取得
    | Saved (Ok _)| Removed (Ok _)->
        let newCriteria = {model.listCriteria with page = initPagenation } 
        { model with listCriteria = newCriteria }, Cmd.ofMsg Reload

    // Apiエラーはそのまま伝搬
    | ApiError e ->
        model, Cmd.ofMsg msg

    | _ -> model, Cmd.none
