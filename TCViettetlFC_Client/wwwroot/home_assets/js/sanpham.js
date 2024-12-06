
var currentPage = 1;
var totalItem = 0;
function loadData(id, url) {
    var link = "";
    if (url.trim() == "" || url == null) {
        link = "https://localhost:5000/api/Product/GetSanPham";
    } else {
        link = url;
    }

    $.ajax({
        url: link,
        data: { cid: id },
        method: "GET",
        dataType: "json",
        success: function (res) {

            var container = $('#lstSanPham').empty();
            $.each(res, function (index, item) {
                if (index === 12) {
                    return false; 
                }
                var Avatar = "";
                if (item.image == null || item.image == "" || item.image == undefined) {
                    Avatar = "/image/imagelogo/ImageFail.jpg"
                } else {
                    Avatar = item.image;
                }
                var price = 0;

                var discout = "";
                var priceText = "";
                if (item.discoutPercent != null && item.discoutPercent != "" && item.discout != 0) {
                    price = (item.price - (item.price * item.discoutPercent / 100));
                    discout = `<div class="discount-badge">-${item.discoutPercent}%</div> `;

                    priceText = `<span class="price" data-price="${price}">${price}</span> 
                                 <del class="price" style="color:#333" data-price="${item.price}"></del>`;
                } else {
                    price = item.price;
                    priceText = `<span class="price" data-price="${price}">${price}</span> `;
                }
                var html = `<div class="col-md-4 col-sm-6 col-12 mb-4">
                    <div class="card position-relative">
                        <a href="/public/ChiTietSanPham/${item.productId}">
                            <img src="${Avatar}" class="card-img-top product-img" alt="Sản phẩm 1">
                        </a>
                       ${discout}
                        <div class="card-body mt-3">
                            <a href="/public/ChiTietSanPham/${item.productId}">
                                    <h5 class="product-title  text-center">${item.productName}</h5>
                                     <p class="product-price text-center" >
                                     ${priceText}
 
                                   </p>
                            </a>

                            <div class="d-flex" id="btnXem" style="float:right ;margin :40px 20px 20px 0 ; display:none">
                                <a class="btn btn-outline-warning text-center" href="/public/ChiTietSanPham/${item.productId}" >
                                  Chọn thuộc tính
                                </a>
                            </div>
                        </div>
                     </div>
                   </div>`

                container.append(html);
               
            });
            if (url.trim() == "" || url == null) {
                totalItem = res.length;
            }
            renderPagination(totalItem)

            $("#paging").show();
            format();

        },
        error: function (res) {
            debugger;
        }

    });
}




$(document).on("click", ".page-btn", function () {
    debugger
    var page = $(this).data("page");
    currentPage = page;

    let filters = [];
    let sortOption = '';
    var txtSearch = $("#search").val().trim();
    var cateID = $("#idCategory").val();
    if (cateID == undefined || cateID == null) {
        cateID = 0;
    }

    var url = "https://localhost:5000/api/Product/GetSanPham?cid=" + cateID;
    if (txtSearch) {
        url += "&$filter=contains(ProductName, '" + encodeURIComponent(txtSearch) + "')";
    }
    $('.filter-checkbox:checked').each(function () {
        let filterValue = $(this).val();
        filters.push(`${filterValue.trim()}`);
    });
    if (filters.length > 0) {
        if (txtSearch) {
            let filterQuery = filters.join(' or ');
            url += `and (${filterQuery})`;
        } else {
            let filterQuery = filters.join(' or ');
            url += `&$filter=(${filterQuery})`;
        }

    }

    $('input.SortProduct:checked').each(function () {

        if ($('#sort-hangMoi').is(':checked')) {
            debugger
            sortOption = ` and (discoutPercent ne null)`
        } else {
            sortOption = $(this).val();

        }
    });
    debugger
    if (sortOption) {
        url += sortOption;
    }
    var skip = 12 * (currentPage-1);
    url += `&$skip=${skip}&top=12`
    loadData(cateID, url);

});
function renderPagination(totalItems) {
    debugger;

    let totalPages = Math.ceil(totalItems / 12);

    const pagination = $("#page-buttons");
    pagination.empty();

    // Tạo các nút trang
    for (let i = 1; i <= totalPages; i++) {
        pagination.append(
            `<button  class="btn btn-outline-primary page-btn ${i === currentPage ? "active" : ""}" data-page="${i}">${i}</button>`
        );
    }
    debugger;

}
function loadCategory() {
    debugger
    $.ajax({
        url: "https://localhost:5000/api/Category/GetCategory",
        method: "GET",
        dataType: "json",
        success: function (res) {
            var lstCate = $("#lstCate").empty();
            var tatca = `<li class="category_product active"  onclick="showProductByCate(0)" >
                                <a >Tất cả</a>
                            </li>`;

            lstCate.append(tatca);
            $.each(res, function (index, item) {

                var html = `<li class="category_product" onclick="showProductByCate(${item.categoryId})" data-cid="${item.categoryId}">
                                <a data-cid="${item.categoryId}">${item.categoryName}</a>
                            </li>`;

                lstCate.append(html);
            });

        },
        error: function (res) {
            console.error("Error loading data", res);
        }
    });
}


function showProductByCate(id) {
    
    $("#idCategory").val(id);
    $('.filter-checkbox').prop('checked', false);
    $('.SortProduct').prop('checked', false);
    loadData(id, "");

}

$(document).ready(function () {
    loadData(0, "");
    loadCategory();
    addActive();
});

function addActive() {
    $(document).on('click', '.category_product', function () {
        $('.category_product').removeClass('active');
        $(this).addClass('active');
    });
}
function format() {
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


function saveCartToLocalStorage(cartItems) {
    const cartJson = JSON.stringify(cartItems);
    localStorage.setItem("cartProduct", cartJson);
}


function getCartFromLocalStorage() {
    const CartJson = localStorage.getItem("cartProduct");
    if (CartJson) {
        return JSON.parse(CartJson);
    }
    return [];
}


function AddToCart(id) {
    debugger
    if (validateAddCart()) {
        var productId = id;
        var size = $("#sizeSp").val();
        var Avartar = $("#imgAvatar").attr("src");
        var Price = $("#giaSP").data("price");
        var TenSP = $("#TenSP").text();
        var soLuong = 1;

        var cartItems = getCartFromLocalStorage();

        var existingProduct = cartItems.find(function (item) {
            return item.productId === productId && item.size === size;
        });

        if (existingProduct) {
            existingProduct.quantity += 1;
        }
        else {
            var Product = {
                productId: productId,
                size: size,
                avartar: Avartar,
                price: Price,
                nameProduct: TenSP,
                quantity: soLuong,
            };
            cartItems.push(Product);
        }
        saveCartToLocalStorage(cartItems);
        alert("thêm thành công")
    }
}


function toggleFilter() {
    let filters = [];
    let sortOption = '';
    var txtSearch = $("#search").val().trim();
    var cateID = $("#idCategory").val();
    if (cateID == undefined || cateID == null) {
        cateID = 0;
    }

    var url = "https://localhost:5000/api/Product/GetSanPham?cid=" + cateID;
    if (txtSearch) {
        url += "&$filter=contains(ProductName, '" + encodeURIComponent(txtSearch) + "')";
    }
    $('.filter-checkbox:checked').each(function () {
        let filterValue = $(this).val();
        filters.push(`${filterValue.trim()}`);
    });
    if (filters.length > 0) {
        if (txtSearch) {
            let filterQuery = filters.join(' or ');
            url += `and (${filterQuery})`;
        } else {
            let filterQuery = filters.join(' or ');
            url += `&$filter=(${filterQuery})`;
        }

    }

    $('input.SortProduct:checked').each(function () {

        if ($('#sort-hangMoi').is(':checked')) {
            debugger
            sortOption = ` and (discoutPercent ne null)`
        } else {
            sortOption = $(this).val();

        }
    });

    if (sortOption) {
        url += sortOption;
    }

    loadData(cateID, url);

}

