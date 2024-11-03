
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
        if (playerId == undefined) {
            playerId = null;
        }
        var SoAo = null;
        var tencauthu = "";

        if (playerId != -1 && playerId != 0 && playerId != undefined) {
            var InfoAo = $("#shirtNumber option:selected").text();
        } else {

            if (playerId == -1 && $("#customAo").is(":checked")) {
                tencauthu = $("#nameAoCustom").val();
                SoAo = $("#SoAoCustom").val();
                var InfoAo = tencauthu + " - số " + SoAo;
                debugger
            } else {
                var InfoAo = "";

            }
        }

        var soLuong = 1;
        var cartItems = getCartFromLocalStorage();
        var item_id = cartItems.length > 0 ? Math.max(...cartItems.map(item => item.Item_id)) + 1 : 1;
        debugger
        var existing = cartItems.find(function (item) {
            return item.productId === productId && item.size === size && item.shirtNumber === playerId;
        });
        if ($("#shirtNumber").val() > 0) {
            if (existing) {
                existing.quantity += 1;
            } else {
                var Product = {
                    Item_id: item_id ,
                    productId: productId,
                    size: size,
                    avartar: Avartar,
                    price: Price,
                    nameProduct: TenSP,
                    quantity: soLuong,
                    shirtNumber: playerId,
                    TenInAo: InfoAo,
                    SoAo: SoAo,
                    TenCauThu: tencauthu
                };
                cartItems.push(Product);
            }
        }
        else {
            var existingProduct = cartItems.find(function (item) {

                return item.productId === productId && item.size === size;
            });

            if (existingProduct && $("#customAo").is(":checked") == false && ($("#shirtNumber").val() == -1 || $("#shirtNumber").val() == 0)) {

                existingProduct.quantity += 1;
            } else {
                var Product = {
                    Item_id: item_id,
                    productId: productId,
                    size: size,
                    avartar: Avartar,
                    price: Price,
                    nameProduct: TenSP,
                    quantity: soLuong,
                    shirtNumber: playerId,
                    TenInAo: InfoAo,
                    SoAo: SoAo,
                    TenCauThu: tencauthu

                };
                cartItems.push(Product);
            }
        }

       
        saveCartToLocalStorage(cartItems, 60 * 24 * 7);
        alert("thêm thành công")
    }
}


function validateAddCart() {
    debugger
    var check = true;
    if ($("#sizeSp").val() == null || $("#sizeSp").val().trim() == "" || $("#sizeSp").val() == undefined) {
        check = false
        alert("Vui lòng chọn kích thước.");
    }
    if ($("#customAo").is(":checked") == true) {
        if ($("#nameAoCustom").val() == "" && $("#SoAoCustom").val() == "") {
            check = false;
            alert("Vui lòng nhập thông tin áo nếu bạn muốn tự thiết kế .");
            document.getElementById("previewModal").style.display = "flex";
        }

    }

    return check;

}
function ChonSoAo() {
    debugger
    var soao = $("#shirtNumber").val();
    if (soao != -1) {
        $("#customAo").prop("checked", false)
    } 
}

function customAo() {
    debugger
    if ($("#customAo").is(":checked")) {
        debugger
        document.getElementById("previewModal").style.display = "flex";
        $("#shirtNumber").val(-1)
    }
    debugger
}

function closeModal() {
    document.getElementById("previewModal").style.display = "none";
}

function updateText(elementId, text) {
    document.getElementById(elementId).textContent = text;
}

window.onclick = function (event) {
    if (event.target == document.getElementById("previewModal")) {
        closeModal();
    }
}
