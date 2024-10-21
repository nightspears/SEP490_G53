function loadData() {
    $.ajax({
        url: "https://localhost:5000/api/Product/GetSanPham",
        method: "GET",
        dataType: "json",
        success: function (res) {
            debugger;
            var container = $('#lstSanPham').empty();
            $.each(res, function (index, item) {
                var Avatar = "";
                if (item.image == null || item.image == "" || item.image == undefined) {
                    Avatar = "/image/imagelogo/ImageFail.jpg"
                } else {
                    Avatar = item.image;
                }
                var price = 0;
                debugger
                var discout = "";
                var priceText = "";
                if (item.discoutPercent != null && item.discoutPercent != "" && item.discout != 0) {
                    price = (item.price - (item.price * item.discoutPercent / 100));
                    debugger
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

                            <div class="d-flex" style="float:right ;margin :40px 20px 20px 0">
                                <button class="btn btn-outline-warning text-center">
                                    <i class="fa fa-shopping-cart" style="color:#eb3636 ;font-size:20px;" onclick="AddToCart(${item.productId})"></i>
                                </button>
                            </div>
                        </div>
                     </div>
                   </div>`

                container.append(html);
            });
            format();

        },
        error: function (res) {
        }
    });
}
$(document).ready(function () {
    loadData();
    debugger

   

});

function format() {
    debugger
    // Hàm định dạng tiền tệ Việt Nam
    function formatCurrency(value) {
        return parseInt(value).toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
    }

    // Lấy tất cả các phần tử có class 'price'
    const prices = document.querySelectorAll('.price');

    // Lặp qua từng phần tử và định dạng giá trị dựa trên thuộc tính 'data-price'
    prices.forEach(function (priceElement) {
        const priceValue = priceElement.getAttribute('data-price');
        if (priceValue) {
            priceElement.innerText = formatCurrency(priceValue);  // Định dạng và hiển thị lại giá trị
        }
    });

}


function saveCartToLocalStorage(cartItems) {
    debugger
    const cartJson = JSON.stringify(cartItems);
    localStorage.setItem("cartProduct", cartJson);
}


function getCartFromLocalStorage() {
    const CartJson = localStorage.getItem("cartProduct");
    if (CartJson) {
        return JSON.parse(CartJson);
    }
    debugger
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
        debugger
        saveCartToLocalStorage(cartItems);
        debugger
        alert("thêm thành công")
    }
}
