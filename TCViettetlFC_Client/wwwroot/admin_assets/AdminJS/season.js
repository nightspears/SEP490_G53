
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
function CheckAll(item) {
    var isChecked = $(item).is(':checked');
    var checkboxes = $(item).closest('table').find('.form-check-input');
    checkboxes.prop('checked', isChecked);
};
$(document).on("click", "#confirmXoa", function () {
    debugger
    $("#idXoa").val("");
    var id = $(this).data("id");
    $("#idXoa").val(id);
    debugger

});
$(document).on("click", "#btnXoa", function () {

    var id = $("#idXoa").val();
    debugger
    var url = "https://tcvtfcapi.azurewebsites.net/api/Season/DeleteSeason/" + id;
    $.ajax({
        url: url,
        method: "Delete",
        success: function (res) {
            debugger;
            $("#delete_modal").modal("hide");
            $(".modal-backdrop").hide();
            showAlert("Xóa thành công ");
            loadData();
        },
        error: function (res) {
            alert("xóa thất bại ")
        }
    });

});

function loadData() {
    debugger

    $.ajax({
        url: "https://tcvtfcapi.azurewebsites.net/api/Season/GetSeason",
        method: "GET",
        dataType: "json",
        success: function (res) {
            debugger
            var table = $('.datatable').DataTable();

            table.clear();
            $.each(res, function (index, item) {

                var rowHtml = `
                    <tr>
                        <td>
                            <div class="form-check">
                                <input class="form-check-input" type="checkbox" value="" data-id="${item.seasonId}">
                            </div>
                        </td>
                        <td>${item.seasonName}</td>
                        <td>${item.startFormatted} <br><small></small></td>
                         <td>${item.endFormatted} <br><small></small></td>
                        <td class="text-center">
                            <div class="status-toggle d-flex justify-content-center">
                                <input type="checkbox" id="status_${item.seasonId}" class="check" ${item.status === 1 ? 'checked' : ''}>
                                <label for="status_${item.seasonId}" class="checktoggle">checkbox</label>
                            </div>
                        </td>
                        <td class="text-center">
                            <div class="actions">
                                <a onclick="modalEditOrCreate(${item.seasonId})" class="btn btn-sm bg-success-light me-2">
                                    <i class="fe fe-pencil"></i> Sửa
                                </a>
                                <a class="btn btn-sm bg-danger-light" data-bs-toggle="modal" data-id="${item.seasonId}" id="confirmXoa" href="#delete_modal">
                                    <i class="fe fe-trash"></i> Xóa
                                </a>
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


// change Image
//$('.change-photo-btn').on('click', function () {
//    $('#fileInput').click();
//});

//$('#fileInput').on('change', function (event) {
//    const file = event.target.files[0];
//    if (file) {
//        const reader = new FileReader();
//        reader.onload = function (e) {
//            $('#ImgAvt').attr('src', e.target.result);
//        };
//        reader.readAsDataURL(file);
//    }
//});

function resetForm() {


    $('#btnNomal').show();
    $('#btnLoading').hide();

    $('#idSeason').val("");
    $('#tenMuaGiai').val("");
    $('#startdate').val("");
    $('#endDate').val("");
    $('#status').prop('checked', true);

    $('#errorTenMua').hide();
    $('#errorNgayEnd').hide();
    $('#errorNgayStart').hide();

}
function modalEditOrCreate(id) {
    debugger
    resetForm();

    if (id != 0 && id != undefined) {
        $("#titleModal").text("Cập nhật mùa giải")

        var url = "https://tcvtfcapi.azurewebsites.net/api/Season/GetSeasonById?id=" + id;
        $.ajax({
            url: url,
            method: "get",
            success: function (res) {
                debugger
                var a = new Date();
                $('#idSeason').val(res.seasonId);
                $('#tenMuaGiai').val(res.seasonName);

                let startYear = res.startYear;
                let formatStartYear = startYear.split("T")[0];

                $('#startdate').val(formatStartYear);


                let endYear = res.endYear; 
                let formattedEndDate = endYear.split("T")[0]; 

                $('#endDate').val(formattedEndDate);

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

        $("#titleModal").text("Thêm mùa giải")

        $('#idSeason').hide()
      
        $(".modal-backdrop").remove();
        $('#modalEditorCreate').modal("show")

    }

}

function EditOrCreate() {

    if (validation()) {


        $('#btnNomal').hide();
        $('#btnLoading').show();
        var id = $('#idSeason').val();

        var formData = new FormData();

        formData.append('SeasonName', $('#tenMuaGiai').val());

        formData.append('StartYear', $('#startdate').val());
        formData.append('EndYear', $('#endDate').val());
        if ($('#status').prop('checked')) {
            formData.append('Status', 1);
        } else {
            formData.append('Status', 2);
        }
        var data = {
            MuaGiai: formData.get("SeasonName"),
            EndYear: formData.get("EndYear"),
            StartYear: formData.get("StartYear")
        }

        if (id != 0 && id != undefined) {
            debugger
            checkExist(data, function (exist, mess, type) {

                if (exist) {

                } else {
                    $.ajax({
                        url: 'https://tcvtfcapi.azurewebsites.net/api/Season/UpdateSeason/' + id,
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

              
            })
           

        } else {


            $.ajax({
                url: 'https://tcvtfcapi.azurewebsites.net/api/Season/AddSeason',
                type: 'post',
                data: formData,
                contentType: false, // Không thiết lập Content-Type
                processData: false, // Không xử lý dữ liệu
                success: function (response) {
                    loadData();
                    showAlert("Thêm mới thành công");
                    $("#modalEditorCreate").modal("hide");
                },
                error: function (error) {
                    debugger
                }
            });
        }
    }
}


function checkExist(data, callback) {
    $.ajax({
        url: 'https://tcvtfcapi.azurewebsites.net/api/Matches/CheckExist',
        type: 'post',
        data: JSON.stringify(data),
        contentType: 'application/json',
        success: function (res) {
            debugger
            callback(res.exists, res.mess, res.type); // Giả sử API trả về { exists: true/false }
        },
        error: function (error) {
            console.error("Error during checkExist", error);
            callback(false); // Mặc định là không tồn tại nếu lỗi xảy ra
        }
    });
}

function validation() {

    var check = true;
    if ($('#tenMuaGiai').val().trim() == "" || $('#tenMuaGiai').val() == null) {
        check = false;
        $('#errorTenMua').show();

    } else {
        $('#errorTenMua').hide();

    }
    let endDate = $('#endDate').val().trim();
    if (endDate === "" || endDate == null) {
        check = false;
        $('#errorNgayEnd').show();

    } else {
        $('#errorNgayEnd').hide();

    }

    let startdate = $('#startdate').val().trim();
    if (startdate === "" || startdate == null) {
        check = false;
        $('#errorNgayStart').show();

    }
    else {
        $('#errorNgayStart').hide();

        let start = new Date(startdate);
        let end = new Date(endDate);
        if (start > end) {
            check = false;
            $('#errorNgayStart').text("Ngày bắt đầu  phải trước ngày kết thúc");

            $('#errorNgayStart').show();

        } else {
            $('#errorNgayStart').hide();

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



