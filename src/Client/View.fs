module App.View

open System
open Fable.Helpers.React
open Fable.Helpers.React.Props

open Fulma
open Fulma.FontAwesome

open Shared
open Types
open Pages

let navBrand =
    Navbar.navbar [ Navbar.Color IsWhite ]
        [ Container.container [ ]
            [ Navbar.Brand.div [ ]
                [ Navbar.Item.a [ Navbar.Item.CustomClass "brand-text" ]
                      [ str "猫に.NET" ]
                  Navbar.burger [ ]
                      [ span [ ] [ ]
                        span [ ] [ ]
                        span [ ] [ ] ] ]
            ]
        ] 

let menuItem label page currentPage =
    Menu.Item.li  
        [ Menu.Item.IsActive (page = currentPage)
          Menu.Item.Props [
             Href (toPageUrl page)
             OnClick ViewHelper.goToUrl]]
        [  str label  ]


let menu currentPage =
    Menu.menu [ ]
        [ Menu.label [ ] [ str "Page Sample" ]
          Menu.list [ ]
              [ 
                  menuItem "Home" Page.Home currentPage
                  menuItem "Counter" Page.Counter currentPage
                  menuItem "Janken" Page.Janken currentPage
              ]
          Menu.label [ ] [ str "Maintenance" ]
          Menu.list [ ]
              [ 
                  menuItem "Taxonomies" Page.Taxonomies currentPage
              ]
        ]

let view (model : Model) (dispatch : Msg -> unit) =
    let pageHtml =
        function
        | HomeModel m -> Home.View.root m (HomeMsg >> dispatch)
        | CounterModel m -> Counter.View.root m (CounterMsg >> dispatch)
        | JankenModel m -> Janken.View.root m (JankenMsg >> dispatch)
        | TaxonomiesModel m -> Taxonomies.View.root m (TaxonomiesMsg >> dispatch)        

    div [ ]
        [ navBrand
          Container.container [ ]
              [ Columns.columns [ ]
                  [ Column.column [ Column.Width (Screen.All, Column.Is3) ]
                      [ menu model.CurrentPage ]
                    Column.column [ Column.Width (Screen.All, Column.Is9) ]
                      [ 
                        pageHtml model.PageModel 
                      ] 
                  ] 
              ] 
        ]
