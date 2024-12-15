
$(document).ready(function () {
    loadData();
    if (!$.fn.DataTable.isDataTable('.datatable')) {
        $('.datatable').DataTable({
            "paging": true,
            "pageLength": 10,
            "ordering": true,
            "info": true
        });
    }
});



function loadData() {
    $.ajax({
        url: "https://tcvtfcapi.azurewebsites.net/api/MatchAreas/GetMatchArea",
        method: "GET",
        dataType: "json",
        success: function (res) {
           
            var table = $('.datatable').DataTable();

            table.clear();
            $.each(res, function (index, item) {
              

                var matchDate = new Date(item.matchDate);
                var today = new Date();
                var dateFormatted = matchDate.toLocaleDateString('en-GB');
                var check = false;
                var statusTxt = "";
                if (matchDate > today) {
                    check = true;
                    statusTxt = ` <td class="text-center">
                                       <span class="badge badge-pill bg-success inv-badge">Chưa đá</span>
                                  </td>` ;
                } else {
                    check = false;
                    statusTxt = ` <td class="text-center">
                                       <span class="badge badge-pill bg-secondary inv-badge">Đã đá</span>
                                  </td>` ;
                }
                var button = "";
                if (check) {
                    button = `<a href="/Staff/SeatEdit/${item.id}" class="btn btn-sm bg-success-light me-2">
                                  <i class="fe fe-settings"></i> Sửa ghế
                              </a>` ;
                }

               
                var rowHtml = `
                                   <tr>
                                        <td>Thể Công - Viettel vs ${item.opponentName}</td>
                                        <td>${dateFormatted}</td>
                                        <td>${item.stadiumName}</td>
                                        ${statusTxt}
                                        <td class="text-center">
                                            <div class="actions">
                                               ${button}
                                            </div>
                                        </td>
                                    </tr>`;

                table.row.add($(rowHtml)); // Thêm từng dòng mới vào bảng
            });

            table.draw();
        },
        error: function (res) {
            console.error("Error loading data", res);
        }
    });
}

// Khởi tạo DataTable khi trang tải lần đầu
$(document).ready(function () {
    if (!$.fn.DataTable.isDataTable('.datatable')) {
        $('.datatable').DataTable({
            "paging": true,
            "pageLength": 10,
            "ordering": true,
            "info": true
        });
    }
});




function resetForm() {
    debugger
    $('#errorTen').hide();
    $('#errorTenSan').hide();
    $('#errorNgayDa').hide();

    $('#btnNomal').show();
    $('#btnLoading').hide();

    $('#idTran').val("");
    $('#TenDoiThu').val("");
    $('#tenSan').val("SVĐ Mỹ Đinh");
    $('#isHome').val(1);
    $('#ngayDa').val("");
    $('#status').prop('checked', true);

}

function modalEditOrCreate(id) {
    debugger
    resetForm();
    if (id != 0 && id != undefined) {
        $("#titleModal").text("Cập nhật trận đấu")

        var url = "https://tcvtfcapi.azurewebsites.net/api/Matches/GetMatchesById?id=" + id;
        $.ajax({
            url: url,
            method: "get",
            success: function (res) {

                var logoUrl = "";
                if (res.logoUrl == null || res.logoUrl == "" || res.logoUrl == undefined) {
                    logoUrl = "/image/imagelogo/ImageFail.jpg"
                } else {
                    logoUrl = res.logoUrl;
                }

                $('#idTran').val(res.id);
                $("#ImgAvt").attr("src", logoUrl);
                $('#TenDoiThu').val(res.opponentName);
                $('#tenSan').val(res.stadiumName);
                $("#tenCLB").text(res.opponentName)
                $('#ngayDa').val(res.matchDate);
                debugger
                res.isHome == true ? $('#isHome').val(1) : $('#isHome').val(2);
                if (res.status == 1) {
                    $('#status').prop('checked', true);
                } else {
                    $('#status').prop('checked', false);

                }
                $(".modal-backdrop").hide();

                $('#modalEditorCreate').modal("show")

            },
            error: function (res) {
                alert("Lỗi ")
            }
        });

    }
    else {

        $("#titleModal").text("Thêm trận đấu")

        $("#ImgAvt").attr("src", "/image/imagelogo/plus.jpg")
        $("#tenCLB").text("Tên đối thủ")
        $('#idTran').hide()
      
        $(".modal-backdrop").remove();
        $('#modalEditorCreate').modal("show")

    }

}

function EditOrCreate() {
    if (validation()) {

        $('#btnNomal').hide();
        $('#btnLoading').show();
        var id = $('#idTran').val();

        var formData = new FormData();
        var inputElement = $('#fileInput');
        var files = inputElement.prop('files');
        if (files.length > 0) {
            var file = files[0];
            formData.append('LogoUrl', file);
        }

        formData.append('OpponentName', $('#TenDoiThu').val());
        formData.append('StadiumName', $('#tenSan').val());
        formData.append('MatchDate', $('#ngayDa').val());
        if ($('#status').prop('checked')) {
            formData.append('Status', 1);
        } else {
            formData.append('Status', 2);
        }

        formData.append('IsHome', $('#isHome').val() === '1');


        if (id != 0 && id != undefined) {
            UpdateMatch(formData, id);

        } else {
            AddMatches(formData);
        }

    }

}



function UpdateMatch(formData, id) {

    var data = {
        NgayDa: formData.get("MatchDate"),
        TenDoiThu: formData.get("OpponentName"),
        TennSan: formData.get("StadiumName")
    }
    debugger
    checkExist(data, function (exist, mess,type) {

        if (exist) {
            if (type == 1) {
                $('#errorTenSan').text(mess);
                $('#errorTenSan').show();
            }
            else if (type == 2) {
                $('#errorNgayDa').text(mess);
                $('#errorNgayDa').show();

            }
            else if (type == 3) {
                $('#errorNgayDa').text(mess);
                $('#errorNgayDa').show();
            }
            $('#btnNomal').show();
            $('#btnLoading').hide();
        } else {
            $('#errorNgayDa').hide();
            $('#errorNgayDa').hide();

            $.ajax({
                url: 'https://tcvtfcapi.azurewebsites.net/api/Matches/UpdateMatches/' + id,
                type: 'Put',
                data: formData,
                contentType: false, // Không thiết lập Content-Type
                processData: false, // Không xử lý dữ liệu
                success: function (response) {
                    loadData();
                    showAlert("Cập nhật thành công");
                    $("#modalEditorCreate").modal("hide");
                },
                error: function (error) {
                    debugger
                }
            });
        }
    });

  
}
function validation() {

    var check = true;
    if ($('#TenDoiThu').val().trim() == "" || $('#TenDoiThu').val() == null) {
        check = false;
        $('#errorTen').show();
    } else {
        $('#errorTen').hide();

    }
    if ($('#isHome').val() === '2') {
        if ($('#tenSan').val().trim() == "" || $('#tenSan').val() == null) {
            check = false;
            $('#errorTenSan').show();

        } else {
            $('#errorTenSan').hide();

        }
    }
    let ngayDa = $('#ngayDa').val().trim();
    debugger
    if (ngayDa === "" || ngayDa == null) {
        check = false; 
        $('#errorNgayDa').show();

    } else {
        $('#errorNgayDa').hide();

        let inputDateTime = new Date(ngayDa);
        let now = new Date(); 

        if (inputDateTime < now) {
            check = false;
            $('#errorNgayDa').text("Vui lòng nhập ngày lớn hơn ngày hiện tại");
            $('#errorNgayDa').show();

        } else {
            $('#errorNgayDa').hide();

        }
    }
    return check;


}



function showAlert(mess) {

    $("#alertMessageSuccess").text(mess);
    $("#alertSuccess").show();
    setTimeout(function () {
        $("#alertSuccess").hide(); // Hide the notification box with fade-out effect
    }, 3500); // 4 seconds delay
}


