﻿


function getCartFromLocalStorage() {
    const cartJson = localStorage.getItem("cartProduct");

    if (cartJson) {
        const cartData = JSON.parse(cartJson);
        const now = new Date().getTime();

        // Kiểm tra thời gian hết hạn
        if (now < cartData.expiration) {
            return cartData.items;
        } else {
            localStorage.removeItem("cartProduct");
            return [];
        }
    }
    return [];
}
$(document).ready(function () {
    loadData();
    checkTableData();
    getLienQuan();
    format();

});
function loadData() {

    var cartItems = [];
    cartItems = getCartFromLocalStorage();


    var tbody = $('#bodyData').empty();
    $.each(cartItems, function (index, item) {
        var Image = "";
        if (item.avartar == null || item.avartar == "" || item.avartar == undefined) {
            Image = "/image/imagelogo/ImageFail.jpg"
        } else {
            Image = item.avartar;
        }


        var total = parseFloat(item.price) * item.quantity;
        var teninao = "";


        if (item.TenInAo.trim() == "" || item.TenInAo.trim() == null) {
            
            teninao = "Không in tên"
        } else {
            teninao = item.TenInAo;
        }
        var sizeTen = "";
        if (item.shirtNumber != null) {
            sizeTen = ` <span  title="${item.size} - ${teninao}" >${item.size} - ${teninao} </span> <span></span>`;
        } else {
            sizeTen = ` <span  title="${item.size}" >${item.size}</span> <span></span>`
        }
        var html = `
   <div class="cart-item">
    <div class="product-checkbox">
        <input class="form-check-input" type="checkbox" data-pid="${item.Item_id}">
    </div>
    <div class="product-image-info">
        <a href="/public/ChiTietSanPham/${item.productId}">
            <img src="${Image}" alt="" class="ImgProduct">
        </a>
        <div class="product-info">
            <span class="product-name" title="${item.nameProduct}">${item.nameProduct}</span>
            ${sizeTen}
        </div>
    </div>
    <div class="product-quantity">
        <button class="quantity-btn" onclick="updateCart(${item.Item_id}, -1)">-</button>
        <input type="text" id="quantity_${item.Item_id}" value="${item.quantity}" class="quantity-input" min="1">
        <button class="quantity-btn" onclick="updateCart(${item.Item_id}, 1)">+</button>
    </div>
    <div class="price" data-price="${item.price}">${item.price}</div>
   
    <div class="product-actions">
        <button class="btnDeleteCart" data-id="${item.Item_id}">Xóa</button>
    </div>
</div>
`;

        tbody.append(html);
    });
    if (cartItems == null || cartItems.length == 0) {
        var noDataMessage = document.getElementById('noDataMessage');
        noDataMessage.style.display = 'block';
    } else {
        var noDataMessage = document.getElementById('noDataMessage');
        noDataMessage.style.display = 'none';
    }

}

$(document).on("click", ".btnDeleteCart", function () {

    var pid = $(this).data("id");
    
    removeProductByIdAndSize(pid);
    loadData();
    format();
    getLienQuan();
    var cartItems = getCartFromLocalStorage();
    $("#cartSl").text(cartItems.length)

});

function removeProductByIdAndSize(id, size) {

    let products = getCartFromLocalStorage();
    if (products.length == 1) {
        localStorage.removeItem('cartProduct');
        loadData();
        return;
    }
    if (!products) {
        return;
    }
    products = products.filter(product => !(product.Item_id === id ));
    saveCartToLocalStorage(products, 60 * 24 * 7);
    debugger


}

function updateCart(itemID, quantity) {

    var cartItems = getCartFromLocalStorage();

    var existingProduct = cartItems.find(function (item) {
        return item.Item_id === itemID;
    });

    if (existingProduct) {
        existingProduct.quantity += quantity;
        if (existingProduct.quantity < 1) {
            existingProduct.quantity = 1;
        }

        var a = existingProduct.quantity;
        $(`#quantity_${itemID}`).val(a);


    }
    saveCartToLocalStorage(cartItems, 60 * 24 * 7);
    $(`#priceTotal_${itemID}`).text(parseInt(existingProduct.quantity) * parseFloat(existingProduct.price));
    $(`#priceTotal_${itemID}`).attr("data-price", parseInt(existingProduct.quantity) * parseFloat(existingProduct.price));
    lstChecked = [];
    getTotalPrice();
    fillprice();
}

function saveCartToLocalStorage(cartItems, expirationInMinutes) {
    const now = new Date();
    const expirationTime = now.getTime() + expirationInMinutes * 60000;

    const cartData = {
        items: cartItems,
        expiration: expirationTime
    };

    const cartJson = JSON.stringify(cartData);
    localStorage.setItem("cartProduct", cartJson);
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
var lstChecked = [];
$(document).on("change", ".form-check-input", function () {
    lstChecked = [];
    getTotalPrice();
    fillprice();


});

function fillprice() {
    var total = 0;
    var totalBill = 0;
    var VAT = 0;
    var cartItems = getCartFromLocalStorage();
    lstChecked.forEach(function (checkedItem) {

        var existingProduct = cartItems.find(function (item) {
            return item.Item_id === checkedItem.Item_id ;
        });

        if (existingProduct) {

            total += (parseInt(existingProduct.quantity) * parseFloat(existingProduct.price));
            VAT = (total * 5 / 100);
            totalBill = (total + VAT);
        }
    });

    $("#totalPrice").text(total);
    $("#totalPrice").attr("data-price", total)
    $("#VAT_Price").text(VAT);
    $("#VAT_Price").attr("data-price", VAT)
    $("#totalBill").text(totalBill);
    $("#totalBill").attr("data-price", totalBill)

    format();
}

function getTotalPrice() {
    $(".form-check-input:checked").each(function () {
        var id = $(this).data('pid');
        if (id != null && id != undefined && id != 0) {
            var pro = {
                Item_id: id
            }
            lstChecked.push(pro)
        }

    })
}

var lstCheckout = [];
function checkOut() {
    var cartItems = getCartFromLocalStorage();
    lstChecked.forEach(function (checkedItem) {
        var existingProduct = cartItems.find(function (item) {
            return item.Item_id === checkedItem.Item_id ;
        });

        if (existingProduct) {
            lstCheckout.push(existingProduct);
        }
    });

    // Send data to the checkout page
    if (lstCheckout.length > 0) {
        // Serialize the checkout data
        var checkoutData = JSON.stringify(lstCheckout);

        // Redirect to the checkout page with data in query parameter
        window.location.href = '/Checkout?data=' + encodeURIComponent(checkoutData);
    } else {
        alert('No items selected for checkout.');
    }
}
function getLienQuan() {
    debugger
    var cartItems = getCartFromLocalStorage();
    var lstID = [...new Map(cartItems.map(item => [item.productId, item])).keys()];
    $.ajax({
        url: "https://tcvtfcapi.azurewebsites.net/api/Product/SanPhamLienQuan",
        method: "post",
        contentType: "application/json",
        data: JSON.stringify(lstID),
        success: function (res) {
            var lstLQ = res.products;
            debugger
            var lstLienQuan = $('#lstLienQuan').empty();

            $.each(lstLQ, function (index, p) {
                var Avatar = "";
                if (p.image == null || p.image == "" || p.image == undefined) {
                    Avatar = "/image/imagelogo/ImageFail.jpg"
                } else {
                    Avatar = p.image;
                }
                var discout = "";
                var pricetxt = "";
                var price = 0;
                if (p.discoutPercent != null && p.discoutPercent != 0) {
                    discout = ` <div class="discount-badge">-${p.discoutPercent}%</div >`;
                    price = ((p.price) - (p.price * p.discoutPercent/100));
                    pricetxt = `<p class="product-price price text-center" data-price="${price}">
                                  ${price} 
                                  <del class="price" data-price="${p.price}" style="color:#333">${p.price}</del>
                               </p>` ;
                }
                else {
                    pricetxt = `<p class="price product-price text-center" data-price="${p.price}">${p.price} </p>`;
                }

                var html = `
             <div class="col-md-4 col-sm-6 col-lg-3 mb-4">
                <div class="card position-relative">
                    <a href="/public/ChiTietSanPham/${p.productId}">
                        <img src="${Avatar}" class="card-img-top product-img" alt="${p.productName}">
                    </a>
                    ${discout}
                    <div class="card-body mt-3">
                        <a href="/public/ChiTietSanPham/${p.productId}">
                            <h5 class="product-title text-center">${p.productName}</h5>
                            ${pricetxt}
                        </a>

                       <div class="d-flex" style="float:right ;margin :40px 20px 20px 0">
                            <button class="btn btn-outline-warning text-center" onclick="loadDetail(${p.productId})" >
                                <i class="fa fa-shopping-cart" style="color:#eb3636 ;font-size:20px;" ></i>
                            </button>
                        </div>
                    </div>
                </div>
            </div>`;
                lstLienQuan.append(html);
            });
            format();

        },
        error: function (res) {

        }
    });

}
function loadDetail(id) {
    window.location.href = "/public/ChiTietSanPham/" + id;
}
function checkTableData() {
    debugger
    var noDataMessage = document.getElementById('noDataMessage');

    var cartItems = getCartFromLocalStorage();

    if (cartItems.length == 0) {
        noDataMessage.style.display = 'block';

    } else {
        noDataMessage.style.display = 'none';
    }


}

