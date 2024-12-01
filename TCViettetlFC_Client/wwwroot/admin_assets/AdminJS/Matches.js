
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
    var url = "https://localhost:5000/api/Matches/DeleteMatches/" + id;
    $.ajax({
        url: url,
        method: "Delete",
        success: function (res) {
            debugger;
            $("#delete_modal").modal("hide");
            $(".modal-backdrop").hide();
            showAlert("xóa thành công ");
            loadData();
        },
        error: function (res) {
            alert("xóa thất bại ")
        }
    });

});

function loadData() {
    $.ajax({
        url: "https://localhost:5000/api/Matches/GetMatches",
        method: "GET",
        dataType: "json",
        success: function (res) {
           
            var table = $('.datatable').DataTable();

            table.clear();
            $.each(res, function (index, item) {
                var tenSan = (item.isHome === true ? 'SVĐ Mỹ Đình' : item.stadiumName);
                var logoUrl = item.logoUrl ? item.logoUrl : "/image/imagelogo/icon-image-not-found-free-vector.jpg";

                var matchDate = new Date(item.matchDate);
                var dateFormatted = matchDate.toLocaleDateString('en-GB'); // Formats as dd/MM/yyyy
                var timeFormatted = matchDate.toLocaleTimeString('en-GB', { hour: '2-digit', minute: '2-digit' }); // Formats as HH:mm

                var rowHtml = `
                    <tr>
                        <td>
                            <div class="form-check">
                                <input class="form-check-input" type="checkbox" value="" data-id="${item.id}">
                            </div>
                        </td>
                        <td>
                            <h2 class="table-avatar">
                                <a href="profile.html" class="avatar avatar-sm me-2">
                                    <img class="avatar-img rounded-circle"
                                        src="${logoUrl}"
                                        alt="User Image">
                                </a>
                                <a href="profile.html">${item.opponentName}</a>
                            </h2>
                        </td>
                        <td>${tenSan}</td>
                        <td>${dateFormatted}</td>
                         <td>${timeFormatted}</td>
                        <td>${item.isHome === true ? 'Sân nhà' : 'Sân khách'}</td>
                        <td class="text-center">
                            <div class="status-toggle d-flex justify-content-center">
                                <input type="checkbox" data-mid="${item.id}" id="status_${item.id}" class="check" ${item.status === 1 ? 'checked' : ''}>
                                <label for="status_${item.id}" class="checktoggle">checkbox</label>
                            </div>
                        </td>
                        <td class="text-center">
                            <div class="actions">
                                <a onclick="modalEditOrCreate(${item.id})" class="btn btn-sm bg-success-light me-2">
                                    <i class="fe fe-pencil"></i> Sửa
                                </a>
                                <a class="btn btn-sm bg-danger-light" data-bs-toggle="modal" data-id="${item.id}" id="confirmXoa" href="#delete_modal">
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
$('.change-photo-btn').on('click', function () {
    $('#fileInput').click();
});

$('#fileInput').on('change', function (event) {
    const file = event.target.files[0];
    if (file) {
        const reader = new FileReader();
        reader.onload = function (e) {
            $('#ImgAvt').attr('src', e.target.result);
        };
        reader.readAsDataURL(file);
    }
});
$('#isHome').change(function () {
    var isHome = $(this).val(); 

    if (isHome == 1) {
        $('#tenSan').val("SVĐ Mỹ Đình");
        $('#tenSan').prop('readonly', true); 
    } else if (isHome == 2) {
        $('#tenSan').val(""); 
        $('#tenSan').prop('readonly', false); 
        $('#tenSan').attr('placeholder', 'Nhập tên sân ');
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

        var url = "https://localhost:5000/api/Matches/GetMatchesById?id=" + id;
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

function checkExist(data , callback) {
    $.ajax({
        url: 'https://localhost:5000/api/Matches/CheckExist',
        type: 'post',
        data: JSON.stringify(data),
        contentType: 'application/json',
        success: function (res) {
            debugger
            callback(res.exists, res.mess,res.type); // Giả sử API trả về { exists: true/false }
        },
        error: function (error) {
            console.error("Error during checkExist", error);
            callback(false); // Mặc định là không tồn tại nếu lỗi xảy ra
        }
    });
}

function AddMatches(formData) {

    var data = {
        NgayDa: formData.get("MatchDate"),
        TenDoiThu: formData.get("OpponentName"),
        TenSan: formData.get("StadiumName")
    }
    debugger
    checkExist(data, function (exist, mess, type) {
        debugger
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
            $('#errorTenSan').hide();
            $.ajax({
                url: 'https://localhost:5000/api/Matches/AddMatches',
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
    });

   
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
                url: 'https://localhost:5000/api/Matches/UpdateMatches/' + id,
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

$(document).on("change", ".check", function () {

    var checkbox = $(this);
    var status = 1;
    if (checkbox.prop('checked')) {
        status = 1
    } else {
        status = 2;
    }
    var id = checkbox.data("mid");
    var formData = new FormData();

    formData.append('id', id);
    formData.append('status', status);
    debugger
    $.ajax({
        url: 'https://localhost:5000/api/Matches/updateStatus',
        type: 'post',
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            debugger
            loadData();
            showAlert("Cập nhật trạng thái thành công");
        },
        error: function (error) {
            debugger
        }
    });

});

