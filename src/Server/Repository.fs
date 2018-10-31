module Repository

open System.Data.SQLite
open DataAccess
open Shared
open Shared.BlogModels

let getConnection (connectionString:string) = 
    new SQLiteConnection(connectionString)


type IdParam = { 
    Id : int;
}

type CountResult = {
    cnt: int64;
}

let getTaxonomies (connectionString:string) (taxonomyType:TaxonomyTypeEnum option) (page:PagerModel)  =
    let connection = getConnection connectionString

    let sqlFrom = 
        """
        from [Taxonomy]
        """
    let sqlCount = 
        """
        select count(1) as [cnt]
        """
        + sqlFrom

    let sqlList = 
        """
        select *
        """
        + sqlFrom

    let sqlWhere = 
        match taxonomyType with
        | None -> ""
        | Some x -> sprintf "where [Type] = %d" (int x)

    let sqlOrder = "order by [Id] "
    let sqlLimitAndOffset = sprintf "limit %d offset %d" page.rowsPerPage ((page.currentPage - 1L) * page.rowsPerPage)

    let makePagenation x = {page with allRowsCount = x}
    let countResult =
        connection 
        |> query<CountResult> (sqlCount + sqlWhere) |> Seq.head
    let pageResult = makePagenation countResult.cnt

    let listResult =    
        connection 
        |> query<Taxonomy> (sqlList + sqlWhere + sqlOrder + sqlLimitAndOffset)

    { data = listResult
      pagenation = pageResult }

let getTaxonomy (connectionString:string) (id:int) =
    let connection = getConnection connectionString

    let sql =
        """
        select * from [Taxonomy]
        where [Id] = @Id
        """
    let param = {Id = id}

    connection 
    |> parametrizedQuery<Taxonomy> sql param
    |> Seq.tryHead


let addNewTaxonomy (connectionString:string) (record:Taxonomy) =
    let connection = getConnection connectionString

    let sql = 
        """
        insert into [Taxonomy] (
          [Type] 
         ,[Name]
         ,[UrlSlug]
         ,[Description]
        )
        values (
          @Type 
         ,@Name 
         ,@UrlSlug 
         ,@Description
        );
        select [Id] from [Taxonomy] where ROWID = last_insert_rowid()
        """
    // connection |> execute sql record
    connection 
    |> parametrizedQuery<IdParam> sql record
    |> Seq.head
    |> (fun x -> x.Id)

let updateTaxonomy (connectionString:string) (record:Taxonomy) =
    let connection = getConnection connectionString

    let sql = 
        """
        update [Taxonomy] 
        set 
          [Type] = @Type
         ,[Name] = @Name
         ,[UrlSlug] = @UrlSlug
         ,[Description] = @Description
        where [Id] = @Id
        """
    connection |> execute sql record

let removeTaxonomy (connectionString:string) (record:Taxonomy) =
    let connection = getConnection connectionString

    let sql =
        """
        delete from [Taxonomy]
        where [Id] = @Id
        """
    connection |> execute sql record        