//分页列表动态排序字段
$(function () {
    var AdminOrderBy = $("#hid_order_by").val();
    var CookieOrderBy = $("#hid_order_by_key").val();

    if (AdminOrderBy == undefined) {
        alert("Input Hidden【hid_order_by】not exists");
        return;
    }
    if (AdminOrderBy == "") {
        alert("Input Hidden【hid_order_by】 value is empty");
        return;
    }

    if (CookieOrderBy == undefined) {
        alert("Input Hidden【hid_order_by_key】not exists");
        return;
    }
    if (CookieOrderBy == "") {
        alert("Input Hidden【hid_order_by_key】 value is empty");
        return;
    }

    var order_arr = AdminOrderBy.split(' ');
    if (order_arr.length == 2) {
        var keys = order_arr[0].toLowerCase();
        $(".sort_attr").each(function (index, item) {
            if ($(this).attr("s-name").toLowerCase() == keys) {
                if (order_arr[1].toLowerCase() == "desc") {
                    $(this).text($(this).text().replace("↓", "").replace("↑", "") + "↓");
                } else if (order_arr[1].toLowerCase() == "asc") {
                    $(this).text($(this).text().replace("↓", "").replace("↑", "") + "↑");
                }
            }
        });
    }

    $(".sort_attr").click(function () {
        var SortKey = $(this).attr("s-name");
        var show_text = $(this).text();//↓ ↑
        var OrderBy = SortKey;
        var new_text = "";
        if (show_text.indexOf("↓") != -1) {
            OrderBy += " ASC";
            new_text = show_text.replace("↓", "") + "↑";
        } else if (show_text.indexOf("↑") != -1) {
            OrderBy += " DESC";
            new_text = show_text.replace("↑", "") + "↓";
        } else {
            OrderBy += " ASC";
            new_text = show_text + "↑";
        }
        $(".sort_attr").each(function (index, item) {
            if ($(this).attr("s-name") != SortKey) {
                $(this).text($(this).text().replace("↓", "").replace("↑", ""));
            }
        })

        $.post("/admin/tools/SetSortCookie", { "cname": CookieOrderBy, "orderby": OrderBy }, function (result) {
            $(this).text("" + new_text + "");
            console.log(new_text + "--" + OrderBy);
            window.location.reload();
        });


    });
})