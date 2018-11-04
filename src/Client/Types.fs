module App.Types
open Shared
open Pages

type Msg =
  | HomeMsg of Home.Types.Msg
  | CounterMsg of Counter.Types.Msg
  | JankenMsg of Janken.Types.Msg
  | TaxonomiesMsg of Taxonomies.Types.Msg

type PageModel =
  | HomeModel of Home.Types.Model
  | CounterModel of Counter.Types.Model
  | JankenModel of Janken.Types.Model
  | TaxonomiesModel of Taxonomies.Types.Model

type Model = {
    Note: string
    PageModel: PageModel
  }
  with 
      member this.CurrentPage = 
        match this.PageModel with
        | HomeModel _ -> Page.Home
        | CounterModel _ -> Page.Counter
        | JankenModel _ -> Page.Janken
        | TaxonomiesModel _ -> Page.Taxonomies
