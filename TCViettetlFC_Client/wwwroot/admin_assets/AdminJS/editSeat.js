
function updateSeat(AreaId, MatchId) {

    var quantittySeat = $("#seat_" + AreaId).val();

    var data = {
        AreaId: AreaId,
        MatchId: MatchId,
        SeatQuantity: quantittySeat
    }
    $.ajax({
        url: 'https://tcvtfcapi.azurewebsites.net/api/MatchAreas/UpdateSeat',
        type: 'Put',
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function (response) {
            /*  loadData();*/
            showAlert("Cập nhật thành công");
            //$("#modalEditorCreate").modal("hide");
        },
        error: function (error) {
            debugger
        }
    });

}



function showAlert(mess) {

    $("#alertMessageSuccess").text(mess);
    $("#alertSuccess").show();
    setTimeout(function () {
        $("#alertSuccess").hide();
    }, 3500);
}


$("#khandai").on("change", function () {
    debugger
    var khandai = $("#khandai").val();
    if (khandai === "C" || khandai === "D") {
        $("#tang").val(0);
        $("#cua10").val(0);
        
        $("#divCD").show();
        $("#divAB").hide();
        $("#tangdiv").hide();

    } else {
        $("#tang").val(0);
        $("#tangdiv").show();
        $("#divCD").hide();
        $("#divAB").hide();
    }
    var id = $("#matchID").val();
    var url = "https://tcvtfcapi.azurewebsites.net/api/MatchAreas/GetSanPhamById?id=" + id;
    if (khandai != 0 && khandai != undefined) {

        $.ajax({
            url: url + `&$filter=stands eq '${khandai}'`,
            type: 'get',
            contentType: 'application/json',
            success: function (response) {
                loadData(response);
                formatPrice()
            },
            error: function (error) {
            }
        });
    } else {
        $("#tangdiv").hide();
        $("#divCD").hide();
        $("#divAB").hide();
        $.ajax({
            url: url,
            type: 'get',
            contentType: 'application/json',
            success: function (response) {
                loadData(response);
                formatPrice()
            },
            error: function (error) {
            }
        });
    }

});

$("#tang").on("change", function () {
    var tang = $("#tang").val();
    var id = $("#matchID").val();
    if (tang == 2) {
        $("#divCD").show();
        $("#divAB").hide();
        $("#cua10").val(0);
        $("#cua8").val(0);

    } else if (tang == 5) {
        $("#divAB").show();
        $("#divCD").hide();
        $("#cua10").val(0);
        $("#cua8").val(0);
    }
    var url = "https://tcvtfcapi.azurewebsites.net/api/MatchAreas/GetSanPhamById?id=" + id;
    if (tang != null && tang != 0) {
        debugger
        var khandai = $("#khandai").val();
       
        $.ajax({
            url: url + `&$filter=stands eq '${khandai}' and floor eq '${tang}'`,
            type: 'get',
            contentType: 'application/json',
            success: function (response) {
                loadData(response);
                formatPrice()
            },
            error: function (error) {
            }
        });
    } else {
        $("#divCD").hide();
        $("#divAB").hide();
        $.ajax({
            url: url ,
            type: 'get',
            contentType: 'application/json',
            success: function (response) {
                loadData(response);
                formatPrice()
            },
            error: function (error) {
            }
        });
    }
})


$(".sections").on("change", function () {
    var cua = $(this).val();
    var id = $("#matchID").val();
    var url = "https://tcvtfcapi.azurewebsites.net/api/MatchAreas/GetSanPhamById?id=" + id;
    var khandai = $("#khandai").val();

    if (cua != null && cua != 0) {
        debugger

        if (khandai === "C" || khandai === "D") {
            $.ajax({
                url: url + `&$filter=stands eq '${khandai}' and section eq '${cua}'`,
                type: 'get',
                contentType: 'application/json',
                success: function (response) {
                    loadData(response);
                    formatPrice()
                },
                error: function (error) {
                }
            });
        } else {
            var tang = $("#tang").val();
            $.ajax({
                url: url + `&$filter=stands eq '${khandai}' and floor eq '${tang}' and section eq '${cua}' `,
                type: 'get',
                contentType: 'application/json',
                success: function (response) {
                    loadData(response);
                    formatPrice()
                },
                error: function (error) {
                }
            });
        }

        if (tang != null && tang && undefined && !isNaN(tang) ) {

        }

        
    } else {
        
        if (khandai === "C" || khandai === "D") {
            $.ajax({
                url: url + `&$filter=stands eq '${khandai}'`,
                type: 'get',
                contentType: 'application/json',
                success: function (response) {
                    loadData(response);
                    formatPrice()
                },
                error: function (error) {
                }
            });
        } else {
            var tang = $("#tang").val();
            $.ajax({
                url: url + `&$filter=stands eq '${khandai}' and floor eq '${tang}'`,
                type: 'get',
                contentType: 'application/json',
                success: function (response) {
                    loadData(response);
                    formatPrice()
                },
                error: function (error) {
                    debugger
                }
            });
        }
       
    }
})

function loadData(data) {

    var body = $('#tbody').empty();
    $.each(data, function (index, m) {
        var html = `
                                                   <tr class="add-row">
                                                        <td>
                                                            <input type="text" value="${m.stands}" readonly class="form-control">
                                                        </td>
                                                        <td>
                                                            <input type="text" value="${m.floor}" readonly class="form-control">
                                                        </td>
                                                        <td>
                                                            <input type="text" value="${m.section}" readonly class="form-control">
                                                        </td>
                                                        <td>
                                                            <input type="text"  value="${m.price}" readonly class="form-control price">
                                                        </td>
                                                        <td>
                                                            <input type="text" value="${m.availableSeats}" class="form-control seat-input" id="seat_${m.areaId}" />
                                                        </td>
                                                        <td class="add-remove text-center">
                                                            <button class="btn btn-rounded btn-outline-primary me-2" data-aid="${m.areaId}" data-mid="${m.matchId}" onclick="updateSeat(${m.areaId} , ${m.matchId})">
                                                                <i class="fe fe-pencil">cập nhật</i>
                                                            </button>
                                                        </td>

                                                    </tr>
                `;

        body.append(html) // Thêm từng dòng mới vào bảng
    });



}

function formatPrice() {
    $('.price').each(function () {
        let value = $(this).val().trim(); // Lấy giá trị từ input
        if (value.includes(',')) {
            value = value.replace(',', '.'); // Thay dấu phẩy bằng dấu chấm
        }
        if (!isNaN(value) && value !== '') {
            const formattedValue = formatCurrency(parseFloat(value)); // Định dạng tiền tệ
            $(this).val(formattedValue); // Cập nhật lại giá trị
        }
    });
}