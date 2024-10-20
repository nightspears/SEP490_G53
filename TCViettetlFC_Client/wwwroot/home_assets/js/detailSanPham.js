
$(document).on("click", ".size", function () {
    $(".size").css("border", "1px solid #979da0");
    $(this).css("border", "1px solid red");
    var a = $(this).data('size');
    $("#sizeSp").val(a);
});
$(document).ready(function () {
    format();

});
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


function AddToCart(id) {
    debugger
    if (validateAddCart()) {
        var productId = id;
        var size = $("#sizeSp").val();
        var Avartar = $("#imgAvatar").attr("src");
        var Price = $("#giaSP").data("price");
        var TenSP = $("#TenSP").text();
        var playerId = $("#shirtNumber").val();
        if (playerId != null && playerId != 0 && playerId != undefined) {
            var InfoAo = $("#shirtNumber option:selected").text();
        } else {
            var InfoAo = "";
        }
        
        var soLuong = 1;
        debugger
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
                shirtNumber: playerId,
                TenInAo: InfoAo

            };
            cartItems.push(Product);
        }
        saveCartToLocalStorage(cartItems,60*24*7);
        alert("thêm thành công")
    }
}


function validateAddCart() { 
    var check = true;
    if ($("#sizeSp").val() == null || $("#sizeSp").val().trim() == "" || $("#sizeSp").val() == undefined) {
        check = false
        alert("Vui lòng chọn kích thước.");
    }

    return check;

}