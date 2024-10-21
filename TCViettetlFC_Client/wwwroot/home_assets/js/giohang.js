

const key = "keyCartSep490G53keyCartSep490G53";
//const encryptedCart = encryptData(cartItems, key);
//localStorage.setItem("cart", encryptedCart);

//const storedCart = localStorage.getItem("cart");
//const decryptedCart = decryptData(storedCart, key);



// Mã hóa giỏ hàng


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
    format();

});
function loadData() {

    var cartItems = [];
    cartItems = getCartFromLocalStorage();
    debugger
    if (cartItems != null && cartItems.length > 0) {

        var tbody = $('#bodyData').empty();
        debugger
        $.each(cartItems, function (index, item) {
            var Image = "";
            if (item.avartar == null || item.avartar == "" || item.avartar == undefined) {
                Image = "/image/imagelogo/ImageFail.jpg"
            } else {
                Image = item.avartar;
            }
            

            var total = parseFloat(item.price) * item.quantity;
            debugger
            var teninao = "";
            if (item.TenInAo.trim() == "" || item.TenInAo.trim() == null) {
                teninao = "Không in tên"
            } else {
                teninao = item.TenInAo;
            }
            var html = ` <tr>
                            <td>
                                <div class="form-check">
                                    <input class="form-check-input" type="checkbox" data-pid="${item.productId}" data-size="${item.size}" >
                                </div>
                            </td>
                            <td>
                                <div style="display:flex">
                                <a href="/public/ChiTietSanPham/${item.productId}" >
                                    <img src="${Image}" alt="" class="ImgProduct">
                                </a>
                                    <div class="product-info" style="margin-top: 2px; cursor: pointer;">
                                       
                                        <span class="product-name" title="${item.nameProduct}">${item.nameProduct}</span>
                                        <span  title="${item.size} - ${teninao}" >${item.size} - ${teninao} </span> <span></span>
                                     
                                       
                                    </div>
                                </div>
                               
                            </td>
                            <td class="quantity">
                                <div style="display:flex">
                                   <button class="quantity-btn" onclick="updateCart(${item.productId}, '${item.size}', ${-1})">-</button>

                                    <input type="text" id="quantity_${item.productId}_${item.size}" value="${item.quantity}" class="quantity-input" min="1">
                                    <button class="quantity-btn" onclick="updateCart(${item.productId}, '${item.size}', ${1})">+</button>

                                </div>
                      
                            </td>
                            <td class="price" data-price="${item.price}">${item.price}</td>
                            <td class="price" id="priceTotal_${item.productId}_${item.size}" data-price="${total}">${total}</td>
                            <td class="text-center">
                                <button class="btn btn-danger w-100" data-id="${item.productId}" style="margin-top :10px">Xóa</button>
                            </td>
                        </tr>`;

            tbody.append(html);
            debugger
        });
        format();


    } else {

    }

}

function updateCart(productId, size, quantity) {
    debugger
    var cartItems = getCartFromLocalStorage();

    var existingProduct = cartItems.find(function (item) {
        return item.productId === productId && item.size === size;
    });

    if (existingProduct) {
        existingProduct.quantity += quantity;  
        if (existingProduct.quantity < 1) {
            existingProduct.quantity = 1;  
        }

        var a = existingProduct.quantity;
        $(`#quantity_${productId}_${size}`).val(a);  
       
        debugger
    }
    saveCartToLocalStorage(cartItems, 60 * 24 * 7);
    $(`#priceTotal_${productId}_${size}`).text(parseInt(existingProduct.quantity) * parseFloat(existingProduct.price));
    $(`#priceTotal_${productId}_${size}`).attr("data-price", parseInt(existingProduct.quantity) * parseFloat(existingProduct.price));
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
var lstChecked = [];
$(document).on("change", ".form-check-input", function () {
    lstChecked = [];
    getTotalPrice();
    fillprice();
    debugger
    
});

function fillprice() {
    var total = 0;
    var totalBill = 0;
    var VAT = 0;
    var cartItems = getCartFromLocalStorage();
    lstChecked.forEach(function (checkedItem) {
        debugger
        var existingProduct = cartItems.find(function (item) {
            return item.productId === checkedItem.ProID && item.size === checkedItem.Size;
        });

        if (existingProduct) { 
            debugger
            total += (parseInt(existingProduct.quantity) * parseFloat( existingProduct.price));
            VAT += (parseInt(existingProduct.quantity) * parseFloat(existingProduct.price) * 10 / 100);
            totalBill += ( (parseInt(existingProduct.quantity) * parseFloat(existingProduct.price) ) + VAT);
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
        var size = $(this).data('size');
        debugger
        if (id != null && id != undefined && id != 0) {
            var pro = {
                ProID: id,
                Size: size
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
            return item.productId === checkedItem.ProID && item.size === checkedItem.Size;
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
