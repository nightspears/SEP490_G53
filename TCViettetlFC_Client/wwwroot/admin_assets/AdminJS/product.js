
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
    fillDataModal();

});
function CheckAll(item) {
    var isChecked = $(item).is(':checked');
    var checkboxes = $(item).closest('table').find('.form-check-input');
    checkboxes.prop('checked', isChecked);
};

function loadData() {
    debugger;
    $.ajax({
        url: "https://localhost:5000/api/Product/GetProduct",
        method: "GET",
        dataType: "json",
        success: function (res) {
            debugger;
            //var tbody = $("#tbody"); 
            //tbody.empty(); 
            var table = $('.datatable').DataTable();

            table.clear();
            $.each(res, function (index, item) {
                /*  var tenSan = (item.isHome === true ? 'SVĐ Mỹ Đình' : item.stadiumName);*/
                var Avatar = "";
                if (item.image == null || item.image == "" || item.image == undefined) {
                    Avatar = "/image/imagelogo/ImageFail.jpg"
                } else {
                    Avatar = item.image;
                }
                debugger
                var html = `
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
                        src="${Avatar}"
                        alt="User Image">
                </a>
                <a href="profile.html">${item.productName}</a>
            </h2>
        </td>
        <td> ${item.categoryName}</td>
        <td>${item.seasonName} <br><small></small></td>
        <td class="price" data-price="${item.price}">${item.price}</td>
        <td class="text-center">
            <div class="status-toggle d-flex justify-content-center">
                <input type="checkbox" data-sid = "${item.productId}" id="status_${item.productId}" class="check" ${item.status === 1 ? 'checked' : ''}>
                    <label for="status_${item.productId}" class="checktoggle">checkbox</label>
            </div>
        </td>
        <td class="text-center">
            <div class="actions">
                <a onclick="modalEditOrCreate(${item.productId})" class="btn btn-sm bg-success-light me-2">
                    <i class="fe fe-pencil"></i> Sửa
                </a>
                <a class="btn btn-sm bg-danger-light" data-bs-toggle="modal" data-id="${item.productId}" id="confirmXoa" href="#delete_modal">
                    <i class="fe fe-trash"></i> Xóa
                </a>
            </div>
        </td>
    </tr>`;

                /* tbody.append(html);*/
                table.row.add($(html));

            });
            table.draw();
            format()


        },
        error: function (res) {
            debugger;
        }
    });

}
function format() {
    debugger
    function formatCurrency(value) {
        return parseInt(value).toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
    }

    const prices = document.querySelectorAll('.price');

    prices.forEach(function (priceElement) {
        const priceValue = priceElement.getAttribute('data-price');
        if (priceValue) {
            priceElement.innerText = formatCurrency(priceValue);
        }
    });

}

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
    var url = "https://localhost:5000/api/Product/DeleteProduct/" + id;
    $.ajax({
        url: url,
        method: "Delete",
        success: function (res) {
            debugger;
            $("#delete_modal").modal("hide");
            showAlert("xóa thành công ");
            loadData();
        },
        error: function (res) {
            showAlert("xóa thất bại ")
        }
    });

});

// show modal and fill data (modal chung cho edit và create)
function resetForm() {
    $('#btnNomal').show();
    $('#btnLoading').hide();
    $('#fileInput').val('');
    $('#previewContainer').empty();
    $('#idSanPham').val("");
    $('#tensp').val("");
    $('#muaGiai').val(0);
    $('#theLoai').val(0);
    $('#moTa').val("");
    $('#giaSp').val("");
    $('#kichthuoc').val("");
    $('#chatlieu').val("");
    $('#maGiamGia').val(0);
    $('#errorTen').hide();
    $('#errorSeason').hide();
    $('#errorCate').hide();
    $('#errorGia').hide();
    $('#errorSize').hide();

    $('#status').prop('checked', true);
}
function modalEditOrCreate(id) {
    debugger
    resetForm();


    if (id != 0 && id != undefined) {
        $("#titleModal").text("Cập nhật sản phẩm")

        var url = "https://localhost:5000/api/Product/GetProductById?id=" + id;
        $.ajax({
            url: url,
            method: "GET",
            success: function (res) {
                debugger
                var avatar = "";
                if (res.product.image == null || res.product.image == "" || res.product.image == undefined) {
                    avatar = "/image/imagelogo/ImageFail.jpg"
                } else {
                    avatar = res.product.image;
                }
                $('#idSanPham').val(res.product.productId);
                $("#ImgAvt").attr("src", avatar);
                $('#tensp').val(res.product.productName);
                $('#muaGiai').val(res.product.seasonId);
                $('#theLoai').val(res.product.categoryId);
                $('#moTa').val(res.product.description);
                $('#giaSp').val(res.product.price);
                $('#kichthuoc').val(res.product.size);
                $('#chatlieu').val(res.product.material);
                if (res.product.discountId == null) {
                    $('#maGiamGia').val(0);
                } else {
                    $('#maGiamGia').val(res.product.discountId);

                }
                if (res.product.status == 1) {
                    $('#status').prop('checked', true);
                } else {
                    $('#status').prop('checked', false);

                }
                $(".modal-backdrop").hide();
                loadImageOther(res.pFile);
                $('#modalEditorCreate').modal("show")

            },
            error: function (res) {
                alert("Lỗi ")
            }
        });

    }
    else {
        debugger
        $("#fileInputImg").val("");
        $("#ImgAvt").attr("src", "/image/imagelogo/logoviettel.jpg")
        $("#titleModal").text("Thêm sản phẩm")


        $('#idTran').hide()
        $(".modal-backdrop").remove();
        $('#modalEditorCreate').modal("show")

    }

}

function loadImageOther(data) {
    const preview = $('#previewContainer').empty();
    debugger
    $.each(data, function (index, item) {
        const $imgWrapper = $('<div>').css({
            'border': '2px solid #ccc',
            'padding': '5px',
            'margin': '5px',
            'border-radius': '10px',
            'display': 'inline-block',
            'position': 'relative'
        }).attr('data-id', item.fileId);

        const $img = $('<img>').attr('src', item.filePath).css({
            'width': '150px',
            'height': '150px',
            'object-fit': 'cover'
        }).addClass('rounded-5');

        const $deleteBtn = $('<button>').text('X').css({
            'position': 'absolute',
            'top': '0',
            'right': '0',
            'background-color': 'white',
            'color': 'black',
            'border': 'none',
            'border-radius': '50%',
            'width': '20px',
            'height': '20px',
            'cursor': 'pointer'
        });

        $deleteBtn.on('click', function () {
            $imgWrapper.remove();
            /*currentImages--;*/
        });
        $imgWrapper.append($img).append($deleteBtn);
        preview.append($imgWrapper);
        /* currentImages++;*/
    });
}

$(document).on("click", ".change-photo-btn", function () {
    debugger
    $('#fileInput').click();
})

var fileList = [];

function validateData() {
    var check = true;

    if ($('#tensp').val().trim() == "" || $('#tensp').val() == undefined) {
        check = false;
        $('#errorTen').show();
    } else {
        $('#errorTen').hide();
    }

    if ($('#muaGiai').val().trim() == 0 || $('#muaGiai').val().trim() == undefined) {
        check = false;
        $('#errorSeason').show();
    } else {
        $('#errorSeason').hide();

    }

    if ($('#theLoai').val().trim() == 0 || $('#theLoai').val().trim() == undefined) {
        check = false;
        $('#errorCate').show();
    } else {
        $('#errorCate').hide();

    }


    if ($('#giaSp').val().trim() == "" || $('#giaSp').val() == undefined) {
        check = false;
        $('#errorGia').show();
    } else {
        $('#errorGia').hide();
    }

    if ($('#kichthuoc').val().trim() == "" || $('#kichthuoc').val() == undefined) {
        check = false;
        $('#errorSize').show();
    } else {
        $('#errorSize').hide();
    }


    return check;
}

// edit or create sản phẩm 
function EditOrCreate() {
    if (validateData()) {
        $('#btnNomal').hide();
        $('#btnLoading').show();
        debugger
        var id = $('#idSanPham').val();
        var formData = new FormData();
        //xử lý lay file Avatar
        var inputElement = $('#fileInputImg');
        var files = inputElement.prop('files');
        if (files.length > 0) {
            var file = files[0];
            formData.append('Avatar', file);
        }

        formData.append('ProductName', $('#tensp').val());
        formData.append('SeasonId', $('#muaGiai').val());
        formData.append('CategoryId', $('#theLoai').val());
        formData.append('Description', $('#moTa').val());
        formData.append('Price', $('#giaSp').val());
        formData.append('Size', $('#kichthuoc').val());
        formData.append('Material', $('#chatlieu').val());
        formData.append('DiscountId', $('#maGiamGia').val());

        if ($('#status').prop('checked')) {
            formData.append('Status', 1);
        } else {
            formData.append('Status', 2);
        }

        // xử lý lấy listFile
        var inputFileList = $('#fileInput');
        var filesList = inputFileList.prop('files');

        if (filesList.length > 0) {
            var ListFile = [];
            $.each(filesList, function (index, file) {
                ListFile.push({
                    FileName: file.name,
                    File: file
                });
            });
            $.each(ListFile, function (index, file) {
                formData.append(`DataFile[${index}].FileNamme`, file.FileNamme);
                formData.append(`DataFile[${index}].File`, file.File);
            });
        }
        $('#fileInput').val("");

        var dataIdList = [];
        debugger
        $('#previewContainer > div').each(function () {
            debugger
            var dataId = $(this).attr('data-id');
            if (dataId != 0 && dataId != undefined) {
                dataIdList.push(dataId);
            }
        });
        if (dataIdList.length > 0) {
            debugger
            dataIdList.forEach(function (id) {
                formData.append('ListExist[]', id);
            });
        };


        if (id != 0 && id != undefined) {
            debugger
            $.ajax({
                url: 'https://localhost:5000/api/Product/UpdateProduct/' + id,
                type: 'Put',
                data: formData,
                contentType: false,
                processData: false,
                success: function (response) {
                    loadData();
                    showAlert("Cập nhật sản phẩm thành công");
                    $("#modalEditorCreate").modal("hide");
                    fileList = [];
                    debugger
                },
                error: function (error) {
                    debugger
                }
            });

        }
        else {
            $.ajax({
                url: 'https://localhost:5000/api/Product/AddProduct',
                type: 'post',
                data: formData,
                contentType: false,
                processData: false,
                success: function (response) {
                    debugger
                    loadData();
                    showAlert("Thêm mới sản phẩm thành công");
                    $("#modalEditorCreate").modal("hide");
                    $('#fileInput').val('');
                    fileList = [];
                },
                error: function (error) {
                    debugger
                }
            });
        }
    }

}




function fillDataModal() {
    $.ajax({
        url: "https://localhost:5000/api/Product/GetJson",
        method: "GET",
        success: function (res) {
            debugger
            $('#muaGiai').empty();
            $('#muaGiai').append($('<option>', {
                value: 0,
                text: "-- Chọn mùa giải --"
            }));
            $.each(res.season, function (i, season) {
                $('#muaGiai').append($('<option>', {
                    value: season.seasonId,
                    text: season.seasonName
                }));
            });

            $('#theLoai').empty();
            $('#theLoai').append($('<option>', {
                value: 0,
                text: "-- Chọn thể loại --"
            }));
            $.each(res.cate, function (i, cate) {
                $('#theLoai').append($('<option>', {
                    value: cate.categoryId,
                    text: cate.categoryName
                }));
            });
            // fill data vào mã giảm giá

            $('#maGiamGia').empty();
            $('#maGiamGia').append($('<option>', {
                value: 0,
                text: "-- Chọn mã giảm giá --"
            }));
            debugger
            $.each(res.dis, function (i, d) {
                $('#maGiamGia').append($('<option>', {
                    value: d.discountId,
                    text: d.discountName + " - " + d.discountPercent + "%"
                }));
            });


        },
        error: function (res) {

        }
    });
}


$('.change-photo').on('click', function () {
    $('#fileInputImg').click();
});
$('#fileInputImg').on('change', function (event) {
    const file = event.target.files[0];
    if (file) {
        const reader = new FileReader();
        reader.onload = function (e) {
            $('#ImgAvt').attr('src', e.target.result);
        };
        reader.readAsDataURL(file);
    }
});



$('#fileInput').on('change', function (event) {
    const files = event.target.files;
    const $previewContainer = $('#previewContainer');

    const existingImages = $previewContainer.find('img').map(function () {
        return $(this).attr('src');
    }).get();

    let currentImages = existingImages.length;

    const maxImages = 6;
    const remainingSlots = maxImages - currentImages;

    if (remainingSlots <= 0) {
        alert('Bạn chỉ có thể thêm tối đa 6 ảnh.');
        return;
    }
    debugger
    $.each(Array.from(files).slice(0, remainingSlots), function (index, file) {
        const reader = new FileReader();

        reader.onload = function (e) {
            const imageUrl = e.target.result;

            if (existingImages.includes(imageUrl)) {
                alert('Ảnh này đã được thêm trước đó.');
                return;
            }

            const $imgWrapper = $('<div>').css({
                'border': '2px solid #ccc',
                'padding': '5px',
                'margin': '5px',
                'border-radius': '10px',
                'display': 'inline-block',
                'position': 'relative'
            }).attr('data-id', 0);

            const $img = $('<img>').attr('src', imageUrl).css({
                'width': '150px',
                'height': '150px',
                'object-fit': 'cover'
            }).addClass('rounded-5');

            const $deleteBtn = $('<button>').text('X').css({
                'position': 'absolute',
                'top': '0',
                'right': '0',
                'background-color': 'white',
                'color': 'black',
                'border': 'none',
                'border-radius': '50%',
                'width': '20px',
                'height': '20px',
                'cursor': 'pointer'
            });

            $deleteBtn.on('click', function () {
                $imgWrapper.remove();
                currentImages--;

                fileList = fileList.filter(f => f !== file);

                updateFileInput();
            });
            $imgWrapper.append($img).append($deleteBtn);
            $previewContainer.append($imgWrapper);
            currentImages++;

            fileList.push(file);

            updateFileInput();
        };

        reader.readAsDataURL(file);
    });
});

function updateFileInput() {
    const dataTransfer = new DataTransfer();
    debugger
    $.each(fileList, function (index, file) {
        dataTransfer.items.add(file);
    });

    $('#fileInput')[0].files = dataTransfer.files;
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
    var id = checkbox.data("sid");
    var formData = new FormData();

    formData.append('id', id);
    formData.append('status', status);
    debugger
    $.ajax({
        url: 'https://localhost:5000/api/Product/updateStatus',
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