new DataTable('#tblData', {
    // 1. 告訴 DataTables 去呼叫後端的 GET /product/getall API
    //    它會自動去抓回傳 JSON 裡面的 data 陣列
    ajax: '/product/getall',

    // 2. 定義每一欄要綁定 JSON 物件裡的哪個欄位，因為JSON裡面都是小寫，所以這邊的資料都要以小寫為主
    columns: [
        { data: 'title' },
        { data: 'isbn' },
        { data: 'price' },
        { data: 'author' },
        { data: 'category.name' },
        {defaultContent:''}
    ]
});